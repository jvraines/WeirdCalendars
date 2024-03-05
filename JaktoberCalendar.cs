using System;
using System.Globalization;

namespace WeirdCalendars
{
    public class JaktoberCalendar : FixedCalendar {
  
        public override string Author => "Jack Wagner";
        public override Uri Reference => new Uri("https://web.archive.org/web/20110512001214/http://jaktober.com/jaktober/");

        protected override DateTime SyncDate => new DateTime(2020, 12, 21);
        protected override int SyncOffset => -2011;

        protected override int FirstMonth => 0;

        public JaktoberCalendar() => Title = "Jaktober";
        
        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month == 0 ? 0 : 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month > 0 ? 28 : IsLeapYear(year) ? 2 : 1;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 14;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 0 && day == 1;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int r = year % 33;
            return r > 0 && r % 4 == 0;
        }

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % DaysInWeek;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 0;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 0 ? "Solstice Day" : "Leap Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 0 ? "Sol" : "Leap";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            string[] m = (string[])dtfi.MonthNames.Clone();
            m[12] = "Jaktober";
            string[] a = (string[])dtfi.AbbreviatedMonthNames.Clone();
            a[12] = "Jak";
            SetNames(dtfi, m, a);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (GetMonth(time) == 0) {
                fx.MonthFullName = "Solstice";
                fx.MonthShortName = "Sol";
            }
            return fx;
        }
    }
}
