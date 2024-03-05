using System;
using System.Globalization;

namespace WeirdCalendars {
    public class WorldSeasonCalendar : FixedCalendar {

        protected override DateTime SyncDate => new DateTime(2020, 12, 21);
        protected override int SyncOffset => 1;

        public override string Author => "Isaac Asimov";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/World_Season_Calendar");

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 4 || (IsLeapYear(year) && month == 2) ? 92 : 91;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 2 && day == 92;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return (month == 4 && day == 92) || IsLeapDay(year, month, day);
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return base.IntercalaryDayName(year, month, day) == "Leap Day" ? "Leap Day" : "Year Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day) == "Leap Day" ? "Leap" : "Year";
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            fx.ShortDatePattern = "MMMM-d-yyyy";
            fx.LongDatePattern = $"'Season' MMMM, 'Day' d, yyyy";
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "A", "B", "C", "D", "", "", "", "", "", "", "", "", "" });
        }
    }
}