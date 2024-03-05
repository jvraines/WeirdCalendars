using System;

namespace WeirdCalendars {
    public class AristeanCalendar : WorldCalendar {
        public override string Author => "Aristeo Canlas Fernando";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Aristean_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return month == 6 ? "Leap Year Day" : "World Peace Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return month == 6 ? "Leap" : "Pace";
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = GetDayOfYear(time);
            int s = IsLeapYear(time.Year) && d > 91 * 2 ? 1 : 0;
            return (d - s) % 7;
        }
    }
}
