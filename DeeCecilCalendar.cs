using System;

namespace WeirdCalendars {
    public class DeeCecilCalendar : WeirdCalendar {

        public override string Author => "John Dee and William Cecil";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/dee-cecil-calendar.html");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int r = year % 33;
            return r > 0 && r % 4 == 0;
        }
    }
}