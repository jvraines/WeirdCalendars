using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class SolilunarCalendar : WeirdCalendar {

        protected override DateTime SyncDate => new DateTime(2020, 3, 24);
        protected override int SyncOffset => 4146;

        public override string Author => "Peter Meyer and Karl Palmen";
        public override Uri Reference => new Uri("http://www.hermetic.ch/cal_stud/nlsc/nlsc.htm");

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("c", "Current cycle"),
            ("n", "Year of current cycle")
        };

        /// <summary>
        /// Gets the era of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer representing the era.</returns>
        public override int GetEra(DateTime time) {
            return GetCycle(time) / 114 ;
        }

        /// <summary>
        /// Gets the cycle of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer representing the cycle.</returns>
        public int GetCycle(DateTime time) {
            return (ToLocalDate(time).Year - 1) / 60;
        }

        /// <summary>
        /// Gets the sequential year of date within a cycle.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer representing the sequential year within a cycle.</returns>
        public int GetCycleYear(DateTime time) {
            return (ToLocalDate(time).Year - 1) % 60 + 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? (month & 1) == 1 ? 29 : 30 : IsLongMeton(year) ? 31 : 30;
        }

        private bool IsLongMeton(int year) {
            ValidateDateParams(year, 0);
            return year * 2519 / 6840 * 1328 % 2519 < 1328;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? IsLongMeton(year) ? 385 : 384 : 354;
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 13 : 12;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year * 2519 % 6840 < 2519;
        }

        public override DateTime ToDateTime(int cycle, int year, int month, int day, int hour, int minute, int second, int millisecond) {
            return base.ToDateTime(cycle * 60 + year, month, day, hour, minute, second, millisecond, 0);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int cycle = GetCycle(time);
            int shortYear = GetCycleYear(time);
            fx.ShortDatePattern = $"{cycle}-{shortYear}-MM-dd";
            fx.LongDatePattern = dtfi.LongDatePattern.ReplaceUnescaped("yyyy", $"{cycle}-{shortYear}");
            fx.Format = fx.Format.ReplaceUnescaped("c", $"%{cycle}").ReplaceUnescaped("n", $"{shortYear}");
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Aristarchus", "Bruno", "Copernicus", "Dee", "Eratosthenes", "Flamsteed", "Galileo", "Hypatia", "Ibrahim", "Julius", "Khayyam", "Lilius", "Meton" });
        }
    }
}
