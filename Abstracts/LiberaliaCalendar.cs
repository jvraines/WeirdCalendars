using System;
using System.Globalization;

namespace WeirdCalendars {

    public abstract class LiberaliaCalendar : WeirdCalendar {
        
        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((ToLocalDate(time).Day - 1) % 3);
        }
        
        /// <summary>
        /// Gets the triday of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer representing the triday number.</returns>
        public int GetTriday(DateTime time) {
            return (int)Math.Ceiling(ToLocalDate(time).Day / 3d);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            FixNegativeYears(fx, time);
            return fx;
        }
    }
}