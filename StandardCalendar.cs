using System;

namespace WeirdCalendars {
    public class StandardCalendar : WeirdCalendar {
        
        public override string Author => "Various";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Standard_Calendar");

        protected override DateTime SyncDate => new DateTime(2020, 1, 2);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 2:
                case 4:
                case 6:
                case 7:
                case 9:
                case 11:
                    return 30;
                case 12:
                    return IsLeapYear(year) ? 31 : 30;
                default:
                    return 31;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 31 && month == 12 && IsLeapYear(year);
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year * 159 + 522) % 656 < 159;
        }
    }
}
