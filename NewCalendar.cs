using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class NewCalendar : FixedCalendar {
        
        public override string Author => "Tantek Çelik";
        public override Uri Reference => new Uri("http://tantek.pbworks.com/w/page/19402947/NewCalendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int DaysInWeek => 5;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> { ("I", "\"ISO\" format") };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 0 && (month < 12 || IsLeapYear(year)) ? 31 : 30;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 31 && month == 12;
        }

        public override DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            DayOfWeekWC w = base.GetDayOfWeekWC(time);
            return w == DayOfWeekWC.Blankday ? DayOfWeekWC.Sunday : w;
        }

        protected override int WeekdayNumber(DateTime time) {
            return (GetDayOfMonth(time) - 1) % DaysInWeek + 1;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 31;
        }

        private string SundayName, SundayAbbrName;

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return month == 12 ? $"Leap {SundayName}" : $"New {SundayName}";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return month == 12 ? $"L{SundayAbbrName}" : $"N{SundayAbbrName}";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string nw = "New";
            string nwa = "N";
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            for (int i = 0; i < 12; i++) {
                m[i] = $"{nw} {m[i]}";
                string abbr = $"{nwa}{ma[i].Substring(0,2)}";
                for (int j = 0; j < i; j++) {
                    if (ma[j] == abbr) abbr = $"{nwa}{ma[i].Substring(0, 1)}{ma[i].Substring(ma[i].Length - 1)}";
                }
                ma[i] = abbr;
            }
            string[] d = (string[])dtfi.DayNames.Clone();
            string[] da = (string[])dtfi.AbbreviatedDayNames.Clone();
            for (int i = 1; i < 6; i++) {
                int o = i > 3 ? 1 : 0;
                d[i] = $"{nw} {d[i + o]}";
                string cand = da[i + 0];
                string abbr = $"{nwa}{cand.Substring(0, 2)}";
                for (int j = 0; j < i; j++) {
                    if (da[j] == abbr) abbr = $"{nwa}{cand.Substring(0, 1)}{cand.Substring(cand.Length - 1)}";
                }
                da[i] = abbr;
            }
            SetNames(dtfi, m, ma, d, da);
            SundayName = d[0]; SundayAbbrName = da[0];
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("I")) {
                var ld = ToLocalDate(time);
                int bm = (ld.Month + 1) / 2;
                int dy = ld.Day + ((ld.Month & 1) == 0 ? 30 : 0);
                fx.Format = format.ReplaceUnescaped("I", $"yyyy-{bm}-{dy:D2}");
            }
            return fx;
        }
    }
}
