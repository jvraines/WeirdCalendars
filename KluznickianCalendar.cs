using System;
using System.Globalization;

namespace WeirdCalendars {
    public class KluznickianCalendar : FixedCalendar {
        
        public override string Author => "Nick Kluznick";
        public override Uri Reference => new Uri("https://web.archive.org/web/20190611180202/http://www.kluznick.com/Calendar.html");

        protected override DateTime SyncDate => new DateTime(2023, 12, 21);
        protected override int SyncOffset => 1;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && year % 128 != 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 30;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day);
            return day > 28;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 29 ? "Lastday" : "Leapday";
        }

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % DaysInWeek;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            for (int i = 12; i > 6; i--) m[i] = m[i - 1];
            m[6] = "Aten";
            SetNames(dtfi, m);
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }
    }
}
