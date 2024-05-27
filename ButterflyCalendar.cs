using System;
using System.Globalization;

namespace WeirdCalendars {
    public class ButterflyCalendar : CoalescedWeekendCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Butterfly_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 30);
        protected override int SyncOffset => 1;

        protected override int StartDay(int year) => (year + 3) % 6;

        protected override int StartDay(int year, int month) => (StartDay(year) + month - 1) % 6;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int d = month < 13 ? 29 : IsLeapYear(year) ? 22 : 15;
            return d + (StartDay(year, month) == 5 ? 1 : 0);
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 372 : StartDay(year) == 5 ? 366 : 365;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 93 == 0;
        }

        public override bool IsLeapDay(int year, int month, int date, int era) {
            ValidateDateParams(year, month, ToDay(year, month, date), era);
            return IsLeapYear(year) && month == 13 && date > 13;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            SetNames(dtfi, new string[] { "Januar", "Februar", "Mar", "Pupar", "Tersar", "Junar", "Quintar", "Sixar", "Septar", "Octar", "Novar", "Neccar", "Farfar" });
        }
    }
}
