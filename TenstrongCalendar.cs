using System;
using System.Globalization;

namespace WeirdCalendars {
    public class TenstrongCalendar : WeirdCalendar {
        
        public override string Author => "David Mitchell";
        public override Uri Reference => new Uri("http://tenstrongcalendar.blogspot.com/");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;

        public enum DayOfWeekWC {
            Coexistence,
            Intention,
            Sustenance,
            Tenancy,
            Intensity,
            Attention,
            Enlightenment,
            Tenacity,
            Contentment,
            Tenstrong
        }

        protected override int DaysInWeek => 10;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int wd = WeekdayNumber(time);
            if (wd > 6) throw BadWeekday;
            return (DayOfWeek)wd;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        private int WeekdayNumber(DateTime time) {
            //Default to "Sunday" start with fixed months
            return (GetDayOfMonth(time) - 1) % DaysInWeek;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 30 : IsLeapYear(year) ? 6 : 5;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            m[12] = "Transition";
            ma[12] = "Tra";
            SetNames(dtfi, m, ma, new string[] { "Coexistence", "Intention", "Sustenance", "Tenancy", "Intensity", "Attention", "Enlightenment" }, new string[] { "Coe", "Itn", "Sus", "Tcy", "Ity", "Att", "Enl"});
        }

        private string[] XtraAbbrs = { "Tty", "Con", "Ten" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var d = WeekdayNumber(time);
            if (d > 6) {
                fx.DayFullName = ((DayOfWeekWC)d).ToString();
                fx.DayShortName = XtraAbbrs[d - 7];       
            }
            return fx;
        }
    }
}
