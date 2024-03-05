using System;
using System.Globalization;

namespace WeirdCalendars {
    public class Calendar13Calendar : WeirdCalendar {
        
        public override string Author => "Cody Jassman";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Calendar_13");

        protected override DateTime SyncDate => new DateTime(2020, 12, 21);
        protected override int SyncOffset => 1;

        public Calendar13Calendar() => Title = "Calendar 13";
        
        protected override int GetFirstDayOfMonth(int year, int month) {
            return month == 1 ? 0 : 1;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 1 || month == 13 && IsLeapYear(year) ? 29 : 28;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 29 && month == 13 && IsLeapYear(year);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            for (int i = 1; i < 14; i++) m[i - 1] = i.ToRoman();
            SetNames(dtfi, m);
        }
    }
}
