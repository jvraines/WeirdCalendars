using System;

namespace WeirdCalendars {
    public class Symmetry454Calendar : LeapWeekCalendar {

        protected override DateTime SyncDate => new DateTime(2018, 1, 1);
        protected override int SyncOffset => 0;

        public override string Author => "Irv Bromberg";
        public override Uri Reference => new Uri("http://individual.utoronto.ca/kalendis/symmetry.htm");

        public Symmetry454Calendar() => Title = "Symmetry454";

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 3 == 2 || (month == 12 && IsLeapYear(year)) ? 35 : 28;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 12 && day > 28;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return(52 * year + 146) % 293 < 52;
        }

        internal override void CustomizeDTFI(System.Globalization.DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }

    }
}