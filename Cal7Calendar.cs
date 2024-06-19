using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class Cal7Calendar : LeapWeekCalendar {

        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Cal7");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public Cal7Calendar() => Title = "Cal7";

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("I", "\"ISO\" format")
        };

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)(GetDayOfMonth(time) % 7);
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 7;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 0 || month == 7 && !IsLeapYear(year) ? 49 : 56;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 7 == 0 || year % 29 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 7 && day > 49;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            SetNames(dtfi, new string[] { "Monmonth", "Tuesmonth", "Wednesmonth", "Thursmonth", "Frimonth", "Saturmonth", "Sunmonth", "", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("I")) {
                int day = ToLocalDate(time).Day;
                int w = (day - 1) / 7 + 1;
                int d = (day - 1) % 7 + 1;
                fx.Format = format.ReplaceUnescaped("I", $"yyyy-M-{w}-{d}");
            }
            return fx;
        }
    }
}
