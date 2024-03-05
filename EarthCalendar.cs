using System;

namespace WeirdCalendars {
    public class EarthCalendar : FixedCalendar {

        protected override DateTime SyncDate => new DateTime(2018, 1, 1);
        protected override int SyncOffset => 0;

        public override string Author => "Brij Bhushan Vij";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Earth_Calendar");

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 5:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 4:
                case 7:
                case 9:
                case 11:
                    return 30;
                case 6:
                    return IsLeapYear(year) ? 31 : 30;
                default:
                    return 29;
            }
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            string n = base.IntercalaryDayName(year, month, day);
            return n == "Leap Day" ? n : "World Peace Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day) == "Leap" ? "Leap" : "Pace";
        }

        public override bool IsLeapYear(int year) {
            ValidateDateParams(year, 0);
            return year % 4 == 0 && year % 128 != 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 6 && day == 31;
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = GetDayOfYear(time);
            if (IsLeapYear(time.Year) && d > 91 * 2) d--;
            return d % 7;
        }

        internal override void CustomizeDTFI(System.Globalization.DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }
    }
}