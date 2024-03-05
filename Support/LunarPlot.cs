using AA.Net;

namespace WeirdCalendars.Support {
    internal class LunarPlot {
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

        protected int Year;
        protected Earth.Season FirstSeason { get; set; }

        internal LunarPlot(int year, Earth.Season start) {
            Year = year;
            FirstSeason = start;
            FindYearBounds();
            YearDays = yearStart[1] - yearStart[0];
        }

        protected int[] yearStart = new int[2];

        private void FindYearBounds() {
            // Begins with the new moon preceding the first full moon after the first season
            for (int i = 0; i < 2; i++) {
                double equinox = Earth.SeasonStart(Year + i, FirstSeason);
                double fullMoon = Moon.NextPhase(Moon.Phase.FullMoon, equinox);
                double newMoon = Moon.NextPhase(Moon.Phase.NewMoon, fullMoon - 30);
                yearStart[i] = JulianToUTMidnight(newMoon);
            }
        }

        private void PlotYear() {
            moons = new int[13];
            int moonPtr = 0;
            int lastNewMoon = yearStart[0];
            int moonCounter = 1;
            int nextSeason = (int)FirstSeason;
            double seasonEnds;
            int year = Year;
            GetNextSeason();
            // Find subsequent new moons and moon start days through end of year
            do {
                int nextNewMoon = JulianToUTMidnight(Moon.NextPhase(Moon.Phase.NewMoon, lastNewMoon + 28));
                double nextFullMoon = Moon.NextPhase(Moon.Phase.FullMoon, nextNewMoon);
                if (nextFullMoon < seasonEnds) moonCounter++;
                else {
                    if (moonCounter > 3) BlueMoon = moonPtr - 1;
                    moonCounter = 1;
                    GetNextSeason();
                }
                moons[moonPtr++] = nextNewMoon - lastNewMoon;
                lastNewMoon = nextNewMoon;
            }
            while (lastNewMoon < yearStart[1]);

            void GetNextSeason() {
                nextSeason = ++nextSeason % 4;
                if (nextSeason == 0) year++;
                seasonEnds = Earth.SeasonStart(year, (Earth.Season)nextSeason);
            }
        }

        protected int JulianToUTMidnight(double jde) {
            return (int)(jde.JulianUniversalDay() + 0.5);
        }
    }
}
