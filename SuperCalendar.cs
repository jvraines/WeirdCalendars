using System;
using System.Globalization;

namespace WeirdCalendars {
    public class SuperCalendar : FixedCalendar {
        
        public override string Author => "Arivumani";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Super_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month != 2 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 30;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day > 28;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 29 ? "Uranday" : "Nepday";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 29 ? "Uru" : "Nep";
        }

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % 7;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Anu", "Beo", "Cip", "Deq", "Ero", "Fis", "Gat", "Hup", "Ivo", "Jaw", "Kix", "Lay", "Mez" });
        }
    }
}
