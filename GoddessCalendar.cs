using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class GoddessCalendar : WeirdCalendar {

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.fractal-timewave.com/mmgc/mmgc.htm");

        protected override DateTime SyncDate => new DateTime(2022, 6, 29);
        protected override int SyncOffset => -1906;
        public override DateTime MinSupportedDateTime => new DateTime(1901, 8, 14);

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("c", "Cycle")
        };

        public GoddessCalendar() => Title = "Goddess Lunar Calendar";

        /// <summary>
        /// Gets the cycle number of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer representing the cycle number.</returns>
        public int GetCycle(DateTime time) {
            ValidateDateTime(time);
            return (int)Math.Floor(ToLocalDate(time).Year / 470d) + 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 0 || month == 13 && IsLeapYear(year) ? 29 : 30;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 383 : 384;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 29 && month == 13 && IsLeapYear(year);
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 10 == 0 || year % 235 == 0;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int cycle = GetCycle(time);
            fx.ShortDatePattern = $"{cycle}-yyy-MM-dd";
            fx.LongDatePattern = dtfi.LongDatePattern.ReplaceUnescaped("yyyy", $"{cycle}-yyy");
            FixNegativeYears(fx, time);
            fx.Format = fx.Format.ReplaceUnescaped("c", $"%{cycle}");
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Athena", "Brigid", "Cerridwen", "Diana", "Epona", "Freya", "Gaea", "Hathor", "Innana", "Juno", "Kore", "Lilith", "Maria" });
        }
    }
}