using System;
using AA.Net;
using System.Linq;
using System.Collections.Generic;

namespace WeirdCalendars {
    
    internal class EarthEpicPlot {
        
        internal int YearDays { get; }
        private int[] quarters;
        internal int[] Quarters {
            get {
                if (quarters == null) PlotSolarYear();
                return quarters;
            }
        }
        private int leapQuarter = -1;
        internal int LeapQuarter {
            get {
                if (quarters == null) PlotSolarYear();
                return leapQuarter;
            }
        }
        private int[] moons;
        internal int[] Moons {
            get {
                if (moons == null) PlotLunarYear();
                return moons;
            }
        }

        internal EarthEpicPlot(int year) {
            FindYearBounds(year);
            YearDays = yearStart[1] - yearStart[0];
        }

        private int[] yearStart = new int[2];
        
        private void FindYearBounds(int year) {
            // Begins when the sun enters 15° Scorpio in the tropical zodiac (i.e. 225° ecliptic longitude).
            for (int i = 0; i < 2; i++) yearStart[i] = CrossQuarterDate(Time.JulianDay(year + i, 11, 4), 225);
        }
        
        private int CrossQuarterDate (double search, double longitude) {
            double thisDiff = 360, lastDiff = 361;
            List<(double jd, double diff)> probe = new List<(double jd, double longitude)>();
            // Probe until difference in position starts to increase
            while (thisDiff < lastDiff) {
                lastDiff = thisDiff;
                //thisDiff = longitude - Sun.Position(search).longitude - Sun.Semidiameter(search).equatorial / 3600;
                //Author appears to use center of the sun, rather than leading edge
                thisDiff = longitude - Sun.Position(search).longitude;
                probe.Add((search, thisDiff));
                thisDiff = Math.Abs(thisDiff);
                search++;
            }
            // Interpolate time of zero difference
            int j = probe.Count;
            double n = Interpolation.Zero(probe[j - 3].diff, probe[j - 2].diff, probe[j - 1].diff);
            double z = probe[j - 2].jd + n;
            // Return closest UT midnight
            return (int)Math.Round(z + 0.5);
        }

        private void PlotSolarYear() {
            int[] seasons = new int[4];
            int lastStart = yearStart[0];
            // Find lengths of quarters
            for (int q = 1; q < 4; q++) {
                int nextStart = CrossQuarterDate(lastStart + 85, (225 + q * 90) % 360);
                seasons[q - 1] = nextStart - lastStart;
                lastStart = nextStart;
            }
            seasons[3] = yearStart[1] - lastStart;
            // Sort seasons by length
            var sorted = seasons.Select((x, index) => new {x, index }).OrderByDescending(y => y.x).ToArray();
            // Initialize quarters to standardized lengths for 364-day year
            quarters = new int[] { 91, 91, 91, 91 };
            // Add 365th day to longest quarter
            quarters[sorted[0].index]++;
            // Add possible 366th day to second longest quarter
            if (YearDays == 366) {
                quarters[sorted[1].index]++;
                leapQuarter = sorted[1].index;
            }
        }

        private void PlotLunarYear() {
            moons = new int[15];
            int moonPtr = 0;
            // Find new moon immediately preceding this year
            double lastNewMoon = Moon.NextPhase(Moon.Phase.NewMoon, yearStart[0] - 30).ToLastUTMidnight();
            // Find subsequent new moons and moon start days through end of year
            while (lastNewMoon < yearStart[1]) {
                double nextNewMoon = Moon.NextPhase(Moon.Phase.NewMoon, lastNewMoon + 28).ToLastUTMidnight();
                moons[moonPtr++] = (int)(lastNewMoon - yearStart[0] - 1);
                lastNewMoon = nextNewMoon;
            }
            moons[moonPtr] = yearStart[1];
        }
    }
}