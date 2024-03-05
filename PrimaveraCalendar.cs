using System;
using System.Globalization;

namespace WeirdCalendars {
    public class PrimaveraCalendar : FixedCalendar {

        protected override DateTime SyncDate => new DateTime(2020, 12, 22);
        protected override int SyncOffset => 1;
        protected override int DaysInWeek => 6;

        public override string Author => "Primavera D. Hornblower";
        public override Uri Reference => new Uri("http://bosonline.com/primavera/");

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 1 || month % 3 == 0 || IsLeapYear(year) && month == 7 ? 31 : 30;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month == 1 || IsLeapYear(year) && month == 7 ? 0 : 1;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 7 && day == 0;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 1 && day == 0 || month % 3 == 0 && day == 31 || IsLeapDay(year, month, day);
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return day == 31 ? $"{new string[] { "East", "North", "West", "South"}[month / 3 - 1]} Day" : month == 1 ? "New Year's Day" : "Olympiad Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day).Substring(0, 3);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Capricorn", "Aquarius", "Pisces", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio", "Sagittarius", "" }, null, new string[] { "Sunday", "Moonday", "Airday", "Waterday", "Firesday", "Earthday", "" });
        }
    }
}
