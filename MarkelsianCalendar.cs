using System;
using System.Globalization;

namespace WeirdCalendars {
    public class MarkelsianCalendar : FixedCalendar {

        public override string Author => "Paul Markel";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Markelsian_calendar");

        protected override DateTime SyncDate => new DateTime(2022, 12, 18);
        protected override int SyncOffset => 1;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 30 && month == 13 && IsLeapYear(year);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return IsLeapDay(year, month, day);
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            if (IsIntercalaryDay(year, month, day)) return "Julius";
            throw new ArgumentOutOfRangeException("Not an intercalary day.");
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            if (IsIntercalaryDay(year, month, day)) return "Jul";
            throw new ArgumentOutOfRangeException("Not an intercalary day.");
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Sagittarius", "Capricornus", "Aquarius", "Pisces", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio", "Ophiucus" });
        }
    }
}