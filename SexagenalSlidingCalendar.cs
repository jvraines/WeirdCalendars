using System;
using System.Collections.Generic;
using System.Text;

namespace WeirdCalendars {
    public class SexagenalSlidingCalendar : WeirdCalendar {
        
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Sexagenal_sliding_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 2);
        protected override int SyncOffset => 3849;

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
            int y = year % 62;
            return y % 4 == 0 && y != 60;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 31;
        }
    }
}
