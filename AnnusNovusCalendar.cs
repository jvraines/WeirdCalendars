using System;
using System.Globalization;

namespace WeirdCalendars {
    public class AnnusNovusCalendar : FixedCalendar {
        
        public override string Author => "Peter Meyer";
        public override Uri Reference =>  new Uri("https://www.atlantium.org/calendaran.html");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 8519;

        public new enum DayOfWeekWC {
            Blank = -1,
            Primidi,
            Secundi,
            Tertidi,
            Quartidi,
            Quintidi
        }

        public override int DaysInWeek => 5;

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 11 && day == 1;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return "Intercalarius";
        }

        protected override int WeekdayNumber(DateTime time) {
            return (GetDayOfYear(time) - 1) % DaysInWeek;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 11 ? (month & 1) == 1 ? 36 : 37 : 1;
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 11 : 10;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 11 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 1 && month == 11 && IsLeapYear(year);
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 11 && IsLeapYear(year);
        }

        public override bool IsLeapYear(int year, int era) {
            return base.IsLeapYear(year + 281, era);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Primus", "Secundus", "Tertius", "Quartus", "Quintus", "Sextus", "Septimus", "Octavus", "Nonus", "Decimus", "Intercalarius", "", "" }, null, new string[] { "Primidi", "Secundi", "Tertidi", "Quartidi", "Quintidi", "", "" });
        }
    }
}
