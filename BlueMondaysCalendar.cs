using System;
using System.Globalization;

namespace WeirdCalendars {
    public class BlueMondaysCalendar : FixedCalendar {

        public override string Author => "Sonny Pondrom";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Sonny_Pondrom_calendars#Blue_Mondays");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int DaysInWeek => 6;

        public new enum DayOfWeekWC {
            Blankday = -1,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2:
                    return 28;
                case 12:
                    return IsLeapYear(year) ? 32 : 31;
                default:
                    return 31;
            }
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 1 && day == 31 || (month == 4 || month == 6 || month == 8 || month == 10) && day == 2 || IsLeapDay(year, month, day);
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) => "Monday";

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 12 && day == 2;
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = GetDayOfYear(time);
            int s = (d + 30) / 61;
            if (s == 6 && !IsLeapYear(ToLocalDate(time).Year)) s = 5;
            return (d - s - 1) % 6;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, null, null, new string[] { "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "" });
        }
    }
}
