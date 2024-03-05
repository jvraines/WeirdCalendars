using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class LiberaliaSolarCalendar : LiberaliaCalendar {
        
        protected override DateTime SyncDate => new DateTime(2015, 3, 20);
        protected override int SyncOffset => -1904;

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/ltc/ltc.htm");

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> { ("n", "Triday") };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 1 || month == 3 ? 90 : month == 4 && IsLeapYear(year) ? 90 : 93;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 363 : 366;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year + 1) % 4 == 0 || (year + 1) % 198 == 0;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int t = GetTriday(time);
            int d = (ToLocalDate(time).Day - 1) % 3 + 1;
            fx.ShortDatePattern = $"yyy-M-{t:D2}-{d}";
            fx.Format = format.ReplaceUnescaped("n", t.ToString());
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Kamaliel", "Gabriel", "Samio", "Abrasax", "", "", "", "", "", "", "", "", "" }, null, new string[] { "Sophiesday", "Zoesday", "Norasday", "", "", "", "" });
        }
    }
}
