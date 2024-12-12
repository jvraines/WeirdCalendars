using AA.Net;

namespace WeirdCalendars.Support {
    internal class STAYPlot {
        internal int YearDays { get; private set; }
        private int[] monthDays;
        internal int[] MonthDays {
            get {
                if (monthDays == null) PlotYear();
                return monthDays;
            }
        }
        private int Year;
        private double[] YearStart = new double[2];

        public STAYPlot(int year) {
            Year = year;
            FindYearBounds();
            YearDays = (int)(YearStart[1] - YearStart[0]);
        }

        private void FindYearBounds() {
            for (int i = 0; i < 2; i++) {
                YearStart[i] = Earth.SeasonStart(Year + i, Earth.Season.March).ToLastUTMidnight() - 78;
            }
        }

        private void PlotYear() {
            monthDays = new int[] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            double aphelion = Earth.NextAphelion(Year).ToLastUTMidnight();
            int apMonth = (int)(aphelion - YearStart[0]) / 30;
            for (int i = apMonth - 2; i < apMonth + 3; i++) monthDays[i]++;
            if (YearDays == 366) monthDays[11]++;
        }
    }
}
