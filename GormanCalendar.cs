using System;
using System.Globalization;

namespace WeirdCalendars {
    public class GormanCalendar : FixedCalendar {
        
        public override string Author => "Dave Gorman";
        public override Uri Reference => new Uri("http://gormano.blogspot.com/2008/01/problem-solving.html");

        protected override DateTime SyncDate => new DateTime(2024, 3, 1);
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
            return false;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 1);
            return month == 13 && day > 28;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return "Intermission";
        }

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % DaysInWeek;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "March", "April", "May", "June", "Quintilis", "Sextilis", "September", "October", "November", "December", "January", "February", "Gormanuary" });
        }
    }
}
