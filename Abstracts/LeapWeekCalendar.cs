using System;

namespace WeirdCalendars {
    public abstract class LeapWeekCalendar : WeirdCalendar {

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            //Default to Monday start
            return (DayOfWeek)(GetDayOfYear(time) % 7);
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 371 : 364;
        }
    }
}