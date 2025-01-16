using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class GaralianCalendar : WeirdCalendar {
       
        public override string Author => "Ferr Garal";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Garalian_calendar");

        protected override DateTime SyncDate => new DateTime(2025, 3, 20);
        protected override int SyncOffset => -2021;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Holiday")
        };

        public override string SpecialDay(DateTime time) => GetHoliday(time);

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 16 : 15;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 5:
                case 7:
                    return 31;
                case 2:
                case 6:
                case 10:
                case 14:
                    return 30;
                case 4:
                case 8:
                    return 2;
                case 9:
                case 11:
                case 13:
                case 15:
                    return 29;
                default:
                    return 1;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return base.IsLeapYear(year - SyncOffset, 0);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 16;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int d = GetDayOfMonth(time);
            return d == 31 ? DayOfWeek.Saturday : (DayOfWeek)((d - 1) % 6);
        }

        public string GetHoliday(DateTime time) {
            string h = NoSpecialDay;
            var (_, month, day, _) = ToLocalDate(time);
            if (month == 1 && day == 1) h = "New Year's Day";
            else if (month == 2 && day == 12) h = "Workers' Day";
            else if (month == 14 && day == 15) h = "Independence Day";
            else if (month == 15 && day == 3) h = "Ancestors Day";
            return h;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Early Spring", "Mid Spring", "Late Spring", "Summer Solstice", "Early Summer", "Mid Summer", "Late Summer", "Autumnal Equinox", "Early Autumn", "Mid Autumn", "Late Autumn", "Winter Solstice", "Early Winter" }, new string[] { "ESp", "MSp", "LSp", "SSo", "ESu", "MSu", "LSu", "AEq", "EAu", "MAu", "LAu", "WSo", "EWi" }, new string[] { "primidi", "duodi", "tridi", "quartidi", "quintidi", "sextidi", "septidi" });
        }

        private static string[] ExtraMonth = { "Mid Winter", "Late Winter", "Leap Day" };
        private static string[] ExtraMonthAbbr = { "MWi", "LWi", "Leap" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int m = ToLocalDate(time).Month - 14;
            if (m >= 0) {
                fx.MonthFullName = ExtraMonth[m];
                fx.MonthShortName = ExtraMonthAbbr[m];
            }
            fx.Format = format.ReplaceUnescaped("n", $"\"{GetHoliday(time)}\"");
            FixNegativeYears(fx, time);
            return fx;
        }
    }
}
