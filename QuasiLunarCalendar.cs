using System;
using System.Globalization;

namespace WeirdCalendars {
    public class QuasiLunarCalendar : WeirdCalendar {
        
        public override string Author => "Shriramana Sharma";
        public override Uri Reference => new Uri("https://web.archive.org/web/20061014114120/http://samvit.org/calendar/qlc.html");

        protected override DateTime SyncDate => new DateTime(2024, 12, 11);
        protected override int SyncOffset => 1;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public QuasiLunarCalendar() => Title = "Quasi-Lunar Calendar";

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 30;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 390 : 360;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            // L and C from https://groups.io/g/calendars/attachment/1459/0/symiso.frm
            int M = 30;
            int L = 71;
            int C = 293;
            int P = 365 % M * C + L;
            return year * P % (M * C) < P;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 0;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            m[12] = $"{m[11]} II";
            ma[12] = $"{ma[11].Substring(0,1)}II";
            SetNames(dtfi, m, ma);
        }
    }
}
