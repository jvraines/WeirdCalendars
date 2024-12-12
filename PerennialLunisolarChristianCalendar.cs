using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class PerennialLunisolarChristianCalendar : WeirdCalendar {
        
        public override string Author => "Paculino";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Perennial_Lunisolar_Christian_Calendar");

        protected override DateTime SyncDate => new DateTime(2025, 1, 29);
        protected override int SyncOffset => 0;

        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        private Dictionary<int, PerennialChristianPlot> Plots = new Dictionary<int, PerennialChristianPlot>();
        private PerennialChristianPlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out PerennialChristianPlot p)) {
                p = new PerennialChristianPlot(year);
                Plots.Add(year, p);
            }
            return p;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Months[month];
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).LeapMonth;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).LeapMonth == month;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).LeapMonth > 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Simon", "Matthias", "Peter", "Andrew", "James the Greater", "John", "Phillip", "Bartholomew", "Thomas", "Matthew", "James the Lesser", "Thaddaeus", "" }, new string[] { "Sim", "Mts", "Pet", "And", "JaG", "Joh", "Phi", "Bar", "Tho", "Mtw", "JaL", "Tha", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            int lm = GetLeapMonth(ld.Year);
            if (lm > 0) {
                string m, ma;
                if (ld.Month == lm) {
                    m = "Judas";
                    ma = "Jud";
                }
                else {
                    int mo = ld.Month - (ld.Month > lm ? 2 : 1);
                    m = dtfi.MonthNames[mo];
                    ma = dtfi.AbbreviatedMonthNames[mo];
                }
                fx.MonthFullName = m;
                fx.MonthShortName = ma;
            }
            return fx;
        }
    }
}
