using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class SentinelCalendar : LeapWeekCalendar {

        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Sentinel_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 3, 19);
        protected override int SyncOffset => -2010;
        public override DateTime MinSupportedDateTime => new DateTime(2011, 3, 20);

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> {
            ("l", "Lesson")
        };

        public string GetDozenalDay(int day) {
            int a = day / 12;
            int b = day % 12;
            return $"{a}{(b < 10 ? b.ToString() : b == 10 ? "D" : "Z")}";
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeek)((ToLocalDate(time).Day - 1) % 7);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 7 && IsLeapYear(year) ? 35 : 28;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        private static List<string> Lessons = new List<string> { "Faith", "Reason", "Hope", "Reflection", "Love", "Illumination", "Wisdom", "Courage", "Understanding", "Discipline", "Knowledge", "Strength", "Nurture" };

        public string GetLesson(int month) {
            if (month < 1 || month > 13) throw new ArgumentOutOfRangeException("month", "Value must be from 1 to 13.");
            return Lessons[month - 1];
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 6 == 0 || year % 50 == 0;
        }
            
        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day > 28 && month == 7 && IsLeapYear(year);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Tomas", "Hugh", "Ashia", "Arabela", "Amadea", "Jomei", "Sophia", "Cheyenne", "Olivia", "Cosmos", "Gyan", "Gabriel", "Eden" }, null, new string[] { "Plan", "Work", "Sow", "Observe", "Reap", "Enjoy", "Rest" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            FixNegativeYears(fx, time);
            var ymd = ToLocalDate(time);
            string doz = GetDozenalDay(ymd.Day);
            fx.Format = FixDigits(format, null, null, null, null, doz, doz).ReplaceUnescaped("l", $"'{GetLesson(ymd.Month)}'");
            fx.LongDatePattern = FixDigits(fx.LongDatePattern, null, null, null, null, doz, doz);
            fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, null, null, doz, doz);
            return fx;
        }
    }
}
