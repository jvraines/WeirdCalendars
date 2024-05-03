using System;

namespace WeirdCalendars {
    public class PregordianCalendar : WeirdCalendar {
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Pregordian_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 98401;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 2:
                    return 29;
                case 12:
                    return IsLeapYear(year) ? 31 : 30;
                default:
                    return base.GetDaysInMonth(year, month, era);
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0 && year % 1600 !=0 || year % 25600 == 0 && year % 102400 != 0);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 31;
        }
    }
}
