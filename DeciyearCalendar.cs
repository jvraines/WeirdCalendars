using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class DeciyearCalendar : WeirdCalendar {
        
        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://web.archive.org/web/20240326203508/http://www.deciyear.com/");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public enum DayOfWeekWC {
            Primiday,
            Secunday,
            Tertday,
            Quartday,
            Quintday,
            Sixtday,
            Septday,
            Octday,
            Novday
        }

        public override int DaysInWeek => 9;
        protected override int FirstMonth => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> { ("c", "Compact format") };

        private int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % 9;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int w = WeekdayNumber(time);
            if (w > 6) throw BadWeekday;
            return (DayOfWeek)w;
        }

        public virtual DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            int w = WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 11;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 10 ? 36 : IsLeapYear(year) ? 6 : 5;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 1);
            return 0;
        }

        public double GetTime(DateTime time) => time.TimeOfDay.Ticks / (double)TimeSpan.TicksPerDay;

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Primilis", "Secundilis", "Tertilis", "Quartilis", "Quintilis", "Sixtilis", "Septilis", "Octilis", "Novilis", "Decilis", "Yearend", "", "" }, null, new string[] { "Primiday", "Secunday", "Tertday", "Quartday", "Quintday", "Sixtday", "Septday" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int w = WeekdayNumber(time);
            if (w > 6) {
                fx.DayFullName = ((DayOfWeekWC)w).ToString();
                fx.DayShortName = fx.DayFullName.Substring(0, 3);
            }
            var ld = ToLocalDate(time);
            fx.MonthFullName = dtfi.MonthNames[ld.Month];
            fx.MonthShortName = dtfi.MonthNames[ld.Month];
            string t = GetTime(time).ToString(".000");
            fx.LongTimePattern = t;
            fx.ShortTimePattern = t;
            fx.Format = fx.Format.ReplaceUnescaped("c", $"{ld.Year}.{(ld.Month == 10 ? "A" : ld.Month.ToString())}:{ld.Day}{t}");
            return fx;
        }
    }
}
