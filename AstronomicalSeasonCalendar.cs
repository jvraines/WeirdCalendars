using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class AstronomicalSeasonCalendar : WeirdCalendar {
        
        public override string Author => "Chase Roycroft";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Astronomical_Season_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 3, 20);
        protected override int SyncOffset => 0;

        // Maximum valid date for season calculation from VSOP87.
        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public override int DaysInWeek => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Quarters[month - 1];
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 4;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        private static Dictionary<int, AstronomicalSeasonPlot> Plots = new Dictionary<int, AstronomicalSeasonPlot>();

        private AstronomicalSeasonPlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out AstronomicalSeasonPlot p)) {
                p = new AstronomicalSeasonPlot(year - SyncOffset);
                Plots.Add(year, p);
            }
            return p;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "A", "B", "C", "D", "", "", "", "", "", "", "", "", "" });
            dtfi.ShortDatePattern = "yyyyMMMMdd";
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            double tyme = time.TimeOfDay.TotalHours / 24;
            fx.ShortTimePattern = $"{tyme:.00}";
            fx.DateAndTimeSeparator = "";
            return fx;
        }
    }
}
