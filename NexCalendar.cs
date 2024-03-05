using System;

namespace WeirdCalendars {
    public class NexCalendar : WeirdCalendar {
        
        public override string Author => "Donny C.F. Lai and Justin J.S. Lai";
        public override Uri Reference => new Uri("https://web.archive.org/web/20220409035511/http://www.nexcalendar.org/");

        protected override DateTime SyncDate => new DateTime(2018, 1, 1);
        protected override int SyncOffset => 0;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int d = GetDayOfYear(time);
            return d == 366 ? DayOfWeek.Sunday : (DayOfWeek)((d - d / 245) % 7);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                    return 31;
                case 2:
                    return 29;
                case 12:
                    return IsLeapYear(year) ? 31 : 30;
                default:
                    return 30;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 12 && day == 31;
        }
    }
}
