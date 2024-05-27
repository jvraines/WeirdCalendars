using System;
using System.Globalization;

namespace WeirdCalendars {
    public class CardassianCalendar : WeirdCalendar {
 
        public override string Author => "Christopher L. Bennett";
        public override Uri Reference => new Uri("https://christopherlbennett.wordpress.com/home-page/star-trek-fiction/dti-watching-the-clock/dti-calendar-notes/");

        protected override DateTime SyncDate => new DateTime(2024, 1, 12, 19, 52, 19, 200);
        protected override int SyncOffset => -1617;

        protected override int DaysInWeek => 0;

        protected override double TimescaleFactor => 1.083;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 3;
        }

        private int AdjustWaning(int year, int days) => days - (year % 12 == 0 ? 2 : year % 3 == 0 ? 1 : 0);

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                    return 126;
                case 2:
                    return 116;
                default:
                    return AdjustWaning(year, 117);
            }
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return AdjustWaning(year, 359);
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
            SetNames(dtfi, new string[] { "Rising Season", "Middle Season", "Waning Season", "", "", "", "", "", "", "", "", "", "" });
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
