using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class WhisperingRealmsCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Whispering_Realms_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("p", "Pattern")
        };

        //Using Sunday = 0 and 0-based year numbers

        private int StartDay(int year) => ((int)Math.Ceiling((year + 1684) % 1792 % 124 % 17 * 1.25) + 1) % 7;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                    return 29 + (int)Math.Ceiling((10 - StartDay(year)) % 7 / 2d);
                case 2:
                    return 29 + (10 - StartDay(year)) % 7 / 2;
                case 11:
                    int s = StartDay(year);
                    return s == 2 && IsLeapYear(year) ? 32 : 29 + (s + 3) % 7 / 2;
                case 12:
                    s = StartDay(year);
                    int n = 29 + (int)Math.Ceiling((s + 3) % 7 / 2d);
                    return IsLeapYear(year) && (s == 1 || s == 4 || s == 6) ? n + 1 : n;
                default:
                    return month % 3 == 0 ? 31 : 30;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            year = (year + 1684) % 1792 % 124 % 17;
            return year == 0 || year == 4 || year == 8 || year == 12;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            int s = StartDay(year);
            return month == 11 && day == 32 && s == 2 ||
                    month == 12 &&
                        (day == 30 && s == 4 || day == 31 && s == 6 || day == 32 && s == 1);
        }

        private static string[] Patterns = { "Bowenia", "Cycas", "Dioon", "Encephalartos", "Namele", "Stangeria", "Zamia" };

        public string GetPattern(DateTime time) {
            var ymd = ToLocalDate(time);
            int s = StartDay(ymd.Year);
            if (IsLeapDay(ymd.Year, ymd.Month, ymd.Day)) s = ++s % 7;
            return Patterns[s];
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            base.CustomizeDTFI(dtfi);
            SetNames(dtfi, new string[] { "Gingko", "Bryum", "March", "Pine", "Magnoli", "Gnetum", "Lycoph", "Anthoce", "Cycad", "Fern", "Chara", "Chlorophyt", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("p")) fx.Format = format.ReplaceUnescaped("p", $"'{GetPattern(time)}'");
            return fx;
        }
    }
}
