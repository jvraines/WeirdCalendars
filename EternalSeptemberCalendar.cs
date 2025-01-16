using System;

namespace WeirdCalendars {
    public class EternalSeptemberCalendar : WeirdCalendar {
        
        public override string Author => "Various";
        public override Uri Reference => new Uri("http://www.catb.org/~esr/jargon/html/S/September-that-never-ended.html");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override CalendarRealization Realization => CalendarRealization.Current;

        private DateTime baseDate = new DateTime(1993, 9, 1);

        protected override void ValidateDateParams(params int[] param) {
            if (param[0] != 1993) throw new ArgumentOutOfRangeException("year", $"Calendar year must always be 1993. Year {param[0]} was passed.");
            base.ValidateDateParams(param);
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return year == 1993 ? 9 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return year == 1993 && month == 9 ? 2924253 : base.GetDaysInMonth(year, month, 0);
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return year == 1993 ? 2924496 : base.GetDaysInYear(year, 0);
        }

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            ValidateDateTime(time);
            if (time <= baseDate) return (time.Year, time.Month, time.Day, time.TimeOfDay);
            else return (1993, 9, (int)(time - baseDate).TotalDays + 1, time.TimeOfDay);
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            ValidateDateParams(year, month, day, era);
            if (year <= 1993 && month <= 9 && day <= 30) return new DateTime(year, month, day, hour, minute, second, millisecond);
            else {
                DateTime d = baseDate.AddDays(day - 1);
                return new DateTime(d.Year, d.Month, d.Day, hour, minute, second, millisecond);
            }
        }
    }
}
