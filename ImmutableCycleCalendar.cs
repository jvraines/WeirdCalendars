using System;
using System.Globalization;

namespace WeirdCalendars {
    public class ImmutableCycleCalendar : FixedCalendar {
 
        public override string Author => "fimbulvetr@yahoo.com";
        public override Uri Reference => new Uri("http://web.archive.org/web/20060630130851/http://www.geocities.com:80/fimbulvetr/index.html");

        // Interludes reckoned as their own months. Numeric methods will return a month number
        // from 1 to 16. ToStringWC, however, will return canonical names and numbers.
        // Numeric month strings are H/HI, V/VI, E/EI, and A/AI for the interludes.
        //
        // The author miscalculates major and minor cycles in his examples, making them
        // zero-based instead of one-based.The author's second example date of June 2, 2000
        // (Gregorian) should have been June 22, 2000. Finally, the author incorrectly shows
        // Extremas at the ends of the last 2 months.

        protected override DateTime SyncDate => new DateTime(2020, 12, 21);
        protected override int SyncOffset => 753;

        private bool IsInterlude(int month) {
            return (month - 1) % 4 == 0;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 1);
            return IsInterlude(month) ? 1 : 0;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 6:
                case 7:
                case 8:
                case 10:
                case 11:
                case 12:
                    return 30;
                case 2:
                case 3:
                case 4:
                case 14:
                case 15:
                case 16:
                    return 29;
                default:
                    return month == 1 && !IsLeapYear(year) ? 2 : 3;
            }
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 16;
        }

        public int GetMajorCycle(int year) {
            return (year - 1) / 128 + 1;
        }

        public int GetMinorCycle(int year) {
            return (year - 1) % 128 / 4 + 1;
        }

        public int GetMinorCycleYear(int year) {
            return (year - 1) % 4 + 1;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 1 && day == 3;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && year % 128 != 0;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return IsInterlude(month) || day == 0 || day == 29;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return IsInterlude(month) ? "" : day == 0 ? "Princeps" : "Extremas";
        }

        private static string[] interlude = { "Hibernal", "Vernal", "Aestival", "Autumnal" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Primihibernum", "Medihibernum", "Posthibernum", "Primivernum", "Medivernum", "Postvernum", "Primiaestus", "Mediaestus", "Postaestus", "Primiautumnus", "Mediautumnus", "Postautumnus", "" }, new string[] {"Phb", "Mhb", "Thb", "Pvr", "Mvr", "Tvr", "Pas", "Mas", "Tas", "Pat", "Mat", "Tat", "" }, new string[] { "Dies Solis", "Dies Lunae", "Dies Martis", "Dies Mercurii", "Dies Jovis", "Dies Veneris", "Dies Saturnii" }, new string[] { "Sol", "Lun", "Mar", "Mer", "Jov", "Ven", "Sat" });  
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.DayShortName = IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day);
            }
            int m = ymd.Month - (ymd.Month - 1) / 4 - 1;
            string mn;
            string dr = ymd.Day == 0 || ymd.Day == 29 ? "" : ymd.Day.ToRoman();
            string yr = $"{GetMinorCycleYear(ymd.Year).ToRoman()}.{GetMinorCycle(ymd.Year).ToRoman()}.{GetMajorCycle(ymd.Year).ToRoman()}";
            if (IsInterlude(ymd.Month)) {
                string n = interlude[(ymd.Month - 1) / 4];
                fx.MonthFullName = $"{n} Interlude";
                fx.MonthShortName = n.Substring(0, 3);
                mn = "HVEA".Substring((ymd.Month - 1) % 4, 1);
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, $"{mn}I", mn);
                fx.LongDatePattern = $"'{dr} {fx.MonthFullName}, {yr}'";
                fx.Format = FixDigits(format, null, null, $"{mn}I", mn);
            }
            else {
                fx.MonthFullName = dtfi.MonthNames[m - 1];
                fx.MonthShortName = dtfi.AbbreviatedMonthNames[m - 1];
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, $"{m:00}", m);
                fx.LongDatePattern = $"dddd'{(dr == "" ? " of" : $", {dr}")} {fx.MonthFullName}, {yr}'";
                fx.Format = FixDigits(format, null, null, $"{m:00}", m);
            }
            return fx;
        }
    }
}
