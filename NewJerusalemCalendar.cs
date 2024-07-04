using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;

namespace WeirdCalendars {
    public class NewJerusalemCalendar : FixedCalendar {

        public override string Author => "Russell J. Ewert";
        public override Uri Reference => new Uri("https://www.yumpu.com/en/document/read/54717208/the-new-jerusalem-calendar");

        protected override DateTime SyncDate => new DateTime(2024, 3, 20);
        protected override int SyncOffset => 0;

        public override DateTime MaxSupportedDateTime => VSOPLimit;

        protected override int FirstMonth => 0;
        
        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month > 0 ? 1 : 0;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month > 0) return (month + 1) % 3 == 0 ? 31 : 30;
            return IsLeapYear(year) ? 2 : 1;
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool leap)) {
                // Niagara Falls is UTC-4 after March 14 at the latest. UTC = JD + 0.5 - 4/24
                int equilux1 = (int)(Earth.SeasonStart(year, Earth.Season.March) + 1 / 3);
                int equilux2 = (int)(Earth.SeasonStart(year + 1, Earth.Season.March) + 1 / 3);
                leap = equilux2 - equilux1 > 365;
                LeapYears.Add(year, leap);
            }
            return leap;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 0 && day == 1;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 0;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 0 ? "New Year Day" : "Leap Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 0 ? "New" : "Leap";
        }

        protected override int WeekdayNumber(DateTime time) {
            int offset = IsLeapYear(ToLocalDate(time).Year) ? 2 : 1;
            return (GetDayOfYear(time) - 1 - offset) % DaysInWeek;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            string[] ma = new string[13];
            for (int i = 0; i < 12; i++) {
                int j = (i + 2) % 12;
                m[i] = dtfi.MonthNames[j];
                ma[i] = dtfi.AbbreviatedMonthNames[j];
            }
            m[12] = "";
            ma[12] = "";
            SetNames(dtfi, m, ma);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            //If this is an intercalary day, fix up day and month names
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.DayShortName = IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.MonthFullName = "Alpha Omega";
                fx.MonthShortName = "AO";
            }
            return fx;
        }
    }
}
