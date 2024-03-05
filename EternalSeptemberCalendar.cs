using System;

namespace WeirdCalendars {
    public class EternalSeptemberCalendar : WeirdCalendar {
        
        public override string Author => "Various";
        public override Uri Reference => new Uri("http://www.catb.org/~esr/jargon/html/S/September-that-never-ended.html");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override DateTime MinSupportedDateTime => new DateTime(1993, 9, 1);

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            ValidateDateTime(time);
            return (time.Year, time.Month, time.Day, time.TimeOfDay);
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            ValidateDateParams(year, month, day, era);
            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        public override int GetYear(DateTime time) {
            return 1993;
        }
        public override int GetMonth(DateTime time) {
            return 9;
        }

        public override int GetDayOfMonth(DateTime time) {
            return (int)(time - MinSupportedDateTime).TotalDays + 1;
        }
    }
}
