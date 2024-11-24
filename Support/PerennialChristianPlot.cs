using AA.Net;

namespace WeirdCalendars.Support {
    internal class PerennialChristianPlot {
        
        internal int YearDays { get; }

        private int leapMonth;
        internal int LeapMonth {
            get {
                if (months == null) PlotYear();
                return leapMonth;
            }
        }
        
        private int[] months;
        internal int[] Months {
            get {
                if (months == null) PlotYear();
                return months;
            }
        }

        private int Year;
        private double[] YearStart = new double[2];

        internal PerennialChristianPlot(int year) {
            Year = year;
            FindYearBounds();
            YearDays = (int)(YearStart[1] - YearStart[0]);
        }

        const double JerusalemOffset = 35.2296 / 15 / 24;

        private void FindYearBounds() {
            for (int i = 0; i < 2; i++) {
                double solstice = Earth.SeasonStart(Year + i - 1, Earth.Season.December);
                double fullMoon = Moon.NextPhase(Moon.Phase.FullMoon, solstice);
                double endWindow = Moon.NextPhase(Moon.Phase.FullMoon, fullMoon + 14) + JerusalemOffset - 11;
                int dow = Time.DayOfWeek(endWindow);
                YearStart[i] = (endWindow - (dow + 4) % 7).ToLastUTMidnight();
            }
        }

        private void PlotYear() {
            months = new int[14];
            int m = 1;
            double lastFullMoon = 0;
            double lastStart = YearStart[0];
            while (lastStart < YearStart[1]) {
                double fullMoon = Moon.NextPhase(Moon.Phase.FullMoon, lastStart + 39);
                months[m] = fullMoon + JerusalemOffset - lastStart < 46 ? 28 : 35;
                lastStart += months[m];
                if (leapMonth == 0 && m % 3 == 0) {
                    Earth.Season s = (Earth.Season)(m / 3 - 1);
                    double point = Earth.SeasonStart(Year, s);
                    if (point > lastFullMoon) {
                        leapMonth = m;
                    }
                }
                lastFullMoon = fullMoon;
                m++;
            }
        }
    }
}
