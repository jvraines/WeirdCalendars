using System;
using System.Globalization;

namespace WeirdCalendars {
    public class HankeHenryCalendar : LeapWeekCalendar {

        protected override DateTime SyncDate => new DateTime(2018, 1, 1);
        protected override int SyncOffset => 0;

        public override string Author => "Steve Hanke and Richard Henry";
        public override Uri Reference => new Uri("http://hankehenryontime.com/html/calendar.html");

        public HankeHenryCalendar() => Title = "Hanke-Henry Permanent Calendar";

        public override DateTime AddYears(DateTime time, int years) {
            ValidateDateTime(time);
            var hh = ToLocalDate(time);
            hh.Year += years;
            if (hh.Month == 13 && !IsLeapYear(hh.Year)) {
                hh.Month = 12;
                hh.Day = 31;
            }
            return ToDateTime(hh.Year, hh.Month, hh.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            return IsLeapMonth(year, month) ? 7 : month % 3 == 0 ? 31 : 30;
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return new DateTime(year, 1, 1).DayOfWeek == DayOfWeek.Thursday || new DateTime(year, 12, 31).DayOfWeek == DayOfWeek.Thursday; 
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            string[] m = (string[])dtfi.MonthNames.Clone();
            m[12] = "Extra";
            string[] a = (string[])dtfi.AbbreviatedMonthNames.Clone();
            a[12] = "Xtr";
            SetNames(dtfi, m, a);
        }
    }
}