using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class AethodianCalendar : WeirdCalendar {

        public override string Author => "Miles Bradley Huff";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Aethodian_calendars");

        protected override DateTime SyncDate => new DateTime(2025, 1, 1, 15, 2, 24);
        protected override int SyncOffset => -1988;

        // Author's format example incorrectly uses the Earthling epoch instead of the Default epoch, which he
        // expressly provides: "The calendar starts from 2010-04-22T12:00:00-04:00, the date Theodia was founded."

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.Unknown;

        public override int DaysInWeek => 6;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("c", "Compact format")
        };

        protected override double TimescaleFactor => 1.0075;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeek)((GetDayOfMonth(time) - 1) % 6);
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 36;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 144;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Month 1", "Month 2", "Month 3", "Month 4", "", "", "", "", "", "", "", "", "" }, new string[] { "M1", "M2", "M3", "M4", "", "", "", "", "", "", "", "", "" }, new string[] { "Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6", "" }, new string[] { "D1", "D2", "D3", "D4", "D5", "D6", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            string y = ld.Year.Dozenal();
            int inc = ld.Year < 0 ? 1 : 0;
            string yy = FixNegativeYears(y.PadLeft(2 + inc, '0'));
            string yyyy = FixNegativeYears(y.PadLeft(4 + inc, '0'));
            string m = ld.Month.Dozenal();
            string mm = m.PadLeft(2, '0');
            string d = ld.Day.Dozenal();
            string dd = d.PadLeft(2, '0');
            string t = (ld.TimeOfDay.TotalHours / 6).Dozenal(2);
            fx.LongTimePattern = $"{t}";
            fx.ShortTimePattern = fx.LongTimePattern;
            fx.LongDatePattern = FixDigits(fx.LongDatePattern, yyyy, yy, mm, m, dd, d);
            fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, yyyy, yy, mm, m, dd, d);
            if (format == "G" || format == "g") fx.DateAndTimeSeparator = ":";
            else fx.Format = FixDigits(format, yyyy, yy, mm, m, dd, d).ReplaceUnescaped("c", $"{y.PadLeft(3, '0')}.{dd}:{fx.LongTimePattern}");
            return fx;
        }
    }
}
