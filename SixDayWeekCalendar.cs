using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;

namespace WeirdCalendars {
    public class SixDayWeekCalendar : FixedCalendar {
        
        public override string Author => "Tibi86";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/6-Day_Week_Solar_Calendar_with_common_Muslim/Christian_weekend");

        protected override DateTime SyncDate => new DateTime(2021, 3, 21, 6, 0, 0);
        protected override int SyncOffset => 0;
        public override DateTime MaxSupportedDateTime => VSOPLimit; // Limit of VSOP87 accuracy

        protected override int DaysInWeek => 6;

        public SixDayWeekCalendar() => Title = "6-Day Week Solar Calendar";

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month > 6 ? 30 : month > 1 || IsLeapYear(year) ? 31 : 30;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 1 && day == 31 && IsLeapYear(year);
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool ly)) {
                int yearStart = (int)(Earth.SeasonStart(year, Earth.Season.March) + 0.5);
                int yearEnd = (int)(Earth.SeasonStart(year + 1, Earth.Season.March) + 0.5);
                ly = yearEnd - yearStart == 366;
                LeapYears.Add(year, ly);
            }
            return ly;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 31 && (month < 7 && month > 1 || IsLeapDay(year, month, day));
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return IsLeapDay(year, month, day) ? "Leap Day" : "Extra Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return IsLeapDay(year, month, day) ? "Leap" : "Xtra";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            for (int i = 0; i < 9; i++) m[i] = dtfi.MonthNames[i + 3];
            for (int i = 9; i < 12; i++) m[i] = dtfi.MonthNames[i - 9];
            m[12] = "";
            SetNames(dtfi, m);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
