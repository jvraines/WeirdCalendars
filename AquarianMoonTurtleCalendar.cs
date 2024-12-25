using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class AquarianMoonTurtleCalendar : WeirdCalendar {

        public override string Author => "Dustin Chalker";
        public override Uri Reference => new Uri("https://groups.io/g/calendars/topic/aquarian_moon_turtle_calendar/110228069");

        protected override DateTime SyncDate => new DateTime(2025, 2, 27, 23, 15, 51, 600);
        protected override int SyncOffset => 0;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;
        public override DateTime MaxSupportedDateTime => VSOPLimit;

        private static Dictionary<int, AquarianMoonTurtlePlot> Plots = new Dictionary<int, AquarianMoonTurtlePlot>();

        private AquarianMoonTurtlePlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out AquarianMoonTurtlePlot p)) {
                p = new AquarianMoonTurtlePlot(year);
                Plots.Add(year, p);
            }
            return p;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).BlueMoon == 0 ? 12 : 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Moons[month - 1];
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).BlueMoon > 0;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).BlueMoon == month;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).BlueMoon;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            string[] ma = new string[13];
            for (int i = 0; i < 12; i++) {
                int j = (i + 2) % 12;
                m[i] = dtfi.MonthNames[j];
                ma[i] = dtfi.AbbreviatedMonthNames[j];
            }
            m[12] = "";
            ma[12] = "";
            SetNames(dtfi, m, ma);
        }

        private static string[] Turtles = new string[] { "Raphael", "Leonardo", "Donatello", "Michelangelo" };
        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            CustomizeTimes(fx, time);
            int blue = GetPlot(ld.Year).BlueMoon;
            // N.B. Blue moon is 0, 3, 6, 9, or *13*
            if (blue > 0) {
                string m, ma;
                if (ld.Month - 1 == blue || ld.Month == 13 && blue == 13) {
                    m = Turtles[blue / 4];
                    ma = m.Substring(0, 3);
                }
                else {
                    int mo = ld.Month - (ld.Month > blue ? 2 : 1);
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
