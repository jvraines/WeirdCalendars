using System;
using System.Globalization;

namespace WeirdCalendars {
    public class CalendarACalendar : FixedCalendar {
        
        public override string Author => "Tim Forsythe";
        public override Uri Reference => new Uri("https://web.archive.org/web/20180323051519/http://alpobalognia.appspot.com/");

        protected override DateTime SyncDate => new DateTime(2024, 3, 20);
        protected override int SyncOffset => 4727;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 5;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 5 ? 91 : FestivalLength(year);
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 364 + FestivalLength(year);
        }

        public int GetCycle(DateTime time) => ToLocalDate(time).Year / 9000 + 1;

        public int GetCollapse(DateTime time) => (ToLocalDate(time).Year - 1) % 9000 / 450 + 1;

        public int GetGeneration(DateTime time) => (ToLocalDate(time).Year - 1) % 450 / 25 + 1;

        public int GetStage(DateTime time) => (ToLocalDate(time).Year - 1) % 25 / 5 + 1;

        private int FestivalLength(int year) {
            if (year % 9000 == 0) return 5;
            if (year % 450 == 0) return 4;
            if (year % 25 == 0) return 3;
            if (year % 5 == 0) return 2;
            return 1;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return FestivalLength(year) > 1;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 5 && day > 1;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 5;
        }

        private static string[] Feasts = { "Dead", "Mother", "Earth", "Father", "Living" };
        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return $"Feast of the {Feasts[day - 1]}";
        }

        private static string[] ShortFeasts = { "Ar", "Om", "Po", "Og", "Ri" };
        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return ShortFeasts[day - 1];
        }

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % DaysInWeek;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Spring", "Summer", "Autumn", "Winter", "Feast", "", "", "", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int generation = GetGeneration(time);
            int collapse = GetCollapse(time);
            int stage = GetStage(time);
            var ld = ToLocalDate(time);
            int year = (ld.Year - 1) % 5 + 1;
            fx.LongDatePattern = $"{GetCycle(time)}.{collapse}.{generation}.{stage}.{year}.{ld.Day}";
            fx.ShortDatePattern = $"{(ld.Month < 5 ? "dddd, d MMMM" : "dddd")} {collapse}.{generation}.{stage}.{year}";
            if (fx.Format == "f") fx.Format = $"{fx.ShortDatePattern} {fx.ShortTimePattern}"; //force short date for standard date/time display
            return fx;
        }
    }
}
