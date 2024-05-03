using System;
using System.Globalization;

namespace WeirdCalendars {
    public class ModernChristianCalendar : FixedCalendar {

        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Modern_Christian_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 12 && IsLeapYear(year) ? 30 : 29;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int l = year % 293;
            int s = l % 33;
            return s % 4 == 0 && s != 32 && l != 292;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 29;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 29 ? "Birth Day" : "Family Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 29 ? "Bir" : "Fam";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Pete", "Drew", "Jim", "Han", "Phil", "Bart", "Tom", "Levi", "Jake", "Thad", "Sim", "Jude", "Josh" }, null, new string[] { "Liday", "Skiday", "Waday", "Irday", "Nimday", "Manday", "Holday" });
        }
    }
}
