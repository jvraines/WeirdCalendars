using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;

namespace WeirdCalendars {
    public class InternationalEasterCalendar : WeirdCalendar {
        
        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/International_Easter_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 3, 31);
        protected override int SyncOffset => 0;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Tide")
        };

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetWeeksInYear(year) > 51;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return IsLeapYear(year) && month == 10;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 10 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 11 : 10;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month < 10) return 35;
            switch (GetWeeksInYear(year)) {
                case 50:
                case 55:
                    return 35;
                case 51:
                    return 42;
                case 54:
                    return month == 10 ? 28 : 35;
                default:
                    throw new ArgumentException();
            }
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetWeeksInYear(year) * 7;
        }

        private static Dictionary<int, int> YearWeeks = new Dictionary<int, int>();
        
        private int GetWeeksInYear(int year) {
            if(!YearWeeks.TryGetValue(year, out int w)) {
                DateTime thisEaster = Earth.Easter(year);
                DateTime nextEaster = Earth.Easter(year + 1);
                w = (int)(nextEaster - thisEaster).TotalDays / 7;
                YearWeeks.Add(year, w);
            }
            return w;
        }

        public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek) {
            return (GetDayOfYear(time) - 1) / 7 + 1;
        }

        private static string[] Tides = { "Eastide", "Trinitide", "Marytide", "Angeltide", "Adventide", "Christide", "Lentide"};

        public string GetTide(DateTime time) {
            int weeks = GetWeeksInYear(ToLocalDate(time).Year);
            int w = GetWeekOfYear(time, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday) - 1;
            int t = 0;
            switch (weeks) {
                case 50:
                    if (w < 35) t = w / 7;
                    else t = w < 43 ? 5 : 6;
                    break;
                case 51:
                    if (w < 28) t = w / 7;
                    else t = w < 44 ? (w - 28) / 8 + 4 : 6;
                    break;
                case 54:
                    if (w < 7) t = 0;
                    else if (w > 47) t = 6;
                    else t = (w - 7) / 8 + 1;
                    break;
                case 55:
                    if (w < 7) t = 0;
                    else t = (w - 7) / 8 + 1;
                    break;
            }
            return Tides[t];
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Eastermonth", "Whitmonth", "Solsticemonth", "Betweenmonth", "Equinoxmonth", "Hallowmonth", "Adventmonth", "Christmasmonth", "Epiphanymonth", "Passionmonth", "", "", ""});
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            if (IsLeapYear(ld.Year)) {
                if (ld.Month == 10) {
                    fx.MonthFullName = "Leapmonth";
                    fx.MonthShortName = "Lea";
                }
                else if (ld.Month == 11) {
                    fx.MonthFullName = dtfi.MonthNames[9];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[9];
                }
            }
            fx.Format = format.ReplaceUnescaped("n", $"'{GetTide(time)}'");
            return fx;
        }
    }
}
