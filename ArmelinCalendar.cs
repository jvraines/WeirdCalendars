using System;

namespace WeirdCalendars {
    public class ArmelinCalendar : InvariableCalendar {
        
        public override string Author => "Gustave Armelin";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/Armelin%27s_calendar");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month == 1 ? 0 : 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month % 3 == 0 ? 31 : 30) + (month == 1 || IsLeapYear(year) && month == 12 ? 1 : 0);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0 || day == 32;
        }

        protected override int WeekdayNumber(DateTime time) {
            return (GetDayOfYear(time) - 1) % 7;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 32;
        }
    }
}
