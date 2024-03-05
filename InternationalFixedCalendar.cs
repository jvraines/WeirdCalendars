using System;
using System.Globalization;

namespace WeirdCalendars {
    public class InternationalFixedCalendar : FixedCalendar {

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public override string Author => "Moses B. Cotsworth";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/International_Fixed_Calendar");

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13 || (IsLeapYear(year) && month == 6) ? 29 : 28;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return (month == 13 && day == 29) || IsLeapDay(year, month, day);
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            string n = base.IntercalaryDayName(year, month, day);
            return n == "Leap Day" ? n : "Year Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day) == "Leap" ? "Leap" : "Year";
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 6 && day == 29;
        }

        protected override int WeekdayNumber(DateTime time) {
            //Default to Sunday start
            int d = GetDayOfYear(time);
            int s = IsLeapYear(time.Year) && d > 91 * 2 ? 2 : 1;
            return (d - s) % 7;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            for (int i = 12; i > 6; i--) {
                m[i] = m[i - 1];
            }
            m[6] = "Sol";
            SetNames(dtfi, m);
        }
    }
}