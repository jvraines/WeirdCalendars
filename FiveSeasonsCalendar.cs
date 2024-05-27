using System;
using System.Globalization;

namespace WeirdCalendars {
    public class FiveSeasonsCalendar : WeirdCalendar {
 
        public override string Author => "Stavros Daliakopoulos";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Five_Seasons_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;
        
        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 5;
        }

        protected override int GetRealDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 5 ? GetDaysInMonth(year, month, era) : IsLeapYear(year) ? 86 : 85;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 3 ? 28 : 84;
        }

        public override int GetDayOfMonth(DateTime time) => Math.Min(ToLocalDate(time).Day, 84);

        protected override int GetRealDaysInYear(int year, int era) => base.GetDaysInYear(year, era);

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 365;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) => (DayOfWeek)(GetDayOfMonth(time) % 7);

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override int GetHour(DateTime time) {
            var ymd = ToLocalDate(time);
            int hour = ymd.TimeOfDay.Hours;
            if (ymd.Day > 84) hour += (ymd.Day - 84) * 24;
            return hour;
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            ValidateDateParams(year, month, day, era);
            if (day == 84 && hour > 23) {
                day += hour / 24;
                hour %= 24;
                ValidationEnabled = false;
            }
            return base.ToDateTime(year, month, day, hour, minute, second, millisecond, era);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "First Period", "Second Period", "Third Period", "Fourth Period", "Fifth Period", "", "", "", "", "", "", "", "" }, new string[] { "1st", "2nd", "3rd", "4th", "5th", "", "", "", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            if (ymd.Day > 83 && ymd.Month == 5) {
                CustomizeTimes(fx, time);
                string s = IsLeapYear(ymd.Year) ? "Triple" : "Double";
                fx.DayFullName = $"{s} Sunday";
                fx.DayShortName = $"{s.Substring(0, 1)}S";
            }
            return fx;
        }
    }
}
