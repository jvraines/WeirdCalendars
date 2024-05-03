using System;
using AA.Net;
using static System.Math;

namespace WeirdCalendars.Support {
    internal static class Luna {
        
        internal enum VisibilityType {
            V,  // Easily visible
            VV, // Visible under perfect conditions
            VF, // May need optical aid
            IV, // Will need optical aid
            II, // Not visible with telescope
            I   // Not visible; below Danjon limit
        }

        private static double[] VisibilityCutoff = new double[] { 0.216, -0.014, -0.160, -0.232, -0.293, double.MinValue };

        /// <param name="start">A Julian Day at which to begin the search.</param>
        /// <param name="latitude">Latitude of observation in degrees.</param>
        /// <param name="longitude">Longitude of observation in degrees, West positive.</param>
        /// <param name="visibility">A VisibilityType value, default VV.</param>
        /// <returns>Time (JDE) of first visible crescent under specified conditions.</returns>
        /// <remarks>Yallop method https://webspace.science.uu.nl/~gent0113/islam/downloads/naotn_69.pdf</remarks>
        internal static double FirstVisibleCrescent(double start, double longitude, double latitude, VisibilityType visibility = VisibilityType.VV) {
            // Increase start to day of next new moon
            start = Floor(Moon.NextPhase(Moon.Phase.NewMoon, start));
            double search, T;
            // For each day
            for (search = start; true; search++) {
                // Find times of sunset and moonset
                DateTime searchD = search.ToDateTime();
                double sunset = ((DateTime)Sky.RiseTransitSet(Bodies.Sun, searchD, longitude, latitude, false).set).JulianEphemerisDay();
                double moonset = ((DateTime)Sky.RiseTransitSet(Bodies.Moon, searchD, longitude, latitude, false).set).JulianEphemerisDay();
                // Find best time of observation
                T = sunset + (moonset - sunset) * 4 / 9;
                
                // Find geocentric coordinates of sun and moon
                var sunP = Sun.Position(T);
                var moonP = Moon.Position(T);
                // Convert ecliptical to equatorial
                double epsilon = Sky.ObliquityOfEcliptic(T);
                var sunEq = Transform.ToEquatorial(sunP.longitude, sunP.latitude, epsilon);
                var moonEq = Transform.ToEquatorial(moonP.longitude, moonP.latitude, epsilon);
                // Find horizontal coordinates of sun and moon
                TimeSpan gst = Sky.GreenwichSiderealTime(T);
                var sunH = Transform.ToHorizontal(Sky.HourAngle(gst, longitude, sunEq.rightAscension), latitude, sunEq.declination);
                var moonH = Transform.ToHorizontal(Sky.HourAngle(gst, longitude, moonEq.rightAscension), latitude, moonEq.declination);

                double pi = Moon.Parallax(moonP.distance) * 60;
                double SD = 0.27245 * pi;
                double ARCV = moonH.altitude - sunH.altitude;
                double DAZ = sunH.azimuth - moonH.azimuth;
                double W = SD * (1 - Cos(ARCV.ToRadians()) * Cos(DAZ.ToRadians()));
                double q = (ARCV - (11.8371 - 6.3226 * W + 0.7319 * Pow(W, 2) - 0.1018 * Pow(W, 3))) / 10;

                if (q > VisibilityCutoff[(int)visibility]) break;
            }
            return T;
        }
    }
}
