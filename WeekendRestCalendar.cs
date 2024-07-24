using System;
using System.Globalization;

namespace WeirdCalendars {
    public class WeekendRestCalendar : CoalescedWeekendCalendar {

        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Weekend_Rest_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        protected override int StartDay(int year) {
            int c = (year - 1) % 62;
            return (c + (c + 14) / 17) % 6;
        }

        protected override int StartDay(int year, int month) => (StartDay(year) + (month - 1) * 2) % 6;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int d;
            if (month < 12) d = StartDay(year, month) < 4 ? 30 : 31;
            else {
                int s = StartDay(year);
                if (IsLeapYear(year)) {
                    d = s == 2 || s == 3 ? 32 : 33;
                }
                else {
                    d = s == 2 || s == 3 || s == 4 ? 31 : 32;
                }
            }
            return d;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return StartDay(year) == 5 || IsLeapYear(year) ? 366 : 365;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 62 % 17 == 3;
        }

        public override bool IsLeapDay(int year, int month, int date, int era) {
            ValidateDateParams(year, month, ToDay(year, month, date), era);
            return date == 28;
        }
    }
}