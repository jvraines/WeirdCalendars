using System;
using System.Globalization;

namespace WeirdCalendars {
    public class PaxCalendar : LeapWeekCalendar {

        public override string Author => "James A. Colligan";
        public override Uri Reference => new Uri("https://myweb.ecu.edu/mccartyr/colligan.html");

        protected override DateTime SyncDate => new DateTime(2017, 1, 1);
        protected override int SyncOffset => 0;
        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override DateTime AddYears(DateTime time, int years) {
            ValidateDateTime(time);
            var hh = ToLocalDate(time);
            hh.Year += years;
            if (!IsLeapYear(hh.Year)) {
                if (hh.Month == 14) hh.Month = 13;
                else if (hh.Month == 13) {
                    hh.Month = 12;
                    hh.Day = 28;
                }
            }
            return ToDateTime(hh.Year, hh.Month, hh.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfMonth(time) - 1) % 7);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            return IsLeapMonth(year, month) ? 7 : 28;
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 14 : 13;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 14 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 14;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 100 % 6 == 0 || year % 100 == 99 || (year % 100 == 0 && year % 400 != 0) ;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsLeapYear(ymd.Year)) {
                if (ymd.Month == 13) {
                    fx.MonthFullName = "Pax";
                    fx.MonthShortName = "Pax";
                }
                else if (ymd.Month == 14) {
                    fx.MonthFullName = dtfi.MonthNames[12];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[12];
                }
            }
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            m[12] = m[11];
            m[11] = "Columbus";
            SetNames(dtfi, m);
        }
    }
}