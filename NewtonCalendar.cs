using System;

namespace WeirdCalendars {
    public class NewtonCalendar : WeirdCalendar {
        public override string Author => "Isaac Newton";
        public override Uri Reference => new Uri("https://u.cs.biu.ac.il/~belenka/Newton-calendar.pdf");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && (year % 100 != 0 || year % 500 == 0);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            if (year % 5000 == 0 && month == 2) return 30;
            return base.GetDaysInMonth(year, month, era);
        }
    }
}
