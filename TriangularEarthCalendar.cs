using System;
using System.Globalization;

namespace WeirdCalendars {
    public class TriangularEarthCalendar : WeirdCalendar {
        
        public override string Author => "DeWayne Lehman";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Triangular_Earth_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => -2001;

        protected override int FirstMonth => 0;
        protected override int GetFirstDayOfMonth(int year, int month) => 0;
        public override int DaysInWeek => 6;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 11;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 10 ? 36 : IsLeapYear(year) ? 6 : 5;
        }

        public override bool IsLeapYear(int year, int era) {
            return base.IsLeapYear(year + 2001, 1);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 10 && day == 5;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            fx.Format = $"{ld.Year}.{(ld.Month < 10 ? ld.Month.ToString() : "A")}.{ld.Day}:{ld.TimeOfDay.Ticks / (double)TimeSpan.TicksPerDay * 100000:00000}";
            return fx;
        }
    }
}
