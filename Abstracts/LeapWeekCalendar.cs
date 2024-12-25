using System;

namespace WeirdCalendars {
    public abstract class LeapWeekCalendar : WeirdCalendar {

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            //Default to Monday start
            ValidateDateTime(time);
            return (DayOfWeek)(GetDayOfYear(time) % 7);
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 371 : 364;
        }
    }
}