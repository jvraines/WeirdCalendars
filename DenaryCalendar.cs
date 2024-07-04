using System;
using System.Globalization;

namespace WeirdCalendars {
    public class DenaryCalendar : FixedCalendar {
        
        public override string Author => "Hellerick Ferlibay";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Denary_system_of_measurement#The_Denary_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        protected override int FirstMonth => 0;
        protected override int GetFirstDayOfMonth(int year, int month) => 0;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 10;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 0 || month == 1 && IsLeapYear(year) ? 37 : 36;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 1 && day == 36;
        }

        protected override int WeekdayNumber(DateTime time) {
            //Default to Sunday start with fixed months
            return GetDayOfMonth(time) % DaysInWeek;
        }
        
        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day > 34;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return $"Month-end {day - 34}";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return $"M{day - 34}";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Zerober", "Primember", "Secumber", "Tertimber", "Quartember", "Quintember", "Sextember", "September", "October", "November", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC( dtfi, time, format);
            string monthName = dtfi.MonthNames[ToLocalDate(time).Month];
            fx.MonthFullName = monthName;
            fx.MonthShortName = monthName.Substring(0, 3);
            return fx;
        }
    }
}
