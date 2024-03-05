using System.Collections.Generic;
using AA.Net;

namespace WeirdCalendars {
    public class HermeticLunarPlot {

        internal protected int YearDays { get; }
        internal protected List<(int MonthDays, int[] WeekDays)> Months {
            get {
                if (months == null) PlotYear();
                return months;
            }
        }
        private int[] NewMoon = new int[2];
        private List<(int, int[])> months;

        internal protected HermeticLunarPlot(int year) {
            FindYearBounds(year);
            YearDays = NewMoon[1] - NewMoon[0];
        }

        private void FindYearBounds(int year) {
            //find new moons closest to vernal equinox of this year and next
            for (int y = 0; y < 2; y++) {
                double equinox = Earth.SeasonStart(year + y, Earth.Season.March);
                double priorMoon = Moon.NextPhase(Moon.Phase.NewMoon, equinox - 30);
                double priorDelta = equinox - priorMoon;
                double followMoon = Moon.NextPhase(Moon.Phase.NewMoon, equinox);
                double followDelta = followMoon - equinox;
                NewMoon[y] = priorDelta < followDelta ? ToOffsetDay(priorMoon) : ToOffsetDay(followMoon);
            }
        }

        private void PlotYear() {
            months = new List<(int MonthDays, int[] WeekDays)>();
            int weekStart = NewMoon[0] + 1;
            int nextWeekStart = 0;
            // Find lengths of months and weeks based on moon phase
            do {
                int monthDays = 0;
                int[] weekDays = new int[4];
                for (int w = 0; w < 4; w++) {
                    var moon = Moon.NextPhase(weekStart + 4);
                    nextWeekStart = ToOffsetDay(moon.julianEphemerisDay) + 1;
                    weekDays[w] = nextWeekStart - weekStart;
                    monthDays += weekDays[w];
                    weekStart = nextWeekStart;
                }
                months.Add((monthDays, weekDays));
            } while (nextWeekStart < NewMoon[1]);
        }

        // Convert to TT midnight and adjust for 0600 UTC day start
        private int ToOffsetDay(double jde) => (int)(jde.JulianUniversalDay() + 0.25);
    }
}