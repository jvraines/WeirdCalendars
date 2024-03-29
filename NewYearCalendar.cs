using System;
using System.Collections.Generic;
using System.Text;

namespace WeirdCalendars {
    public class NewYearCalendar : FixedCalendar {
 
        public override string Author => "Orfeas68";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/New_Year_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 6 && IsLeapYear(year) ? 32 : month % 3 == 0 || month == 1 ? 31 : 30;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 32;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 1 && day == 1 || day == 32;
        }
        
        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return day == 32 ? "Leap Day" : "New Year";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 32 ? "Leap" : "New";
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = GetDayOfYear(time);
            int s = IsLeapYear(time.Year) && d > 91 * 2 ? 3 : 2;
            return (d - s) % 7;
        }
    }
}
