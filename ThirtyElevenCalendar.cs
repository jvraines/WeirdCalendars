using System;

namespace WeirdCalendars {
    public class ThirtyElevenCalendar : WeirdCalendar {

        public override string Author => "stephen@abbottpr.com";
        public override Uri Reference => new Uri("https://web.archive.org/web/20211201181419/http://30x11.com/");

        public ThirtyElevenCalendar() => Title = "30x11 Calendar";

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 12 ? 30 : IsLeapYear(year) ? 36 : 35;
        }
    }
}
