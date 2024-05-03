using System;
using System.Globalization;

namespace WeirdCalendars {
    public class MonotonicCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/The_Monotonic_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 12, 26);
        protected override int SyncOffset => 1;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 372 : 365;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 2:
                    return 28;
                case 13:
                    return 7;
                default:
                    return base.GetDaysInMonth(year, month, era);
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            const decimal l = 365.241904M;
            return year == (int)Math.Round(Math.Ceiling((year - 1) * (l - 365) / 7) * 7 / (l - 365));
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 13;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = dtfi.MonthNames;
            string[] mA = dtfi.AbbreviatedMonthNames;
            m[12] = "Good Resolutions Week";
            mA[12] = "GRW";
            SetNames(dtfi, m, mA);
        }
    }
}
