using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WeirdCalendars {
    public class DeadSeaScrollCalendar : LeapWeekCalendar {

        public override string Author => "Ken Johnson";
        public override Uri Reference => new Uri("https://dsscalendar.org/o/outer/");

        protected override DateTime SyncDate => new DateTime(2024, 3, 18, 22, 0, 0);
        protected override int SyncOffset => 3925;

        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("n", "Holy day"),
            ("j", "Jubilee date")
        };

        public override int GetMonthsInYear(int year, int era) {
            // Wheel graphic shows leap week as part of Month 12, but online calendar displays it as separate month with enumerated days.
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            // Day 0 is the Tekufah
            return month % 3 == 1 && month < 13 ? 0 : 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13 ? 7 : month % 3 == 1 ? 31 : 30;
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>() { { 5949, false } };

        public override bool IsLeapYear(int year, int era) {
            //According to the author, "[The online calendars] add a leap week when [the equinox] is no longer in the same week." This is not the same as "within three days," a comment found elsewhere.
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool leap)) {
                int increment;
                int testYear;
                if (year - SyncOffset < SyncDate.Year) {
                    testYear = LeapYears.Keys.Min();
                    increment = -1;
                }
                else {
                    testYear = LeapYears.Keys.Max();
                    increment = 1;
                }
                double testDate = ToDateTime(testYear, 1, 0, 2, 0, 0, 0).JulianDay();
                if (increment == 1 && LeapYears[testYear]) testDate += 7;
                while (testYear != year) {
                    testDate += increment * 364;
                    testYear += increment;
                    double equinox = Earth.SeasonStart(testYear - SyncOffset, Earth.Season.March).ToLastUTMidnight();
                    leap = equinox - equinox.DayOfWeek() != testDate - testDate.DayOfWeek();
                    LeapYears.Add(testYear, leap);
                    if (leap) testDate += increment * 7;
                }
            }
            return leap;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 13;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) => (DayOfWeek)((GetDayOfYear(time) + 1) % 7);

        public string GetHolyDay(DateTime time) {
            string h = NoSpecialDay;
            var (_, month, day, _) = ToLocalDate(time);
            switch (month) {
                case 1:
                    if (day == 14) h = "Passover";
                    else if (day > 14 && day < 22) h = "Unleavened Bread";
                    else if (day == 26) h = "First Fruits";
                break;
                case 3:
                    if (day == 15) h = "Pentecost";
                    break;
                case 5:
                    if (day == 3) h = "New Wine";
                    break;
                case 6:
                    if (day == 22) h = "New Oil";
                    else if (day > 22 && day < 29) h = "Wood Offering";
                    break;
                case 7:
                    if (day < 3) h = "Rosh HaShannah";
                    else if (day < 10) h = "Yamin Noraim";
                    else if (day == 10) h = "Day of Atonement";
                    else if (day > 14 && day < 22) h = "Tabernacles";
                    else if (day == 22) h = "The Great Day";
                    break;
                case 9:
                    if (day > 24) h = "Hanukkah";
                    break;
                case 10:
                    if (day == 1) h = "Hanukkah";
                    break;
                case 12:
                    if (day == 14 || day == 15) h = "Purim";
                    break;
            }
            return h;
        }

        public string GetJubileeDate(DateTime time) => GetJubileeDate(ToLocalDate(time).Year);

        public string GetJubileeDate(int year) {
            year--;
            int a = year / 2000 + 1;
            int o = year / 500 + 1;     // onahs are counted across ages
            int j = year % 500 / 50 + 1;
            int s = year % 50 / 7 + 1;
            int y = year % 50 - 7 * s + 8;
            // "Shemittah 8" is Jubilee Year
            return (s < 8 ? $"Y:{y} S:{s} " : "") + $"J:{j} O:{o} A:{a}";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Nisan", "Iyar", "Sivan", "Tammuz", "Av", "Elul", "Tishrei", "Heshvan", "Kislev", "Tevet", "Shevat", "Adar", "Leap Week" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            var (year, month, day, _) = ToLocalDate(time);
            if (day == 0) {
                fx.DayFullName = $"Tekufah {dtfi.MonthNames[month - 1]}";
                fx.DayShortName = $"T{dtfi.AbbreviatedMonthNames[month - 1].Substring(0, 2)}";
            }
            else if (day == 17) {
                string d = null;
                if (month == 2) d = "Spring";
                else if (month == 5) d = "Summer";
                else if (month == 8) d = "Fall";
                else if (month == 11) d = "Winter";
                if (d != null) {
                    fx.DayFullName = $"Mid-{d}";
                    fx.DayShortName = $"M{d.Substring(0, 2)}";
                }
            }
            fx.Format = format.ReplaceUnescaped("n", $"'{GetHolyDay(time)}'").ReplaceUnescaped("j", $"'{GetJubileeDate(year)}'");
            return fx;
        }
    }
}
