using System;
using System.Globalization;

namespace WeirdCalendars {
    public class RiparianCalendar : WeirdCalendar {
        
        public override string Author => "nhprman";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Riparian_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 3, 1);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 30;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return base.IsLeapYear(year + 1, era);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Ryo", "April", "May", "June", "Quintil", "Sextil", "September", "October", "November", "December", "Undecember", "February", "Ultimus" });
        }
    }
}
