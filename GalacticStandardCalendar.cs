using System;
using System.Globalization;

namespace WeirdCalendars {
    public class GalacticStandardCalendar : FixedCalendar {
 
        public override string Author => "George Lucas et al.";
        public override Uri Reference => new Uri("https://www.starwarsrp.net/threads/resourse-galactic-standard-calendar.49596/");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        //As there is no provision for leap years, adding intercalary "Boonta Eve" between Fifth and Sixth.

        public new enum DayOfWeekWC {
            Blankday = -1,
            Primeday,
            Centaxday,
            Taungsday,
            Zhellday,
            Benduday
        }

        protected override int DaysInWeek => 5;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 8:
                case 12:
                    return 5;
                case 6:
                    return IsLeapYear(year) ? 36 : 35;
                default:
                    return 35;
            }
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day);
            return day == 36;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) => "Boonta Eve";

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) => "Boo";

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "New Year's Fete", "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Festival of Life", "Seventh", "Eighth", "Ninth", "Festival of Stars", "Tenth" }, new string[] {"New", "1st", "2nd", "3rd", "4th", "5th", "6th", "Life", "7th", "8th", "9th", "Star", "10th" }, new string[] { "Primeday", "Centaxday", "Taungsday", "Zhellday", "Benduday", "", "" });
        }
    }
}
