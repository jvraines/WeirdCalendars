using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class DoubleMonthCalendar : LeapWeekCalendar {

        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Double-Month_Calendar");
        public override string Author => "Christoph Päper";

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public DoubleMonthCalendar() => Title = "Double-Month Calendar";

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("I", "\"ISO\" format")
        };

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 6;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 3 != 0 || month == 6 && IsLeapYear(year) ? 63 : 56;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsISOLeapYear(year);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day > 56;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] {"Sestal-1", "Sestal-2", "Sestal-3", "Sestal-4", "Sestal-5", "Sestal-6", "", "", "", "", "", "", ""}, new string[] {"S1", "S2", "S3", "S4", "S5", "S6", "", "", "", "", "", "", ""});
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int day = ToLocalDate(time).Day;
            int w = (day - 1) / 7 + 1;
            int d = (day - 1) % 7 + 1;
            fx.Format = format.ReplaceUnescaped("I", $"yyyy-M-{w}-{d}");
            return fx;
        }
    }
}
