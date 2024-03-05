using System;
using System.Globalization;

namespace WeirdCalendars {
    public class CommonCalendar : LeapWeekCalendar {
        
        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Common_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfYear(time) - 1) % 7);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            return IsLeapMonth(year, month) ? 7 : month % 3 == 2 ? 29 : 31;
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 7 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 7 && IsLeapYear(year);
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year - 2007) * 1.242199 % 7 < 1.242199;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            if (IsLeapYear(ymd.Year)) {
                if (ymd.Month == 7) {
                    fx.MonthFullName = "Intercalaris";
                    fx.MonthShortName = "Int";
                }
                else if (ymd.Month > 7) {
                    fx.MonthFullName = dtfi.MonthNames[ymd.Month - 2];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[ymd.Month - 2];
                }
            }
            return fx;
        }
    }
}
