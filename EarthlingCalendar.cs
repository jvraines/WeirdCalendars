using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class EarthlingCalendar : FixedCalendar {
        public override string Author => "Miles Bradley Huff";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Aethodian_calendars");

        // Author's example under Format is off by +1 day because he didn't account for Year 0 being a leap year.

        protected override DateTime SyncDate => new DateTime(2025, 1, 1, 11, 0, 0);
        protected override int SyncOffset => -535;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override int DaysInWeek => 6;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("c", "Compact format")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 11:
                    return 32;
                case 5:
                    return IsLeapYear(year) ? 32 : 31;
                case 2:
                case 8:
                    return 31;
                default:
                    return 30;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 5 && day == 32;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day > 30;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return $"Celebration {day - 30}";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return $"C{day - 30}";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] {"Month 1", "Month 2", "Month 3", "Month 4", "Month 5", "Month 6", "Month 7", "Month 8", "Month 9", "Month ↊", "Month ↋", "Month 10", ""}, new string[] { "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9", "M↊", "M↋", "M10", "" }, new string[] { "Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6", "" }, new string[] { "D1", "D2", "D3", "D4", "D5", "D6", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            var ld = ToLocalDate(time);
            string y = ld.Year.Dozenal();
            int inc = ld.Year < 0 ? 1 : 0;
            string yy = FixNegativeYears(y.PadLeft(2 + inc, '0'));
            string yyyy = FixNegativeYears(y.PadLeft(4 + inc, '0'));
            string m = ld.Month.Dozenal();
            string mm = m.PadLeft(2, '0');
            string d = ld.Day.Dozenal();
            string dd = d.PadLeft(2, '0');
            string t = (ld.TimeOfDay.TotalHours / 6).Dozenal(2);
            fx.LongTimePattern = $"{t}Ͳ";
            fx.ShortTimePattern = fx.LongTimePattern;
            fx.LongDatePattern = FixDigits(fx.LongDatePattern, yyyy, yy, mm, m, dd, d);
            fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, yyyy, yy, mm, m, dd, d);
            if (format == "G" || format == "g") fx.DateAndTimeSeparator = ":";
            else fx.Format = FixDigits(format, yyyy, yy, mm, m, dd, d).ReplaceUnescaped("c", $"{y.PadLeft(3, '0')}:{mm}:{dd}:{fx.LongTimePattern}");
            return fx;
        }
    }
}
