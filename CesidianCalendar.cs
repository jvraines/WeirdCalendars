using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class CesidianCalendar : FixedCalendar {

        public override string Author => "Cesidio Tallini";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Cesidian_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 31, 23, 0, 0);
        protected override int SyncOffset => 1;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("c", "Compact format")
        };

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 14;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 6:
                    return IsLeapYear(year) ? 27 : 26;
                case 14:
                    return 27;
                default:
                    return 26;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day == 27;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 27;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return month == 6 ? "Marsday" : "Earthday";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return month == 6 ? "Γ" : "Τ";
        }

        public override int GetDayOfYear(DateTime time) {
            ValidateDateTime(time);
            var ymd = ToLocalDate(time);
            return (ymd.Month - 1) * 26 + (ymd.Month > 6 && IsLeapYear(ymd.Year) ? 1 : 0) + ymd.Day;
        }

        public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek) {
            ValidateDateTime(time);
            int doy = GetDayOfYear(time);
            var ymd = ToLocalDate(time);
            if (ymd.Month > 6 && IsLeapYear(ymd.Year)) doy--;
            if (doy == 365) doy--;
            return (doy - 1) / 7 + 1;
        }

        protected override int WeekdayNumber(DateTime time) {
            return (GetDayOfYear(time) - 1) % 7;
        }

        public string GetCyberterraMeanTimeFormat(DateTime time) {
            int w = GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - 1;
            string week = $"{(char)(w % 26 + 65)}{w / 26 + 1}";
            var ymd = ToLocalDate(time);
            string dow = ymd.Day == 27 ? IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day) : DayAbbr[(int)GetDayOfWeek(time)];
            int tod = (int)(ymd.TimeOfDay.TotalHours / 24 * 1000);
            return $"{week}{dow}{ymd.Day:00}{(char)(ymd.Month + 64)}{ymd.Year:0000}•{tod:000}";
        }

        private static string[] DayAbbr = { "Φ", "Χ", "Δ", "Ψ", "Ρ", "Μ", "Π" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Archimedes", "Beethoven", "Columbus", "Dalí", "Edison", "Fleming", "Gandhi", "Hokusai", "Isaiah", "Jung", "Kurosawa", "Lagrange", "Montessori" }, new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" }, new string[] { "Jeuday", "Saturnday", "Uranday", "Neptunday", "Plutoday", "Mercuday", "Venusday" }, DayAbbr);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (ToLocalDate(time).Month == 14) {
                fx.MonthFullName = "Nureyev";
                fx.MonthShortName = "N";
            }
            fx.Format = format.ReplaceUnescaped("c", $"'{GetCyberterraMeanTimeFormat(time)}'");
            return fx;
        }
    }
}
