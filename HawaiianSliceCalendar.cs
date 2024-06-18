using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class HawaiianSliceCalendar : WeirdCalendar {
 
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Hawaiian_Slice_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("S", "Month style")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int d = month % 3 == 0 ? 31 : 30;
            if (StartsWithSplitWeek(year)) d += month < 4 ? 1 : 0;
            else if (StartsWithSplitWeek(year + 1)) d += month == 10 || month == 12 ? 1 : month == 11 ? 2 : 0;
            return d;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return StartsWithSplitWeek(year) ? 367 : StartsWithSplitWeek(year + 1) ? 368 : 364;
        }

        public string GetMonthStyle(DateTime time) {
            if (month > 9 && StartsWithSplitWeek(year + 1)) {
                return month == 11 ? "Jalapeño" : "Tomato";
            }
            else if (month <= 3 && StartsWithSplitWeek(year) {
                return month == 1 ? "Ham" : month == 2 ? "Tomato" : "Jalapeño";
            }
            else {
                switch (month % 3) {
                    case 1:
                        return "Pineapple";
                    case 2:
                        return "Ham";
                    default:
                        return "Tomato";
                }
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return StartsWithSplitWeek(year) || StartsWithSplitWeek(year + 1);
        }

        private bool StartsWithSplitWeek(int year) => year % 896 % 62 % 17 % 6 == 4;

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            fx.Format = format.ReplaceUnescaped("S", $"'{GetMonthStyle(time)}'");
            return fx;
        }
    }
}
