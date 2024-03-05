using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class LiberaliaLunarCalendar : LiberaliaCalendar {

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunarCalendar;

        protected override DateTime SyncDate => new DateTime(2020, 8, 17);
        protected override int SyncOffset => -1900;

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/ltc/ltc.htm");

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
                ("c", "Cycle"),
                ("n", "Triday")
            };

        /// <summary>
        /// Gets the cycle of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer representing the cycle number.</returns>
        public int GetCycle(DateTime time) {
            ValidateDateTime(time);
            return base.ToLocalDate(time).Year / 384;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 6:
                    return 27;
                case 12:
                    return IsLeapYear(year) ? 30 : 27;
                default:
                    return 30;
            }
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 357 : 354;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year - 2) % 8 == 0 && (year != 2);
        }

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            ValidateDateTime(time);
            var ymd = base.ToLocalDate(time);
            return (ymd.Year % 384, ymd.Month, ymd.Day, ymd.TimeOfDay);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int t = GetTriday(time);
            int d = (ToLocalDate(time).Day - 1) % 3 + 1;
            int cycle = GetCycle(time);
            fx.LongDatePattern = fx.LongDatePattern.Replace("yyyy", "yyy");
            fx.ShortDatePattern = cycle > 0 ? $"{cycle}-" : "" + $"yyy-MM-{t:D2}-{d}";
            fx.Format = format.ReplaceUnescaped("c", cycle.ToString()).ReplaceUnescaped("n", t.ToString());
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Armedon", "Nousanios", "Harmozel", "Phaionios", "Ainios", "Oraiel", "Mellephaneus", "Loios", "Davithe", "Mousanios", "Amethes", "Eleleth", "" }, null, new string[] { "Sophiesday", "Zoesday", "Norasday", "", "", "", "" });
        }
    }
}
