using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;
using static System.Math;

namespace WeirdCalendars {
    public class MayaCalendar : WeirdCalendar {

        public override string Author => "Traditional";
        public override Uri Reference => new Uri("https://www.eecis.udel.edu/~mills/maya.html");

        protected override DateTime SyncDate => new DateTime(2022, 3, 31);
        protected override int SyncOffset => 3116;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.Unknown;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> {
            ("c", "Calendar round"),
            ("l", "Long count")
        };

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return 0;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 19 ? 20 : 5;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 365;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 19;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        private int DayCount(DateTime time) => (int)(time.JulianDay() + 0.5) - 584283;

        /// <summary>
        /// Gets the long count of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>A string representing the long count value.</returns>
        public string GetLongCount(DateTime time) {
            const double baktunLen = 144000;
            const double katunLen = 7200;
            const double tunLen = 360;
            const double uinalLen = 20;

            int days = DayCount(time);
            double baktun = Floor(days / baktunLen) % 20;
            double katun = Floor(days % baktunLen / katunLen);
            double tun = Floor(days % katunLen / tunLen);
            double uinal = Floor(days % tunLen / uinalLen);
            double kin = Floor(days % uinalLen);

            return $"{baktun}.{katun}.{tun}.{uinal}.{kin}";
        }

        private static string[] TzolkinName = { "Imix", "Ik'", "Ak'ba'i", "K'an", "Chikchan", "Kimi", "Manik'", "Lamat", "Muluk", "Ok", "Chuwen", "Eb'", "B'en", "Ix", "Men", "Kib'", "Kab'an", "Etz'nab'", "Kawak", "Ajaw" };

        private static string[] MonthName = { "Pop", "Wo'", "Sip", "Sotz'", "Sek", "Xul", "Yaxk'in", "Mol", "Ch'en", "Yax", "Sak", "Keh", "Mak", "K'ank'in", "Muwan", "Pax", "K'ayab", "Kumk'u", "Wayeb'" };

        /// <summary>
        /// Gets the calendar round value of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>A string containing the Tzolkin and vague calendar values.</returns>
        public string GetCalendarRound(DateTime time) {
            int days = DayCount(time) + 159;
            string tzolkin = $"{days % 13 + 1} {TzolkinName[days % 20]}";
            var ymd = ToLocalDate(time);
            string vague = $"{ymd.Day} {MonthName[ymd.Month - 1]}";
            return $"{tzolkin} {vague}";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            for (int i = 0; i < 13; i++) m[i] = MonthName[i];
            SetNames(dtfi, m);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            string month = MonthName[ToLocalDate(time).Month - 1];
            fx.LongDatePattern = FixMonth(fx.LongDatePattern);
            fx.ShortDatePattern = FixMonth(fx.ShortDatePattern);
            if (format.FoundUnescaped("l")) fx.Format = FixMonth(format).ReplaceUnescaped("l", $"\"{GetLongCount(time)}\"");
            if (format.FoundUnescaped("c")) fx.Format = fx.Format.ReplaceUnescaped("c", $"\"{GetCalendarRound(time)}\"");
            return fx;

            string FixMonth(string f) {
                f = f.ReplaceUnescaped("MMMM", $"\"{month}\"").ReplaceUnescaped("MMM", $"\"{month.Substring(0, 3)}\"");
                return f;
            }
        }
    }
}
