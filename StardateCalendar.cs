using System;
using System.Globalization;

namespace WeirdCalendars {
    public class StardateCalendar : WeirdCalendar {
        
        public override string Author => "Gene Roddenberry et al.";
        public override Uri Reference => new Uri("https://memory-alpha.fandom.com/wiki/Stardate#The_Next_Generation_era");

        protected override DateTime SyncDate => new DateTime(2323, 1, 1);
        protected override int SyncOffset => -2323;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override string Notes => "ST:TNG timeline according to Michael and Denise Okuda.";

        public StardateCalendar() => Title = "Stardate";

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            long timeTicks = new TimeSpan(GetDayOfYear(time) - 1, time.Hour, time.Minute, time.Second, time.Millisecond).Ticks;
            long yearTicks = GetDaysInYear(time.Year) * TimeSpan.TicksPerDay;
            string datePart = $"{time.Year + SyncOffset}{(int)((double)timeTicks / yearTicks * 1000):000}";
            string timePart = $"{(double)time.TimeOfDay.Ticks / TimeSpan.TicksPerDay:.00}";
            fx.LongDatePattern = datePart;
            fx.ShortDatePattern = datePart;
            fx.LongTimePattern = timePart;
            fx.ShortTimePattern = timePart;
            if ("FfGgU".Contains(format)) fx.Format = $"{datePart}{timePart}";
            return fx;
        }

    }
}
