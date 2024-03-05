using System;

namespace WeirdCalendars {
    public class NegativeLeapYearCalendar : WeirdCalendar {

        public override string Author => "Various";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Negative_Leap_Year_Solar_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public NegativeLeapYearCalendar() => Title = "Negative Leap Year Solar Calendar";
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if ((month & 1) == 1) return 30;
            int r = year % 4;
            // Incorporating suggestion to follow Gregorian exceptions
            if (month == 12 && r == 0 && !IsLeapYear(year)) return 30;
            return month == r * 4 ? 30 : 31;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }
    }
}
