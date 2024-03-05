using System;
using System.Globalization;

namespace WeirdCalendars {
    public class TwelveAnd12Calendar : WeirdCalendar {

        public override string Author => "Peter Zimmer";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/12%2612");

        protected override DateTime SyncDate => new DateTime(2024, 1, 6);
        protected override int SyncOffset => 0;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public TwelveAnd12Calendar() => Title = "12 & 12 Calendar";

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 2 ? IsLeapYear(year) ? 30 : 29 : month == 13 ? 11 : (month & 1) == 1 ? 30 : 29;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 2 && day == 30;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] mA = (string[])dtfi.AbbreviatedMonthNames.Clone();
            m[12] = "Holy Nights";
            mA[12] = "Hol";
            SetNames(dtfi, m, mA);
        }
    }
}
