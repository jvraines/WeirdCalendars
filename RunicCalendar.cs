using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;

namespace WeirdCalendars {
    public class RunicCalendar : WeirdCalendar {
 
        public override string Author => "Traditional";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/Runic_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) => (time.Year, time.Month, time.Day, time.TimeOfDay);

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) => new DateTime(year, month, day, hour, minute, second, millisecond);

        private int GoldenNumber(int year) => year % 19 + 1;

        private static Dictionary<int, DateTime> YearStart = new Dictionary<int, DateTime>();

        private DateTime GetYearStart(int year) {
            if (!YearStart.TryGetValue(year, out DateTime start)) {
                double solstice = Earth.SeasonStart(year - 1, Earth.Season.December);
                double firstNew = Moon.NextPhase(Moon.Phase.NewMoon, solstice);
                double firstFull = Moon.NextPhase(Moon.Phase.FullMoon, firstNew + 10);
                start = firstFull.ToDateTime().Date;
                YearStart.Add(year, start);
            }
            return start;
        }

        private static string futhark = "ᚠᚢᚦᚬᚱᚴᚼᚾᛁᛅᛋᛏᛒᛚᛘᛦᛮᛯᛰ";

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            DateTime yearStart;
            int y = time.Year;
            do {
                yearStart = GetYearStart(y);
                y--;
            }
            while (yearStart > time);
            fx.Format = futhark.Substring((int)(time - yearStart).TotalDays % 7, 1);
            double jd = time.Date.JulianDay();
            double fm = Moon.NextPhase(Moon.Phase.FullMoon, jd);
            if (fm - jd < 1) fx.Format += $" | {futhark.Substring(GoldenNumber(time.Year) - 1, 1)}";
            return fx;
        }
    }
}
