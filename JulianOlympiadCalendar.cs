using AA.Net;
using System;
using System.Globalization;

namespace WeirdCalendars {
    public class JulianOlympiadCalendar : WeirdCalendar {
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Julian_Olympiad_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) => (time.Year, time.Month, time.Day, time.TimeOfDay);

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) => new DateTime(year, month, day, hour, minute, second, millisecond);

        public int GetJulianOlympiad(DateTime time) {
            return (int)(time.JulianDay() / 1461);
        }

        public double GetOlympiadDay(DateTime time) {
            double jdd = time.JulianDay() / 1461;
            return jdd % 1 * 1461;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            fx.Format = $"{GetJulianOlympiad(time)}:{GetOlympiadDay(time):0.000}";
            return fx;
        }
    }
}
