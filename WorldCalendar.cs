using System;

namespace WeirdCalendars {
    public class WorldCalendar : FixedCalendar {

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public override string Author => "Elisabeth Achelis";
        public override Uri Reference => new Uri("https://web.archive.org/web/20200922210904/http://www.theworldcalendar.org/");

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 3 == 1 || month == 12 || month == 6 && IsLeapYear(year) ? 31 : 30;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day == 31 && IsLeapYear(year);
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return base.IntercalaryDayName(year, month, day) == "Leap Day" ? "Leapyear Day" : "Worldsday";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day) == "Leap" ? "Leap" : "Wrld";
        }

        protected override int WeekdayNumber(DateTime time) {
            //Default to Sunday start
            int d = GetDayOfYear(time);
            int s = IsLeapYear(time.Year) && d > 91 * 2 ? 2 : 1;
            return (d - s) % 7;
        }
    }
}