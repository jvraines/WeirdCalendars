using System;

namespace WeirdCalendars {
    public class NepturanCalendar : FixedCalendar {
        
        public new enum DayOfWeekWC {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Uranday,
            Nepday
        }

        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Nepturan_Calendar_Rule");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;
        
        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 1);
            return month == 2 && day > 27;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 28 ? "Uranday" : "Nepday";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 28 ? "Uran" : "Nep";
        }

        public new DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) return ymd.Day == 28 ? DayOfWeekWC.Uranday : DayOfWeekWC.Nepday;
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = GetDayOfYear(time);
            if (time.Month > 2) {
                var ymd = ToLocalDate(time);
                d -= IsLeapYear(ymd.Year) ? 2 : 1;
            }
            return (d - 1) % DaysInWeek;
        }
    }
}
