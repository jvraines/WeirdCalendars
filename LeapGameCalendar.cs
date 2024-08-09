using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class LeapGameCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Leap_Game_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => -1982;

        public override DateTime MinSupportedDateTime => new DateTime(1982, 1, 1);

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("n", "Feast")
        };

        public override string SpecialDay(DateTime time) => GetFeast(time);

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month != 2 || IsLeapYear(year) ? 30 + (month - 1 & 1) : 30;
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>() { { 0, true } };
        private static Dictionary<int, int> Quillies = new Dictionary<int, int>();

        private int ClockStartSteady(int year) => year > 0 ? 34 + (year - 1) * 31 : 3;
        private int ClockStartLeapfrog(int year) => year * 31;

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool isLeap)) {
                int clockStart = ClockStartSteady(year - 1);
                int clockEnd = (clockStart + 31) % 128;
                clockStart %= 128;
                isLeap = clockEnd < clockStart;
                if (isLeap) Quillies.Add(year - 1, 128 - clockStart);
                LeapYears.Add(year, isLeap);
            }
            return isLeap;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 2 && day == 31;
        }

        public bool IsQuillyDay(int year, int day) {
            ValidateDateParams(year, 12, day, 0);
            if (!IsLeapYear(year + 1)) return false;
            return Quillies[year] == day;
        }

        private static string[] Feasts = { "Sky", "Sun", "Soil", "Water" };
        private static int[] Leaps = { 31, 29, 25, 17, 1 };

        public string GetFeast(DateTime time) {
            ValidateDateTime(time);
            string f = "";
            var ymd = ToLocalDate(time);
            if (ymd.Month == 12) {
                if (IsQuillyDay(ymd.Year, ymd.Day)) f = "Quilly Day";
                int clock = ClockStartSteady(ymd.Year) + ymd.Day;
                if (clock % 16 == 0) f = $"{f}{Slash()}Steady {FindFeast()}";
                int leap = Array.IndexOf(Leaps, ymd.Day);
                if (leap > -1) {
                    clock = ClockStartLeapfrog(ymd.Year);
                    int prevClock = 0;
                    for (int n = 4; n >= leap; n--) {
                        prevClock = clock;
                        clock += (int)Math.Pow(2, n);
                    }
                    if (clock % 16 < clock - prevClock) f = $"{f}{Slash()}Leapfrog {FindFeast()}";
                }
                string Slash() => f.Length > 0 ? "/" : "";
                string FindFeast() => Feasts[clock % 128 / 16 % 4];
            }         
            return f.Length > 0 ? f : NoSpecialDay;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            base.CustomizeDTFI(dtfi);
            SetNames(dtfi, new string[] { "Gingko", "Bryum", "March", "Pine", "Magnoli", "Gnetum", "Lycoph", "Anthoce", "Cycad", "Fern", "Chara", "Chlorophyt", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            fx.Format = format.ReplaceUnescaped("n", $"'{GetFeast(time)}'");
            return fx;
        }
    }
}
