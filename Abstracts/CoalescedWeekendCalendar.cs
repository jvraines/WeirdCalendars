using System;

namespace WeirdCalendars {
    public abstract class CoalescedWeekendCalendar : WeirdCalendar {

        // 0 = Monday
        protected abstract int StartDay(int year);
        protected abstract int StartDay(int year, int month);

        protected int ToDay(int year, int month, int date) {
            int d = Math.Abs(date);
            return d + (d + StartDay(year, month)) / 6;
        }

        public override int GetDayOfMonth(DateTime time) {
            var ymd = ToLocalDate(time);
            return ymd.Day - (ymd.Day + StartDay(ymd.Year, ymd.Month)) / 7;
        }

        /// <param name="date">Negative to indicate that Sunday should be returned from a weekend value.</param>
        /// <returns>A sequential day Monday-Saturday. If <paramref name="date"/> is passed as a negative number, then Sunday is returned from a weekend date.</returns>
        public override DateTime ToDateTime(int year, int month, int date, int hour, int minute, int second, int millisecond, int era) {
            DateTime d = base.ToDateTime(year, month, ToDay(year, month, date), hour, minute, second, millisecond, era);
            return date > 0 && GetDayOfWeek(d) == DayOfWeek.Sunday ? d.AddDays(-1) : d;
        }
    }
}
