using System;
using System.Globalization;

namespace WeirdCalendars {
    public class InternationalCivilCalendar : WeirdCalendar {
 
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/International_Civil_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 3, 22);
        protected override int SyncOffset => 0;

        private static int[] MonthDays = { 30, 30, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30 };
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int i = (year + 600) / 1700 % 12;
            int d = MonthDays[(i + month - 1) % 12];
            return month == 1 && IsLeapYear(year) ? d + 1 : d;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int r = year % 33;
            return r > 0 && r % 4 == 0 && year % 400 != 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 1 && IsLeapYear(year) && day == GetDaysInMonth(year, 1);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Boreal1", "Boreal2", "Boreal3", "Boreal4", "Boreal5", "Boreal6", "Austral1", "Austral2", "Austral3", "Austral4", "Austral5", "Austral6", ""}, new string[] { "B1", "B2", "B3", "B4", "B5", "B6", "A1", "A2", "A3", "A4", "A5", "A6", ""});
        }
    }
}
