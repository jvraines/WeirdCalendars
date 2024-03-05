using System;
using System.Globalization;

namespace WeirdCalendars {
    public class QuintaCalendar : WeirdCalendar {
 
        // Yule is made into its own Month 0.
        // Leap quinta is added to the end of December.
        public override string Author => "Duncan McGregor";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Quinta_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 12, 18);
        protected override int SyncOffset => -1968;

        protected override int FirstMonth => 0;
        protected override int DaysInWeek => 5;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfYear(time) - 1) % DaysInWeek + 1);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 0 ? 5 : month == 12 && IsLeapYear(year) ? 35 : 30;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 370 : 365;
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 13 : 12;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day > 30;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 20 == 0 && year % 640 != 0;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (GetMonth(time) == 0) {
                fx.MonthFullName = "Yule";
                fx.MonthShortName = "Yul";
            }
            FixNegativeYears(fx, time);
            return fx;
        }
    }
}
