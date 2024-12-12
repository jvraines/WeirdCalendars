using System.Globalization;

namespace WeirdCalendars {

    internal class FormatWC {
        internal string Format { get; set; }
        internal string LongDatePattern { get; set; }
        internal string ShortDatePattern { get; set; }
        internal string LongTimePattern { get; set; }
        internal string ShortTimePattern { get; set; }
        internal string MonthFullName { get; set; }
        internal string MonthShortName { get; set; }
        internal string DayFullName { get; set; }
        internal string DayShortName { get; set; }
        internal string DateAndTimeSeparator { get; set; }

        internal FormatWC(string format, DateTimeFormatInfo dtfi) {
            Format = format;
            LongDatePattern = dtfi.LongDatePattern;
            ShortDatePattern = dtfi.ShortDatePattern;
            LongTimePattern = dtfi.LongTimePattern;
            ShortTimePattern = dtfi.ShortTimePattern;
            DateAndTimeSeparator = " ";
            WeirdCalendar c = (WeirdCalendar)dtfi.Calendar;
            if (c.DaysInWeek == 0) {
                LongDatePattern = ZapWeekDays(LongDatePattern);
                ShortDatePattern = ZapWeekDays(ShortDatePattern);
                Format = ZapWeekDays(Format);
            }

            string ZapWeekDays(string s) => s.ReplaceUnescaped("dddd, ", "").ReplaceUnescaped("ddd, ", "").ReplaceUnescaped("dddd", "").ReplaceUnescaped("ddd", "");
        }
    }
}
