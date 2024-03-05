using System;
using System.Globalization;

namespace WeirdCalendars {
    public class DoveCalendar : WeirdCalendar {
        
        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("http://www.the-light.com/cal/kp_Dove.html");

        protected override DateTime SyncDate => new DateTime(2013, 3, 29);
        protected override int SyncOffset => 0;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year % 33 & 1) == 0 ? 21 : 22;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year % 33 & 1) == 0 ? 357 : 374;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 17;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            var ymd = ToLocalDate(time);
            string month = $"{(char)(ymd.Month + 64)}";
            string day = $"{(char)(ymd.Day + 96)}";
            if (ymd.Month > 13) {
                fx.MonthFullName = month;
                fx.MonthShortName = month;
            }
            if ("DfF".Contains(format)) fx.LongDatePattern = $"dddd, '{month}{day}', yyyy";
            if ("dgG".Contains(format)) fx.ShortDatePattern = $"'{month}{day}'/yyyy";
            fx.Format = FixDigits(format, null, null, month, month, day, day);
            return fx;
        }
    }
}
