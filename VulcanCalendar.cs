using System;
using System.Globalization;

namespace WeirdCalendars {
    public class VulcanCalendar : WeirdCalendar {

        public override string Author => "Christopher L. Bennett";
        public override Uri Reference => new Uri("https://christopherlbennett.wordpress.com/home-page/star-trek-fiction/dti-watching-the-clock/dti-calendar-notes/");

        protected override DateTime SyncDate => new DateTime(2024, 1, 18, 7, 52, 20);
        protected override int SyncOffset => 6560;

        public override int DaysInWeek => 0;

        protected override double TimescaleFactor => 1.058;

        protected override long HourTicks => TimeSpan.TicksPerDay / 18;
        protected override long MinuteTicks => TimeSpan.TicksPerDay / 972;
        protected override long SecondTicks => TimeSpan.TicksPerDay / 52488;
        protected override long MilliTicks => TimeSpan.TicksPerDay / 5248800;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 21;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 252;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Z'at", "D'ruh", "K'riBrax", "re'T'Khutai", "T'keKhuti", "Khuti", "ta'Krat", "K'ri'lior", "et'khior", "T'lakht", "T'ke'Tas", "Tasmeen", "" }, new string[] { "Z'a", "D'r", "K'B", "re'", "T'K", "Khu", "ta'", "K'l", "et'", "T'l", "T'T", "Tas", "" });
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
