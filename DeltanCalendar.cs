using System;
using System.Globalization;

namespace WeirdCalendars {
    public class DeltanCalendar : WeirdCalendar {

        public override string Author => "Christopher L. Bennett";
        public override Uri Reference => new Uri("https://christopherlbennett.wordpress.com/home-page/star-trek-fiction/dti-watching-the-clock/dti-calendar-notes/");

        protected override DateTime SyncDate => new DateTime(2024, 1, 26, 0, 57, 36);
        protected override int SyncOffset => 3945;

        public override int DaysInWeek => 0;

        protected override double TimescaleFactor => 1.24;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 4 && IsLeapYear(year) ? 52 : 51;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 205 : 204;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year & 1) == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 52;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Bountiful Womb", "Racing Heart", "Nurturing Hand", "Inner Eye", "", "", "", "", "", "", "", "", "" });
            dtfi.LongDatePattern = dtfi.LongDatePattern.Replace("dddd,", "").Replace("ddd", "").Trim();
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            fx.Format = format.ReplaceUnescaped("dddd,", "").ReplaceUnescaped("ddd", "").Trim();
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
