using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class EdenianCalendar : WeirdCalendar {

        public override string Author => "Horatio Eden";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Edenian_calendar");

        protected override DateTime SyncDate => new DateTime(2017, 12, 18);
        protected override int SyncOffset => -2016;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.Unknown;
        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("c", "Compact format")
        };

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 15;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 60;
        }

        public override int GetEra(DateTime time) {
            ValidateDateTime(time);
            return ToLocalDate(time).Year / 4 + 1;
        }

        internal override int GetEra(int year) {
            ValidateDateParams(year, 0);
            return year / 4 + 1;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "AQ", "BQ", "CQ", "DQ", "", "", "", "", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (time < SyncDate) {
                string y = $"'Hour 0 - {(int)(SyncDate - time).TotalDays}'";
                fx.LongDatePattern = y;
                fx.ShortDatePattern = y;
                fx.Format = format.ReplaceUnescaped("c", y);
            }
            else {
                var ymd = ToLocalDate(time);
                int e = GetEra(ymd.Year);
                string y = $"{ymd.Year}E{e}";
                fx.LongDatePattern = FixDigits(fx.LongDatePattern, y, y);
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, y, y);
                fx.Format = format.ReplaceUnescaped("c", $"MMMM/{ymd.Day:00}/{(ymd.Year - 1) % 4 + 1}E{e}");
            }
            return fx;
        }
    }
}
