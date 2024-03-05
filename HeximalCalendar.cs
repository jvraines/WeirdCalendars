using System;
using System.Globalization;

namespace WeirdCalendars {
    public class HeximalCalendar : WeirdCalendar {
        
        public override string Author => "Tab Atkins-Bittner";
        public override Uri Reference => new Uri("https://xanthir.com/b54c0");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;

        protected override int DaysInWeek => 6;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int w = (GetDayOfMonth(time) - 1) % 6;
            return (DayOfWeek)w;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 10;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 10 ? 36 : IsLeapYear(year) ? 42 : 41;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 42;
        }

        protected override long HourTicks => TimeSpan.TicksPerDay / 36;
        protected override long MinuteTicks => HourTicks / 36;
        protected override long SecondTicks => MinuteTicks / 36;
        protected override long MilliTicks => SecondTicks / 1000;

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            for (int i = 6; i < 11; i++) {
                m[i] = m[i + 2];
                ma[i] = ma[i + 2];
            }
            SetNames(dtfi, m, ma, new string[] { "Sunday", "Monday", "Vensday", "Marsday", "Joday", "Saturday", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time, null, 6, 18);
            string hexDay = ToLocalDate(time).Day.ToBase(6);
            string hexDay2 = hexDay.PadLeft(2, '0');
            fx.LongDatePattern = FixDigits(dtfi.LongDatePattern, null, null, null, null, hexDay2, hexDay);
            fx.ShortDatePattern = FixDigits(dtfi.ShortDatePattern, null, null, null, null, hexDay2, hexDay);
            fx.Format = FixDigits(format, null, null, null, null, hexDay2, hexDay);
            return fx;
        }
    }
}
