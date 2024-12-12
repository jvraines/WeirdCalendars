using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class M13W4D7PlusR2Calendar : FixedCalendar {

        public override string Author => "Claude Ziad El-Bayeh";
        public override Uri Reference => new Uri("https://www.researchgate.net/figure/Comparison-between-the-proposed-and-Gregorian-calendars_tbl2_342861980");

        protected override DateTime SyncDate => new DateTime(2025, 1, 1);
        protected override int SyncOffset => 0;

        public M13W4D7PlusR2Calendar() => Title = "M13W4D7+R2 Calendar";
        protected override int FirstMonth => 0;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 14;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month > 0 ? 28 : IsLeapYear(year) ? 2 : 1;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 0 && day == 2;
        }

        protected override int WeekdayNumber(DateTime time) {
            //Default to Sunday start with fixed months
            return GetDayOfMonth(time) % DaysInWeek;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 0;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 1 ? "Year-day" : "Leap-day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 1 ? "Yday" : "Lday";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            for (int i = 12; i > 0; i--) {
                m[i] = m[i - 1];
                ma[i] = ma[i - 1];
            }
            m[0] = "Zero";
            ma[0] = "Zer";
            SetNames(dtfi, m, ma);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int m = ToLocalDate(time).Month;
            if (m == 13) {
                fx.MonthFullName = "Undecember";
                fx.MonthShortName = "Und";
            }
            else {
                fx.MonthFullName = dtfi.MonthNames[m];
                fx.MonthShortName = dtfi.AbbreviatedMonthNames[m];
            }
            return fx;
        }
    }
}
