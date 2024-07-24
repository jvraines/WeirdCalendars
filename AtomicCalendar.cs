using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class AtomicCalendar : WeirdCalendar {
        
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Atomic_Calendar");

        protected override DateTime SyncDate => new DateTime(1904, 1, 1);
        protected override int SyncOffset => 0;

        private const double HourFactor = 414416000000 / 131323311d / 3600d;   //Length of an atomic hour in hours.
        private const double YearLength = 365.242198;     //Length of year in days.
        private TimeSpan partialBoundary = TimeSpan.FromHours(10 * HourFactor);   //Time after which to add 1 day to a partial day.

        private static Dictionary<int, (int, int[])> YearPlot = new Dictionary<int, (int, int[])>();
        private static int[] MonthHours = { 850, 770, 850, 820, 850, 820, 850, 850, 820, 850, 820, 850 };

        private (int Days, int[] Month) GetYearPlot(int year) {
            if (!YearPlot.TryGetValue(year, out var p)) {
                int[] month = new int[12];
                int days = 0;
                DateTime Adjust(DateTime d) => d.TimeOfDay > partialBoundary ? d.Date.AddDays(1) : d.Date;
                DateTime monthStart = SyncDate.AddDays((year - SyncDate.Year) * YearLength);
                for (int m = 0; m < 12; m++) {
                    DateTime monthEnd = monthStart.AddHours(MonthHours[m] * HourFactor);
                    month[m] = (int)(Adjust(monthEnd) - Adjust(monthStart)).TotalDays;
                    days += month[m];
                    monthStart = monthEnd;
                }
                p = (days, month);
                YearPlot.Add(year, p);
            }
            return p;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetYearPlot(year).Month[month - 1];
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetYearPlot(year).Days;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        protected override long HourTicks => (long)(TimeSpan.TicksPerDay / 24 * HourFactor);
        protected override long MinuteTicks => HourTicks / 50;
        protected override long SecondTicks => MinuteTicks / 50;
        protected override long MilliTicks => SecondTicks / 1000;

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
