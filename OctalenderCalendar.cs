using System;
using System.Globalization;

namespace WeirdCalendars {
    public class OctalenderCalendar : LeapWeekCalendar {
        
        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Octalendar");

        protected override DateTime SyncDate => new DateTime(2021, 1, 1);
        protected override int SyncOffset => 0;

        public OctalenderCalendar() => Title = "Octalender";
        
        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return time.DayOfWeek;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 1 || month == 8 && IsLeapYear(year) ? 49 : 42;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 8;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Month 1", "Month 2", "Month 3", "Month 4", "Month 5", "Month 6", "Month 7", "Month 8", "", "", "", "", ""}, new string[] { "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int w = (ToLocalDate(time).Day - 1) / 7 + 1;
            int d = ((int)time.DayOfWeek + 6) % 7 + 1;
            fx.LongDatePattern = $"dddd, yyyy-M-{w}-{d}";
            return fx;
        }
    }
}
