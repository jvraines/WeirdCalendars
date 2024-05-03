using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class EarthEpicCalendar : WeirdCalendar {
       
        public override string Author => "Haven McClure";
        public override Uri Reference => new Uri("https://earthepiccalendar.com/");

        protected override DateTime SyncDate => new DateTime(2022, 11, 7);
        protected override int SyncOffset => 9701;
        public override DateTime MinSupportedDateTime => new DateTime(2, 1, 1);
        public override DateTime MaxSupportedDateTime => new DateTime(6000, 1, 1);

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> {
            ("n", "Moon number"),
            ("nn", "Day of lunation"),
            ("nnn", "Short moon format")
        };

        protected override int FirstMonth => 0;
        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return 0;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Quarters[month];
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override int GetDayOfYear(DateTime time) {
            return base.GetDayOfYear(time) - 1;
        }

        /// <summary>
        /// Gets the moon number of a date.
        /// </summary>
        /// <param name="time">A DateTime value within the valid range of this calendar.</param>
        /// <returns>A zero-based integer representing the moon number.</returns>
        /// <remarks>For the subdivision which the author calls "quarter," see the GetMonth method.</remarks>
        public int GetMoon(DateTime time) {
            int d = GetDayOfYear(time);
            EarthEpicPlot p = GetPlot(GetYear(time));
            int search = 0;
            while (p.Moons[search] < d) search++;
            return search - 1;
        }

        /// <summary>
        /// Gets the day of moon of a date.
        /// </summary>
        /// <param name="time">A DateTime value within the valid range of this calendar.</param>
        /// <returns>A zero-based integer representing the day of moon.</returns>
        /// <remarks>For the subdivision which the author calls "quarter," see the GetDayOfMonth method.</remarks>
        public int GetDayOfMoon(DateTime time) {
            int d = GetDayOfYear(time) - 1;
            int m = GetMoon(time);
            EarthEpicPlot p = GetPlot(GetYear(time));
            return d - p.Moons[m];
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            if (IsLeapYear(year)) {
                EarthEpicPlot p = GetPlot(year);
                return month == p.LeapQuarter && day == p.Quarters[p.LeapQuarter] - 1;
            }
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            return GetDaysInYear(year) == 366;
        }

        private static Dictionary<int, EarthEpicPlot> Plots = new Dictionary<int, EarthEpicPlot>();

        private EarthEpicPlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out EarthEpicPlot p)) {
                p = new EarthEpicPlot(year - SyncOffset);
                Plots.Add(year, p);
            }
            return p;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            // Missing "Southlight" because it is Month 0; fix in GetFormatWC
            SetNames(dtfi, new string[] { "Eastlight", "Northlight", "Westlight", "", "", "", "", "", "", "", "", "", "" }, null, new string[] { "Sunday", "Moonday", "Airday", "Waterday", "Earthday", "Fireday", "Starday" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (GetMonth(time) == 0) {
                fx.MonthFullName = "Southlight";
                fx.MonthShortName = "Sou";
            }
            if ("DfF".Contains(format) || format.Contains("n") || format.Contains("nnn")) {
                int m = GetMoon(time);
                int d = GetDayOfMoon(time);
                fx.LongDatePattern = $"(18.16.12) yyyy MMMM d ({m}.{d:D2}";
                fx.Format = format.ReplaceUnescaped("nnn",$"yyyy.d.{m}.{d:D2}").ReplaceUnescaped("nn", $"{d:D2}").ReplaceUnescaped("n", $"{m}");
            }
            return fx;
        }
    }
}