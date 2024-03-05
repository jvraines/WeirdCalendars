using System;
using System.Globalization;

namespace WeirdCalendars {
    public class InvariableCalendar : FixedCalendar {
        
        protected override DateTime SyncDate => new DateTime(2018, 1, 1);
        protected override int SyncOffset => 0;

        public override string Author => "L. A. Grosclaude";
        public override Uri Reference => new Uri("https://sundaymagazine.org/2010/06/25/a-proposed-plan-for-an-invariable-calendar/");

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month == 1 || month == 7 && IsLeapYear(year) ? 0 : 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month % 3 == 0 ? 31 : 30) + (month == 1 || IsLeapYear(year) && month == 7 ? 1 : 0);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0;
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return base.IntercalaryDayName(year, month, day) == "Leap Day" ? "Leap Day" : @"New Year\'s Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryDayName(year, month, day) == "Leap Day" ? "Leap" : "New";
        }

        protected override int WeekdayNumber(DateTime time) {
            //Default to Sunday start
            int d = GetDayOfYear(time);
            int s = IsLeapYear(time.Year) && d > 91 * 2 ? 1 : 0;
            return (d - s) % 7;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return (month == 7 && day == 0 && IsLeapYear(year));
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }
    }
}
