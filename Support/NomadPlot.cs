using AA.Net;
using System;

namespace WeirdCalendars.Support {
    internal class NomadPlot {
        
        internal int YearDays { get; }
        private int[] moons;
        internal int[] Moons {
            get {
                if (moons == null) PlotYear();
                return moons;
            }
        }

        private int Year;
        private TimeSpan AlcyoneYearRA;

        internal NomadPlot(int year) {
            Year = year;
            AlcyoneYearRA = Sky.ApparentPositionStar(AlcyoneRA, AlcyoneDec, Time.J2000, Time.JulianEphemerisDay(year, 5, 1), AlcyonePropRA, AlcyonePropDec).rightAscension;
            FindYearBounds();
            YearDays = yearStart[1] - yearStart[0];
            //Console.WriteLine($"{Time.ToDateTime(yearStart[0]).ToString("M/d/yyyy")} {YearDays}");
        }

        private int[] yearStart = new int[2];

        // Coordinates of the Kazakh Steppe, a spot near the capital of Astana
        private const double SteppeLongitude = -70.5; // east negative AA.Net convention
        private const double SteppeLatitude = 51.6;
        private const double SteppeElevation = 330;
        
        // J2000 coordinates of the Pleiades central star
        private readonly TimeSpan AlcyoneRA = new TimeSpan(0, 3, 47, 29, 77);
        private readonly double AlcyoneDec = Transform.ToDegrees(24, 6, 18.49);

        // Alcyone proper motion
        private const double AlcyonePropRA = 0.01934;
        private const double AlcyonePropDec = -0.04367;

        private TimeSpan TopoMoonRA (double jde) {
            var moon = Moon.Position(jde);
            var moonEq = Transform.ToEquatorial(moon.longitude, moon.latitude, Sky.ObliquityOfEcliptic(jde));
            return Transform.ToTopocentric(moonEq.rightAscension, moonEq.declination, Moon.Parallax(moon.distance), SteppeLongitude, SteppeLatitude, SteppeElevation, jde).rightAscension;
        }

        private void FindYearBounds() {
            // "The year begins with the month whose 1st day occurs when the Moon passes the Pleiades immediately after a dark moon (when the crescent Moon first becomes visible), so at that time the age of the Moon is approximately 1 day."
            for (int y = 0; y < 2; y++) {
                double minDate = 0;
                TimeSpan minDiff = TimeSpan.FromHours(24);
                double jde = Time.JulianEphemerisDay(Year + y, 3, 1);
                for (int i = 0; i < 3; i++) {
                    jde = Luna.FirstVisibleCrescent(jde, SteppeLongitude, SteppeLatitude);
                    TimeSpan thisDiff = AlcyoneYearRA - TopoMoonRA(jde);
                    if (thisDiff >= TimeSpan.Zero) {
                        if (thisDiff < minDiff) {
                            minDiff = thisDiff;
                            minDate = jde;
                        }
                    }
                    jde++;
                }
                minDate = (int)minDate + 0.5;
                while (TopoMoonRA(minDate) < AlcyoneYearRA) minDate++;
                yearStart[y] = (int)minDate;
            }
        }

        private void PlotYear() {
            moons = new int[14];
            int monthStart = yearStart[0];
            int m;
            for (m = 0; m < 14; m++) {
                int monthEnd = monthStart + 24;
                if (yearStart[1] - monthEnd < 7) break;
                while (TopoMoonRA(monthEnd) < AlcyoneYearRA) monthEnd++;
                moons[m] = monthEnd - monthStart;
                monthStart = monthEnd;
            }
            moons[m] = yearStart[1] - monthStart;
        }
    }
}
