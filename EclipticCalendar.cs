using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {

    public class EclipticCalendar : WeirdCalendar {
        
        public override string Author => "Damon Scott";
        public override Uri Reference => new Uri("https://web.archive.org/web/20101224152838/http://people.fmarion.edu/dscott/eclipticcalendar/pdfs/TechnicalDefinition.pdf");

        protected override DateTime SyncDate => new DateTime(2020, 3, 20);
        protected override int SyncOffset => 4493;
        
        // Stay within the bounds of VSOP87 Saturn accuracy
        public override DateTime MaxSupportedDateTime => new DateTime(4000, 1, 1);

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
                ("a", "Age"),
                ("S", "Saturnium"),
                ("n", "Date as numerals")
            };

        /// <summary>
        /// Gets the Age of a year.
        /// </summary>
        /// <param name="year">A year within the valid range of this calendar.</param>
        /// <returns>An integer represeting the Age number.</returns>
        public int GetAge(int year) {
            ValidateDateParams(year, 0);
            return GetPlot(year).Age;
        }

        /// <summary>
        /// Gets the Saturnium of a year.
        /// </summary>
        /// <param name="year">A year within the valid range of this calendar.</param>
        /// <returns>An integer represeting the Saturnium number.</returns>
        public int GetSaturnium(int year) {
            ValidateDateParams(year, 0);
            return GetPlot(year).Saturnium;
        }

        /// <summary>
        /// Gets the year of Saturnium of a year.
        /// </summary>
        /// <param name="year">A year within the valid range of this calendar.</param>
        /// <returns>An integer representing the consecutive year of Saturnium number.</returns>
        public int GetYearOfSaturnium(int year) {
            ValidateDateParams(year, 0); 
            return GetPlot(year).SaturniumYear;
        }

        public override int GetDaysInYear(int year) {
            ValidateDateParams(year, 0); 
            return GetPlot(year).YearDays;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Month[month - 1];
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        private static Dictionary<int, EclipticPlot> Plots = new Dictionary<int, EclipticPlot>();
        
        private EclipticPlot GetPlot(int year) {
            year -= SyncOffset;
            if (!Plots.TryGetValue(year, out EclipticPlot p)) {
                p = new EclipticPlot(year);
                Plots.Add(year, p);
            }
            return p;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Various", "Various", "Various", "Various", "Various", "Various", "Various", "Various", "Various", "Various", "Various", "Various", "" });
        }

        static string[] monthName = { "Sagittarius", "Capricornus", "Aquarius", "Pisces", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            var ymd = ToLocalDate(time);
            int age = GetAge(ymd.Year);
            fx.MonthFullName = monthName[(11 - age + ymd.Month) % 12];
            fx.MonthShortName = fx.MonthFullName.Substring(0, 3);
            int sat = GetSaturnium(ymd.Year);
            int year = GetYearOfSaturnium(ymd.Year);
            fx.Format = format.ReplaceUnescaped("n", $"{age:D2}/{sat:D2}/{year:D2}/{ymd.Month:D2}/{ymd.Day:D2}").ReplaceUnescaped("a", $"{age:D2}").ReplaceUnescaped("S", $"{sat:D2}");
            return fx;
        }
    }
}
