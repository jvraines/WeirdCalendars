using AA.Net;
using System;

namespace WeirdCalendars.Support {
    internal class AquarianMoonTurtlePlot {
        internal int YearDays { get; }
        private int blueMoon = 0;
        internal int BlueMoon {
            get {
                if (moons == null) PlotYear();
                return blueMoon;
            }
            private set {
                blueMoon = value;
            }
        }
        private int[] moons;
        internal int[] Moons {
            get {
                if (moons == null) PlotYear();
                return moons;
            }
        }

        private int Year;

        internal AquarianMoonTurtlePlot(int year) {
            Year = year;
            FindYearBounds();
            YearDays = (int)(yearStart[1] - yearStart[0]);
        }

        protected double[] yearStart = new double[2];

        private const double MercurialOffset = 0.5 + 11.035 / 360;
        private double DayOf(double jde) => Math.Round(jde.JulianUniversalDay() + MercurialOffset) - MercurialOffset;

        private void FindYearBounds() {
            // Begins with the new moon preceding the March equinox
            for (int i = 0; i < 2; i++) {
                double equinox = Earth.SeasonStart(Year + i, Earth.Season.March);
                double newMoon = Moon.NextPhase(Moon.Phase.NewMoon, equinox - 29);
                yearStart[i] = DayOf(newMoon);
            }
        }

        private void PlotYear() {
            moons = new int[13];
            int moonPtr = 0;
            double lastNewMoon = yearStart[0];
            int nextSeason = 0;
            double seasonEnds;
            int year = Year;
            GetNextSeason();
            // Find subsequent new moons and moon start days through end of year
            do {
                double nextNewMoon = DayOf(Moon.NextPhase(Moon.Phase.NewMoon, lastNewMoon + 28));
                if (BlueMoon == 0) {
                    if (nextNewMoon < seasonEnds) {
                        if (moonPtr > 0 && moonPtr % 3 == 0) BlueMoon = moonPtr == 12 ? 13 : moonPtr;
                    }
                    else GetNextSeason();
                }
                moons[moonPtr++] = (int)(nextNewMoon - lastNewMoon);
                lastNewMoon = nextNewMoon;
            }
            while (lastNewMoon < yearStart[1]);

            void GetNextSeason() {
                nextSeason = ++nextSeason % 4;
                if (nextSeason == 0) year++;
                seasonEnds = Earth.SeasonStart(year, (Earth.Season)nextSeason);
            }
        }
    }
}
