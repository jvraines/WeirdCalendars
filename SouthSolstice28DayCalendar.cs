using System;
using System.Globalization;

namespace WeirdCalendars {
    public class SouthSolstice28DayCalendar : LeapWeekCalendar {
        
        public override string Author => "Michael Ossipoff";
        public override Uri Reference =>  new Uri("https://calendars.fandom.com/wiki/South-Solstice_Equal_28-Day_Months_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 12, 21);
        protected override int SyncOffset => 1;

        public SouthSolstice28DayCalendar() => Title = "South-Solstice Equal 28-Day Months Calendar";

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)(ToLocalDate(time).Day % 7);
        }
        
        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13 && IsLeapYear(year) ? 35 : 28;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return Math.Abs((year - 2017) % (7 / 1.2422)) < 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Unua", "Dua", "Tria", "Kvara", "Kvina", "Sesa", "Sepa", "Oka", "Nua", "Deka", "DekUnua", "DekDua", "DekTria" });
        }
    }
}
