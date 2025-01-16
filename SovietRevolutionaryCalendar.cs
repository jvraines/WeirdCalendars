using System;
using System.Globalization;

namespace WeirdCalendars {
    public class SovietRevolutionaryCalendar : FixedCalendar {

        public new enum DayOfWeekWC {
            Blankday = -1,
            Yellow,
            Pink,
            Red,
            Purple,
            Green
        }
        
        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Soviet_revolutionary_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override CalendarRealization Realization => CalendarRealization.Archaic;

        public override int DaysInWeek => 5;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 1:
                    return 31;
                case 2:
                    return IsLeapYear(year) ? 31 : 30;
                case 4:
                case 11:
                    return 32;
                default:
                    return 30;
            }
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day > 30;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            switch(month) {
                case 1:
                    return "Lenin Day";
                case 4:
                    return $"{RomanOrdinal(day)} Labor Day";
                case 11:
                    return $"{RomanOrdinal(day)} Industry Day";
                case 2:
                    return "Leap Day";
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string RomanOrdinal(int d) => (d - 30).ToRoman();
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, null, null, new string[] { "Yellow", "Pink", "Red", "Purple", "Green", "", "" });
        }
    }
}
