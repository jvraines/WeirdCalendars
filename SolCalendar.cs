using System;

namespace WeirdCalendars {
    public class SolCalendar : WeirdCalendar {
    
        public override string Author => "Jim Eikner";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Sol_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 13 && day == 30 && IsLeapYear(year);
        }

        internal override void CustomizeDTFI(System.Globalization.DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            for (int i = 12; i > 6; i--) {
                m[i] = m[i - 1];
            }
            m[6] = "Sol";
            SetNames(dtfi, m);
        }
    }
}
