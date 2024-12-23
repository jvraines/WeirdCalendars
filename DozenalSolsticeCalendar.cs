﻿using System;
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

        public override int DaysInWeek => 6;

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
            ValidateDateParams(year, month, day, 0);
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
            string t = $"'{((int)(time.TimeOfDay.Ticks / triceTicks)).Dozenal().PadLeft(4,'0')}'";
            fx.LongTimePattern = t;
            fx.ShortTimePattern = t;
            var ld = ToLocalDate(time);
            string y = ld.Year.Dozenal();
            string yy = y.Substring(y.Length - 3);
            string m = ld.Month.Dozenal();
            string mm = ld.Month.Dozenal().PadLeft(2, '0');
            string d = ld.Day.Dozenal();
            string dd = ld.Day.Dozenal().PadLeft(2, '0');
            fx.LongDatePattern = FixDigits(fx.LongDatePattern, yy, y, mm, m, dd, d);
            fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, yy, y, mm, m, dd, d);
            fx.Format = FixDigits(format, yy, y, mm, m, dd, d);
            return fx;
        }
    }
}
