using System;
using System.Collections.Generic;
using System.Text;

namespace WeirdCalendars {
    public class TesserodsCalendar : CoalescedWeekendCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Tesserods_Calendar");

        protected override DateTime SyncDate => new DateTime(2045, 12, 28);
        protected override int SyncOffset => 1;

        protected override int StartDay(int year) => IsLeapYear(year) ? (year & 1) == 0 ? 3 : 0 : (year - 3) % 6;

        protected override int StartDay(int year, int month) => (StartDay(year) + month - 1) % 6;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (year % 4 < 3) return StartDay(year, month) == 5 ? 30 : 29;
            if (month < 12 || IsLeapYear(year)) return 35;
            return StartDay(year, month) < 2 ? 32 : 33;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);

        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 93 < 3;
        }

        public override bool IsLeapDay(int year, int month, int date, int era) {
            ValidateDateParams(year, month, ToDay(year, month, date), era);
            return month == 12 && date > 28;
        }
    }
}
