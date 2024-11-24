using System;
using System.Globalization;

namespace WeirdCalendars {
    public class KlingonCalendar : FixedCalendar {
        public override string Author => "Christopher L. Bennett et al.";
        public override Uri Reference => new Uri("https://christopherlbennett.wordpress.com/home-page/star-trek-fiction/dti-watching-the-clock/dti-calendar-notes/");

        protected override DateTime SyncDate => new DateTime(2024, 4, 23, 17, 24, 0);
        protected override int SyncOffset => -1373;

        public override int DaysInWeek => 8;
        public new enum DayOfWeekWC {
            Blank = -1,
            jaj_wa,
            DaSjaj,
            povjaj,
            ghItlhjaj,
            loghjaj,
            buqjaj,
            ghInjaj,
            lojmItjaj,
        }
        
        protected override double TimescaleFactor => 0.845;

        protected override long HourTicks => TimeSpan.TicksPerDay / 16;
        protected override long MinuteTicks => TimeSpan.TicksPerDay / 768;
        protected override long SecondTicks => TimeSpan.TicksPerDay / 36864;
        protected override long MilliTicks => TimeSpan.TicksPerDay / 3686400;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 9;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 1 && IsLeapYear(year) ? 49 : 48;
        }

        protected override int GetFirstDayOfMonth(int year, int month) => month == 1 && IsLeapYear(year) ? 0 : 1;

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 433 : 432;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 5 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 0;
        }
        
        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) => "latlhjaj";

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            var ymd = ToLocalDate(time);
            int w = WeekdayNumber(time);
            if (w == 7 || IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            return (DayOfWeek)WeekdayNumber(time);
        }

        public new DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ymd = ToLocalDate(time);
            int w = IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day) ? -1 : WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }

        protected override int WeekdayNumber(DateTime time) => GetDayOfMonth(time) % 8;

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "A’Kahless", "Jo’vos", "Soo’jen", "Lo’Bral", "Maktag", "Merruthj", "Doqath", "Xan’lahr", "nay’Poq", "", "", "", "" }, null, new string[] {"jaj wa'", "DaSjaj", "povjaj", "ghItlhjaj", "loghjaj", "buqjaj", "ghInjaj"});
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.DayShortName = IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day);
            }
            else {
                if (WeekdayNumber(time) == 7) {
                    fx.DayFullName = "lojmItjaj";
                    fx.DayShortName = "loj";
                }
            }
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
