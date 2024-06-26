﻿using AA.Net;

namespace WeirdCalendars.Support {
    internal class AstronomicalSeasonPlot {
        
        internal int YearDays { get; }
        private int[] quarters;
        internal int[] Quarters {
            get {
                if (quarters == null) PlotYear();
                return quarters;
            }
        }

        private double[] yearStart = new double[2];
        private int myYear;

        internal AstronomicalSeasonPlot(int year) {
            for (int i = 0; i < 2; i++) yearStart[i] = Earth.SeasonStart(year + i, Earth.Season.March).ToLastUTMidnight();
            YearDays = (int)(yearStart[1] - yearStart[0]);
            myYear = year;
        }

        private void PlotYear() {
            double lastStart = yearStart[0];
            quarters = new int[4];
            // Find lengths of seasons
            for (int s = 1; s < 4; s++) {
                double nextStart = Earth.SeasonStart(myYear, (Earth.Season)s).ToLastUTMidnight();
                quarters[s - 1] = (int)(nextStart - lastStart);
                lastStart = nextStart;
            }
            quarters[3] = (int)(yearStart[1] - lastStart);
        }
    }
}
