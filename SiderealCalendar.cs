using System;
using System.Globalization;

namespace WeirdCalendars {
    public class SiderealCalendar : WeirdCalendar {

        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Sidereal_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 19);
        protected override int SyncOffset => 1;

        private static int[] MonthDays = { 29, 30, 30, 30, 31, 31, 31, 31, 31, 31, 30, 30 };

        private int GetMonthDays(int year, int month) {
            int i = (year - 2019) / 2100 % 12; // month lengths correct as of year of authorship
            return MonthDays[(i + month - 1) % 12];
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int d = GetMonthDays(year, month);
            return month == 1 && IsLeapYear(year) ? d + 1 : d;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && year % 40 != 0 || year % 40 == 39 || year % 160 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == GetMonthDays(year, month) + 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Sagittarius", "Capricorn", "Aquarius", "Pisces", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio", "" });
        }
    }
}
