using System;

namespace WeirdCalendars {
    public class BonavianCivilCalendar : LeapWeekCalendar {
        
        public override string Author => "Chris Carrier";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Bonavian_Civil_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 12, 24);
        protected override int SyncOffset => 4714;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfMonth(time) - 1) % 7);
        }
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month - 1) % 3 == 0 || month == 12 && IsLeapYear(year) ? 35 : 28;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            switch (year % 28) {
                case 0:
                    return year % 896 != 0;
                case 5:
                case 11:
                case 16:
                case 22:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day > 28;
        }
    }
}
