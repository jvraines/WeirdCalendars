using System;
using System.Collections.Generic;
using AA.Net;
using System.Globalization;

namespace WeirdCalendars {
    public class DozenalSolsticeCalendar : FixedCalendar {
 
        public override string Author => "Paul Rapoport";
        public override Uri Reference => new Uri("https://clock.dozenal.ca/pdf/dozenal-calendar.pdf");

        protected override DateTime SyncDate => new DateTime(2023, 12, 22);
        protected override int SyncOffset => 9564;
        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public new enum DayOfWeekWC {
            Blankday = -1,
            Ruber,
            Aurantium,
            Flavus,
            Viridis,
            Caeruleus,
            Violaceus
        }

        private enum Pattern {
            A,
            B1,
            B2
        }

        private static Dictionary<int, Pattern> Years = new Dictionary<int, Pattern>();
        private Pattern FetchPattern(int year) {
            if (!Years.TryGetValue(year, out Pattern p)) {
                int gYear = year - SyncOffset;
                int y1 = (int)(Earth.SeasonStart(gYear, Earth.Season.December) + 0.5);
                int y2 = (int)(Earth.SeasonStart(gYear + 1, Earth.Season.December) + 0.5);
                int days = y2 - y1;
                if (days == 366) p = Pattern.A;
                else {
                    int y0 = (int)(Earth.SeasonStart(gYear - 1, Earth.Season.December) + 0.5);
                    int s = (int)(Earth.SeasonStart(gYear, Earth.Season.June) + 0.5);
                    p = s - y0 == 182 ? Pattern.B1 : Pattern.B2;
                }
                Years.Add(year, p);
            }
            return p;
        }

        protected override int DaysInWeek => 6;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 6:
                case 7:
                case 8:
                case 9:
                    return 31;
                case 5:
                    Pattern p = FetchPattern(year);
                    return p == Pattern.A || p == Pattern.B2 ? 31 : 30;
                case 10:
                    p = FetchPattern(year);
                    return p == Pattern.A || p == Pattern.B1 ? 31 : 30;
                default:
                    return 30;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return FetchPattern(year) == Pattern.A;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day);
            return day == 31;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return "S-day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return "S";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Aigokerōs", "Hydrokhoos", "Ikhthyes", "Krios", "Tauros", "Didymoi", "Karkinos", "Leōn", "Parthenos", "Zygos", "Skorpios", "Toxotēs", "" }, null, new string[] { "Ruber", "Aurantius", "Flavus", "Viridis", "Caeruleus", "Violaceus", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            const double triceTicks = TimeSpan.TicksPerDay / (12 * 12 * 12);
            string t = $"'{Dozenal((int)(time.TimeOfDay.Ticks / triceTicks)).PadLeft(4,'0')}'";
            fx.LongTimePattern = t;
            fx.ShortTimePattern = t;
            var ld = ToLocalDate(time);
            string y = Dozenal(ld.Year);
            string yy = y.Substring(y.Length - 3);
            string m = Dozenal(ld.Month);
            string mm = Dozenal(ld.Month).PadLeft(2, '0');
            string d = Dozenal(ld.Day);
            string dd = Dozenal(ld.Day).PadLeft(2, '0');
            fx.LongDatePattern = FixDigits(fx.LongDatePattern, yy, y, mm, m, dd, d);
            fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, yy, y, mm, m, dd, d);
            fx.Format = FixDigits(format, yy, y, mm, m, dd, d);
            return fx;

            string Dozenal (int n) {
                return n.ToBase(12).Replace("A", "↊").Replace("B", "↋");
            }
        }
    }
}
