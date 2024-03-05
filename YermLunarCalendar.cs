using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class YermLunarCalendar : WeirdCalendar {
        
        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/palmen/yerm1.htm");

        // "Year" will actually be sequential yerm (from the epoch) in this class.
        // GetYerm returns the number of the yerm within its cycle.

        protected override DateTime SyncDate => new DateTime(2022, 1, 3, 12, 0, 0);
        protected override int SyncOffset => -962;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunarCalendar;
        //public override DateTime MinSupportedDateTime => new DateTime(622, 5, 19);
        public override DateTime MinSupportedDateTime => new DateTime(3, 11, 11);

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> { 
            ("c", "Cycle number"),
            ("n", "Yerm number")
        };

        public int GetCycle(int year) {
            return (year - 1) / 52 + 1;
        }

        public int GetYerm(int year) {
            return (year - 1) % 52 + 1;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetYerm(year) % 3 == 0 ? 15 : 17;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 1 ? 30 : 29;
        }

        public override int GetDaysInYear(int year, int era) {
            return GetYerm(year) % 3 == 0 ? 443 : 502;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            CustomizeTimes(fx, time);
            var ymd = ToLocalDate(time);
            int c = GetCycle(ymd.Year);
            int ym = GetYerm(ymd.Year);
            fx.MonthFullName = $"Month {ymd.Month}";
            fx.LongDatePattern = $"'Night' d MMMM 'Yerm' {ym} 'Cycle' {c}";
            fx.ShortDatePattern = $"{ym}(MM(dd";
            fx.Format = format.ReplaceUnescaped("c", $"{c}").ReplaceUnescaped("n", $"{ym}");
            return fx;
        }
    }
}
