using AA.Net;

namespace WeirdCalendars.Support {
    internal class BabylonianPlot {

        internal const double BabylonLongitude = -44.421111; // East negative in AA.Net
        internal const double BabylonLatitude = 32.5425;

        internal int YearDays { get; }
        private int[] moons;
        internal int[] Moons {
            get {
                if (moons == null) PlotYear();
                return moons;
            }
        }

        private int Year;
        private int[] yearStart = new int[2];

        internal BabylonianPlot(int year) {
            Year = year;
            FindYearBounds();
            YearDays = yearStart[1] - yearStart[0];
        }

        private void FindYearBounds() {
            // Starts with the first visible crescent on or after the March equinox
            yearStart[0] = (int)Luna.FirstVisibleCrescent(Earth.SeasonStart(Year, Earth.Season.March), BabylonLongitude, BabylonLatitude);
            yearStart[1] = (int)Luna.FirstVisibleCrescent(Earth.SeasonStart(Year + 1, Earth.Season.March), BabylonLongitude, BabylonLatitude);
        }

        private void PlotYear() {
            moons = new int[13];
            int monthStart = yearStart[0];
            for (int m = 0; m < 13; m++) {
                int nextMonthStart = (int)Luna.FirstVisibleCrescent(monthStart + 25, BabylonLongitude, BabylonLatitude);
                if (nextMonthStart > yearStart[1]) break;
                moons[m] = nextMonthStart - monthStart;
                monthStart = nextMonthStart;
            }
        }
    }
}
