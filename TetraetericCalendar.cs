using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class TetraetericCalendar : WeirdCalendar
    {
        public override string Author => "Hellerick Ferlibay";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Tetraeteric_Calendar");

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        protected override DateTime SyncDate => new DateTime(2020, 7, 21);
        protected override int SyncOffset => -1065;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> { ("c", "Compact format") };

        private int SolarCycle(int year) {
            return (year + 3) % 4 + 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int sc = SolarCycle(year);
            switch (month) {
                case 1:
                case 3:
                case 5:
                    return sc == 3 ? 29 : 30;
                case 2:
                case 4:
                    return sc == 3 ? 30 : 29;
                case 6:
                case 8:
                case 10:
                    return sc == 1 || sc == 4 ? 29 : 30;
                case 7:
                case 9:
                    return sc == 1 || sc == 4 ? 30 : 29;
                case 11:
                    return sc == 2 ? 29 : 30;
                case 12:
                    return sc == 2 ? 30 : 29;
                default:
                    return 30;
            };
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return SolarCycle(year) < 4 ? 12 : 13;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            switch (SolarCycle(year)) {
                case 1:
                case 3:
                    return 354;
                case 2:
                    return 355;
                default:
                    return 384;
            }
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 0;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return SolarCycle(year) == 4;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13;
        }

        public int Tetraeteris(int year) {
            return (year - 1) / 4 + 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Elapheboly", "Munichy", "Thargely", "Scirophory", "Hecatombey", "Metageitny", "Bedromy", "Pyanepsy", "Memactery", "Poseidey", "Gamely", "Anthestery", "Emboly" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            var ymd = ToLocalDate(time);
            int month = ymd.Month - 1;
            int sc = SolarCycle(ymd.Year);
            FormatWC fx = new FormatWC(format, dtfi);
            if (ymd.Month < 13) {
                fx.MonthFullName = $"{new string[] { "First", "Second", "Third", "Fourth" }[sc - 1]} {dtfi.MonthNames[month]}";
                fx.MonthShortName = $"{dtfi.AbbreviatedMonthNames[month]}.{sc}";
            }
            int t = Tetraeteris(ymd.Year);
            fx.LongDatePattern = $"'{ymd.Day.ToOrdinal()} day of the {fx.MonthFullName} of the {t.ToOrdinal()} Tetraeteris'";
            fx.ShortDatePattern = $"d'-{fx.MonthShortName}-{t:D3}'";
            fx.Format = fx.Format.ReplaceUnescaped("c", $"'{t:D3}-{sc}/{ymd.Month:D2}-{ymd.Day:D2}'");
            return fx;
        }
    }
}