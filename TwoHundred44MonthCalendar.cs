using System;
using System.Globalization;

namespace WeirdCalendars {
    public class TwoHundred44MonthCalendar : WeirdCalendar {
        public override string Author => "Victor Engel";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/244_Month_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public enum DayOfWeekWC {
            DayOne = 1,
            DayTwo
        }

        protected override int DaysInWeek => 2;

        public TwoHundred44MonthCalendar() => Title = "244 Month Calendar";

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            var ld = ToLocalDate(time);
            return (DayOfWeek)ld.Day;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ld = ToLocalDate(time);
            return (DayOfWeekWC)ld.Day;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 244;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int cycleMonth = (year * 244 + month) % 322;
            return cycleMonth < 161 && (cycleMonth & 1) == 0 || cycleMonth > 160 && (cycleMonth & 1) == 1 ? 1 : 2;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            ValidationEnabled = false;
            int d = 0;
            for (int m = 1; m < 245; m++) d += GetDaysInMonth(year, m, 0);
            ValidationEnabled = true;
            return d;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, null, null, new string[] { "DayOne", "DayTwo", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            fx.DayFullName = $"Day{(ld.Day == 1 ? "One" : "Two")}";
            fx.DayShortName = ld.Day == 1 ? "One" : "Two";
            fx.MonthFullName = $"Month{ld.Month}";
            fx.MonthShortName = $"M{ld.Month}";
            return fx;
        }
    }
}
