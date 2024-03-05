using System;

namespace WeirdCalendars {
    public class DoubleLeapCalendar : WeirdCalendar {
  
        public override string Author => "Marcus W.";
        public override Uri Reference => new Uri("http://doubleleap.weebly.com/");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month < 12) return month % 3 == 0 ? 31 : 30;
            return IsLeapYear(year) ? 38 : IsDoubleLeapYear(year) ? 45 : 31;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 371 : IsDoubleLeapYear(year) ? 378 : 364;
        }

        // Leap year is divisible by 7, but not 28, unless divisible by 700 but not 2800.
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 7 == 0 && (year % 28 != 0 || year % 700 == 0 && year % 2800 != 0);
        }

        // Double leap year is divisible by 28, but not 700 unless divisible by 2800.
        public bool IsDoubleLeapYear (int year) {
            ValidateDateParams(year, 0);
            return year % 28 == 0 && (year % 700 != 0 || year % 2800 == 0);
        }
    }
}
