using System;
using System.Collections.Generic;
using AA.Net;
using System.Linq;
using static System.Math;

namespace WeirdCalendars {

    internal class EclipticPlot {

        internal int YearDays { get; }
        internal int Saturnium { get; }
        internal int SaturniumYear { get; }
        internal int Age { get; }

        private int[] month;
        internal int[] Month {
            get {
                if (month == null) PlotYear();
                return month;
            }
        }

        private double[] yearStart = new double[2];
        private int year;
        private static readonly List<double> Ages = new List<double> { 1661969.5, 2447240.5 };

        internal EclipticPlot(int year) {
            this.year = year;
            FindYearBounds();
            YearDays = (int)(yearStart[1] - yearStart[0]);
            for (Age = Ages.Count - 1; Age >= 0; Age--) if (yearStart[0] >= Ages[Age]) break;
            int y;
            for (y = 73; y >= 0; y--) if (year >= Saturnia[Age, y]) break;
            Saturnium = y + 1;
            SaturniumYear = year - Saturnia[Age, y] + 1;
            // Fix up age number
            Age += 3;
        }

        private void FindYearBounds() {
            for (int i = 0; i < 2; i++) yearStart[i] = AdjustedDay(Earth.SeasonStart(year + i, Earth.Season.March));
        }

        private double AdjustedDay(double JDE) {
            return (int)(JDE.JulianUniversalDay() + 0.5) - 0.5;
        }

        private void PlotYear() {
            int[] seasons = new int[4];
            double lastStart = yearStart[0];
            // Find lengths of seasons
            for (int s = 1; s < 4; s++) {
                double nextStart = AdjustedDay(Earth.SeasonStart(year, (Earth.Season)s));
                seasons[s - 1] = (int)(nextStart - lastStart);
                lastStart = nextStart;
            }
            seasons[3] =(int)(yearStart[1] - lastStart);

            // Find "unadjusted month" of perihelion for latest crossing which occurred
            //    most recently before the end of this year.
            double popMonth = (GetPointOfPerihelion(yearStart[1]) / 30 + 6) % 12;

            // Apportion month durations
            month = new int[12];
            for (int s = 0; s < 4; s++) {
                int days = seasons[s] / 3;
                for (int m = 0; m < 3; m++) month[s * 3 + m] = days;
                int remains = seasons[s] - days * 3;
                if (remains > 0) {
                    // Add one day to month farthest from pop
                    int month1 = s * 3;
                    double dist1 = ShortestDistance(month1, popMonth);
                    double dist3 = ShortestDistance(month1 + 2, popMonth);
                    if (dist1 > dist3) month[month1]++; else month[month1 + 2]++;
                    if (--remains > 0) month[month1 + 1]++;
                }
            }

            double ShortestDistance(double angle1, double angle2) {
                double diff = Abs(angle1 - angle2);
                return diff > 6 ? 12 - diff : diff;
            }
        }

        // J2000 coordinate of Fixed Ecliptic Zero (it coincided with the fall equinox)
        private readonly (TimeSpan rightAscension, double declination) FixedEclipticZero = (TimeSpan.FromHours(18), -23.439);
        
        private const double KnownCrossing = 2458110.5; //22 Dec 2017
        private static readonly TimeSpan KnownPerihelionRA = new TimeSpan(0, 06, 56, 27, 841);
        private const double KnownPerihelionDec = 22.8028;
        private const int SaturnPeriodDays = 10759;

        // Stores time of Saturn crossing and J2000 coordinates of most recent perihelion
        private static List<(double crossing, TimeSpan rightAscension, double declination)> Crossings = new List<(double, TimeSpan, double)> { (KnownCrossing, KnownPerihelionRA, KnownPerihelionDec) };

        // Saturnium start years for Pisces and Aquarius ages (within Years 0 to 4000)
        private static int[,] Saturnia = new int[,] {
                { 0, 0, 0, 0, 0, 0, -15, 14, 44, 73, 103, 132, 161, 191, 220, 250, 279, 309, 338, 368, 397, 427, 456, 485, 515, 544, 574, 603, 633,  662, 692, 721, 751, 780, 810, 839, 869, 898, 927, 957, 986, 1016, 1045, 1075, 1104, 1134, 1163, 1192, 1222, 1251, 1281, 1310, 1340, 1369, 1399, 1428, 1458, 1487, 1516, 1546, 1575, 1605, 1634, 1664, 1693, 1723, 1752, 1782, 1811, 1841, 1870, 1899, 1929, 1958 },
                { 1988, 2017, 2047, 2076, 2106, 2135, 2164, 2194, 2223, 2253, 2282, 2312, 2341, 2371, 2400, 2430, 2459, 2489, 2518, 2547, 2577, 2606, 2636, 2665, 2695, 2724, 2754, 2783, 2813, 2842, 2872, 2901, 2930, 2960, 2989, 3019, 3048, 3078, 3107, 3137, 3166, 3195, 3225, 3254, 3284, 3313, 3343, 3372, 3402, 3431, 3461, 3490, 3520, 3549, 3578, 3608, 3637, 3667, 3696, 3726, 3755, 3785, 3814, 3844, 3873, 3902, 3932, 3961, 3991, 4021, 4050, 4079, 4999, 5000 }
            };

        private double GetPointOfPerihelion(double time) {
            // Find nearest previous crossing
            var pop = Crossings.Where(x => x.crossing < time).OrderBy(x => time - x.crossing).FirstOrDefault();
            double search = 0;
            // If none, search backward from earliest crossing until less than or equal to time
            if (pop.crossing == 0) {
                search = Crossings.Min(x => x.crossing);
                Func<double, double> inc = x => x -= SaturnPeriodDays;
                Func<double, double, bool> comp = (x, y) => x > y;
                SearchPerihelions(inc, comp);
            }
            // else if outside orbital period, search forward until within orbital period
            else if (time - pop.crossing > SaturnPeriodDays) {
                search = pop.crossing;
                Func<double, double> inc = x => x += SaturnPeriodDays;
                Func<double, double, bool> comp = (x, y) => y - x > SaturnPeriodDays;
                SearchPerihelions(inc, comp);
            }
            // Return point in ecliptic degrees of longitude for equinox of date
            return FromJ2000(pop.rightAscension, pop.declination, time).longitude;
            
            void SearchPerihelions(Func<double, double> increment, Func<double, double, bool> compare) {
                do {
                    search = increment(search);
                    double cross = GetSaturnCrossing(search);
                    // Get time and position of most recent perihelion
                    double pTime = Earth.NextPerihelion(cross.ToFractionalYear() - 1);
                    var pLoc = Earth.Position(pTime);
                    var p2000 = ToJ2000(pLoc.longitude, pLoc.latitude, pTime);
                    pop = (cross, p2000.rightAscension, p2000.declination);
                    Crossings.Add(pop);
                }
                while (compare(search, time));
            }
        }
        
        private double GetSaturnCrossing(double search) {
            double fezOfDate = FromJ2000(FixedEclipticZero.rightAscension, FixedEclipticZero.declination, search).longitude;
            double longitude = Saturn.Position(search).longitude;
            double minSearch = search;
            int step = Sign(fezOfDate - longitude);
            double thisDiff = 0, minDiff = 0;
            if (step != 0) {
                minDiff = Abs(fezOfDate - longitude);
                // Search in one-day increments
                do {
                    longitude = Saturn.Position(search += step).longitude;
                    fezOfDate = FromJ2000(FixedEclipticZero.rightAscension, FixedEclipticZero.declination, search).longitude;
                    thisDiff = Abs(fezOfDate - longitude);
                    if (thisDiff < minDiff) {
                        minDiff = thisDiff;
                        minSearch = search;
                    }
                }
                while (thisDiff == minDiff);
            }
            // Interpolate from last two errors
            return minSearch + minDiff / (thisDiff + minDiff) * (search - minSearch);
        }

        private (TimeSpan rightAscension, double declination) ToJ2000(double longitude, double latitude, double time) {
            double epsilon = Sky.ObliquityOfEcliptic(time, false);
            var equ = Transform.ToEquatorial(longitude, latitude, epsilon);
            return Transform.Precession(equ.rightAscension, equ.declination, time, Time.J2000);
        }

        private (double longitude, double latitude) FromJ2000 (TimeSpan rightAscension, double declination, double time) {
            var equ = Transform.Precession(rightAscension, declination, Time.J2000, time);
            double epsilon = Sky.ObliquityOfEcliptic(time, false);
            return Transform.ToEcliptical(equ.rightAscension, equ.declination, epsilon);
        }
    }
}
