
using System;
using System.Globalization;

namespace WeirdCalendars {
    public class KalentrisCalendar : WeirdCalendar {

        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Kalentris");

        protected override DateTime SyncDate => new DateTime(2023, 8, 31);
        protected override int SyncOffset => 1012;

        public KalentrisCalendar() => Title = "Kalentris";
        public override string Notes => "Time in hours, comes, and ticks.";

        protected override int DaysInWeek => 9;

        public enum DayOfWeekWC {
            Oneday,
            Twoday,
            Threeday,
            Fourday,
            Fiveday,
            Sixday,
            Sevenday,
            Eightday,
            Nineday
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int d = (ToLocalDate(time).Day - 1) % 9;
            if (d > 6) throw BadWeekday;
            return (DayOfWeek)d;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return (DayOfWeekWC)((ToLocalDate(time).Day - 1) % 9);
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 9;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 9 && IsLeapYear(year) ? 30 : 27;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 246 : 243;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day > 27;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 6 == 0 && year % 600 != 0;
        }

        protected override long HourTicks => TimeSpan.TicksPerDay / 27;
        protected override long MinuteTicks => HourTicks / 81;
        protected override long SecondTicks => MinuteTicks / 81;

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            fx.MonthFullName = $"Month-{ld.Month}";
            fx.MonthShortName = $"M{ld.Month}";
            int d = (int)GetDayOfWeekWC(time) + 1;
            fx.DayFullName = $"Day-{d}";
            fx.DayShortName = $"D{d}";
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
