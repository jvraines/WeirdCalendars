using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class TesserodsCalendar : CoalescedWeekendCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Tesserods_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 30);
        protected override int SyncOffset => 1;

        protected override int StartDay(int year) {
            int y = year % 93;
            if (y < 3) return (y & 1) == 0 ? 3 : 0;
            return (year - 3) % 6;
        }

        protected override int StartDay(int year, int month) => (month - 1 + StartDay(year) - (month - 1) / 4) % 6;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month % 4 > 0) return StartDay(year, month) == 5 ? 30 : 29;
            return month < 12 || IsLeapYear(year) ? 35 : StartDay(year, month) < 2 ? 32 : 33;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            int y = year % 93;
            if (y < 3) return (y & 1) == 0 ? 368 : 367;
            return StartDay(year) == 5 ? 366 : 365;
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
