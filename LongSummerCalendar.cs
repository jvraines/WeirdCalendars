using System;
using System.Globalization;

namespace WeirdCalendars {
    public class LongSummerCalendar : FixedCalendar {
        
        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Long_Summer_Modified_Gregorian_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int DaysInWeek => 5;

        public new enum DayOfWeekWC {
            Blank = -1,
            Oneday,
            Twoday,
            Threeday,
            Fourday,
            Fiveday
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 3:
                case 4:
                case 5:
                case 6:
                case 8:
                    return 31;
                case 7:
                    return IsLeapYear(year) ? 32 : 31;
                case 1:
                    return 29;
                default:
                    return 30;
            }
        }

        public override bool IsIntercalaryDay(int year, int month, int day) => IsLeapDay(year, month, day);

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 32;
        }

        protected override int WeekdayNumber(DateTime time) {
            int y = ToLocalDate(time).Year;
            int d = GetDayOfYear(time); 
            if (IsLeapYear(y) && d > 215) d--;
            return (d - 1) % 5;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, null, null, new string[] { "Oneday", "Twoday", "Threeday", "Fourday", "Fiveday", "", "" });
        }
    }
}
