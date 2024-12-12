using System;
using System.Collections.Generic;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class STAYCalendar : WeirdCalendar {

        public override string Author => "Shriramana Sharma";
        public override Uri Reference => new Uri("https://web.archive.org/web/20070213064110/http://samvit.org/calendar/astro/stay.html");

        protected override DateTime SyncDate => new DateTime(2025, 1, 1);
        protected override int SyncOffset => 0;

        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public STAYCalendar() => Title = "Simplified Tropical Astronomical Year Calendar";

        private Dictionary<int, STAYPlot> Plots = new Dictionary<int, STAYPlot>();

        private STAYPlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out STAYPlot p)) {
                p = new STAYPlot(year);
                Plots.Add(year, p);
            }
            return p;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).MonthDays[month - 1];
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 31;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays == 366;
        }
    }
}
