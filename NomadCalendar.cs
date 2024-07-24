using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class NomadCalendar : WeirdCalendar {

        public override string Author => "Traditional and Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/knc/Kazakh_Nomad_Calendar.htm");

        protected override DateTime SyncDate => new DateTime(2024, 5, 9);
        protected override int SyncOffset => 3680;

        // Maximum valid date for season calculation from VSOP87.
        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("n", "Year name"),
            ("c", "Cycle number"),
            ("I", "ISO-like format")
        };

        private static Dictionary<int, NomadPlot> Plots = new Dictionary<int, NomadPlot>();

        private NomadPlot GetPlot(int year) {
            int gYear = year - SyncOffset;
            if (!Plots.TryGetValue(gYear, out NomadPlot p)) {
                p = new NomadPlot(gYear);
                Plots.Add(gYear, p);
            }
            return p;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).Moons[13] > 0 ? 14 : 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Moons[month - 1];
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfMonth(time) - 1) % 7);
        }

        public string GetYearName(int year) {
            return YearName[year % 12];
        }

        public int GetCycle(int year) {
            ValidateDateParams(year, 0);
            return year / 12;
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
            SetNames(dtfi, null, null, new string[] { "Arcturus", "Betelgeuse", "Canopus", "Deneb", "Elnath", "Fomalhaut", "Sirius" });
        }

        private static string[] YearName = { "Mouse", "Cow", "Leopard", "Hare", "Wolf", "Snake", "Horse", "Sheep", "Monkey", "Hen", "Dog", "Boar" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            int n = GetMonthsInYear(ymd.Year) == 13 ? ((14 - ymd.Month) * 2 + 1) % 26 : ((15 - ymd.Month) * 2 + 1) % 28;
            fx.MonthFullName = $"{n}-togys";
            fx.MonthShortName = $"{n}t";
            int cycle = GetCycle(ymd.Year);
            string yearName = $"'{GetYearName(ymd.Year)}'";
            fx.ShortDatePattern = $"{cycle}-{yearName}-{ymd.Month}-{ymd.Day}";
            fx.Format = format.ReplaceUnescaped("n", yearName).ReplaceUnescaped("c", cycle.ToString()).ReplaceUnescaped("I", $"{cycle}-{ymd.Year % 12}-{ymd.Month}-{ymd.Day}");
            return fx;
        }
    }
}
