using System;
using System.Globalization;
using System.Linq;

namespace WeirdCalendars {
    public class AndorianCalendar : WeirdCalendar {
 
        public override string Author => "Christopher L. Bennett";
        public override Uri Reference => new Uri("https://christopherlbennett.wordpress.com/home-page/star-trek-fiction/dti-watching-the-clock/dti-calendar-notes/");

        protected override DateTime SyncDate => new DateTime(2023, 11, 16, 20, 9, 35);
        protected override int SyncOffset => -1701;

        public enum DayOfWeekWC {
            First,
            Second,
            Penultimate,
            Final
        }

        protected override double TimescaleFactor => 4.69;

        protected override long HourTicks => TimeSpan.TicksPerDay / 4; // "Phases" -- there are no further subdivisions

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 18;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 4;
        }

        private int WeekdayNumber(DateTime time) => (ToLocalDate(time).Day - 1) % 4;

        public override DayOfWeek GetDayOfWeek(DateTime time) => (DayOfWeek)WeekdayNumber(time);

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) => (DayOfWeekWC)WeekdayNumber(time);

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 72;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        private static string[] Moons = new string[] { "Firstmoon", "Secondmoon", "Thirdmoon", "Fourthmoon", "Fifthmoon", "Sixthmoon", "Seventhmoon", "Eighthmoon", "Ninthmoon", "Tenthmoon", "Eleventhmoon", "Twelfthmoon", "Thirteenthmoon", "Fourteenthmoon", "Fifteenthmoon", "Sixteenthmoon", "Seventeenthmoon", "Eighteenthmoon" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] ma = new string[13];
            for (int i = 0; i < 13; i++) ma[i] = i.ToOrdinal();
            SetNames(dtfi, Moons.Take(13).ToArray(), ma, new string[] {"First", "Second", "Penultimate", "Final", "", "", ""});
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            string hour = (ymd.TimeOfDay.Ticks / (double)HourTicks).ToString("0.000");
            fx.LongTimePattern = hour;
            fx.ShortTimePattern = hour;
            fx.Format = format.ReplaceUnescaped("hh", hour).ReplaceUnescaped("h", hour).ReplaceUnescaped("m", "").ReplaceUnescaped("s", ""); 
            if (ymd.Month > 13) {
                fx.MonthFullName = Moons[ymd.Month - 1];
                fx.MonthShortName = ymd.Month.ToOrdinal();
            }
            return fx;
        }
    }
}
