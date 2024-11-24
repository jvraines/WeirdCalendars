using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class TimeToComeCalendar : FixedCalendar {

        public override string Author => "Joakim";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/A_Calendar_for_Time_to_Come");

        protected override DateTime SyncDate => new DateTime(2023, 3, 21);
        protected override int SyncOffset => 10000;

        public override int DaysInWeek => 9;

        public new enum DayOfWeekWC {
            DayBlank = -1,
            Day1,
            Day2,
            Day3,
            Day4,
            Day5,
            Day6,
            Day7,
            Day8,
            Day9
        }

        public TimeToComeCalendar() => Title = "A Calendar for Time to Come";

        public new DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ymd = ToLocalDate(time);
            int w = IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day) ? -1 : WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 8;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 5:
                case 7:
                    return 46;
                case 8:
                    return IsLeapYear(year) ? 47 : 46;
                default:
                    return 45;
            }
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            return (month & 1) == 1 ? 0 : 1;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 47;
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int gYear = year - SyncOffset;
            if (!LeapYears.TryGetValue(gYear, out bool ly)) {
                int yearStart = (int)Math.Round(Earth.SeasonStart(gYear, Earth.Season.March) + 0.5);
                int yearEnd = (int)Math.Round(Earth.SeasonStart(gYear + 1, Earth.Season.March) + 0.5);
                ly = yearEnd - yearStart == 366;
                LeapYears.Add(gYear, ly);
            }
            return ly;
        }
        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0 || day > 45;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 0 ? "Intercalary Day" : "Transition Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 0 ? "Int" : "Trn";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            string[] ma = new string[13];
            for (int i = 0; i < 13; i++) {
                m[i] = $"Octant-{i + 1}";
                ma[i] = $"O{i + 1}";
            }
            string[] w = new string[7];
            string[] wa = new string[7];
            for (int i = 0; i < 7; i++) {
                w[i] = $"Day-{i + 1}";
                wa[i] = $"D{i + 1}";
            }
            SetNames(dtfi, m, ma, w, wa);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            //If this is an intercalary day, fix up day name
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.DayShortName = IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day);
            }
            else {
                int w = WeekdayNumber(time);
                if (w > 6) {
                    fx.DayFullName = $"Day-{w + 1}";
                    fx.DayShortName = $"D{w + 1}";
                }
            }
            return fx;
        }
    }
}
