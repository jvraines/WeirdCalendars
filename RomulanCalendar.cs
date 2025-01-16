using System;
using System.Globalization;

namespace WeirdCalendars {
    public class RomulanCalendar : VulcanCalendar {
        
        public override string Author => "Christopher L. Bennett";
        public override Uri Reference => new Uri("https://christopherlbennett.wordpress.com/home-page/star-trek-fiction/dti-watching-the-clock/dti-calendar-notes/");

        protected override DateTime SyncDate => new DateTime(2023, 5, 19, 4, 4, 48);
        protected override int SyncOffset => -1127;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override string Notes => "Years After Settlement";

        protected override double TimescaleFactor => 1.053;

        protected override long MinuteTicks => TimeSpan.TicksPerDay / 1800;
        protected override long SecondTicks => TimeSpan.TicksPerDay / 90000;
        protected override long MilliTicks => TimeSpan.TicksPerDay / 9000000;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                    return 42;
                case 6:
                case 9:
                    return 46;
                case 11:
                    return 36;
                case 12:
                    return IsLeapYear(year) ? 17 : 16;
                default:
                    return 44;
            }
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 493 : 492;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year & 1) == 0;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Z'at", "D'ruh", "K'riBrax", "re'T'Khutai", "T'keKhuti", "Khuti", "ta'Krat", "K'ri'lior", "et'khior", "T'ke'Tas", "Tasmeen", "Havreen", "" }, new string[] { "Z'a", "D'r", "K'B", "re'", "T'K", "Khu", "ta'", "K'l", "et'", "T'T", "Tas", "Hav", "" });
            dtfi.LongDatePattern = dtfi.LongDatePattern.Replace("dddd,", "").Replace("ddd", "").Trim();
        }
    }
}
