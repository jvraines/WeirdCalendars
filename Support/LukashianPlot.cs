using System;
using AA.Net;

namespace WeirdCalendars.Support {
    internal class LukashianPlot {

        private static TimeSpan solarMidnight = new TimeSpan(0, 21, 3, 28, 300);
        // by observation of time display at author's website

        internal int YearDays { get; }
        internal DateTime[] Days {
            get {
                if (dayStart == null) PlotYear();
                return dayStart;
            }
        }
        internal DateTime YearEnd => yearStart[1];

        private DateTime[] yearStart = new DateTime[2];
        private DateTime[] dayStart;

        internal LukashianPlot(int year) {
            // year is the Gregorian year of calendar start, i.e. previous December
            FindYearBounds(year);
            YearDays = (int)Math.Round((yearStart[1] - yearStart[0]).TotalDays);
        }

        private void PlotYear() {
            dayStart = new DateTime[YearDays + 1];
            for (int i = 0; i < YearDays; i++) {
                DateTime d = yearStart[0].Date.AddDays(i) + solarMidnight;
                dayStart[i] = d - TimeSpan.FromMinutes(Sky.EquationOfTime(d.JulianEphemerisDay()));
            }
            dayStart[YearDays] = yearStart[1];
        }

        private void FindYearBounds(int year) {
            // Year begins with the first clock "midnight" on or after the southern solstice
            for (int i = 0; i < 2; i++) {
                double jde = Earth.SeasonStart(year + i, Earth.Season.December);
                DateTime solstice = jde.ToDateTime(true);
                DateTime clockMidnight = solstice.Date + solarMidnight;
                clockMidnight -= TimeSpan.FromMinutes(Sky.EquationOfTime(clockMidnight.JulianEphemerisDay()));
                if (solstice <= clockMidnight) yearStart[i] = clockMidnight;
                else {
                    clockMidnight = solstice.Date.AddDays(1) + solarMidnight;
                    yearStart[i] = clockMidnight - TimeSpan.FromMinutes(Sky.EquationOfTime(clockMidnight.JulianEphemerisDay()));
                }
            }
        }
    }
}
