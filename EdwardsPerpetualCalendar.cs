using System;

namespace WeirdCalendars {
    public class EdwardsPerpetualCalendar : WorldCalendar {

        public override string Author => "Willard Eldridge Edwards";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Edwards_perpetual_calendar");

        protected override DateTime SyncDate => new DateTime(2018, 1, 1);

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 3 != 0 ? 30 : month == 12 || IsLeapYear(year) && month == 6 ? 32 : 31;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 32;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day == 32 && IsLeapYear(year);
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = GetDayOfYear(time);
            int s = IsLeapYear(time.Year) && d > 91 * 2 ? 1 : 0;
            return (d - s) % 7;
        }
    }
}
