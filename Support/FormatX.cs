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

        internal FormatWC(string format, DateTimeFormatInfo dtfi) {
            Format = format;
            LongDatePattern = dtfi.LongDatePattern;
            ShortDatePattern = dtfi.ShortDatePattern;
            LongTimePattern = dtfi.LongTimePattern;
            ShortTimePattern = dtfi.ShortTimePattern;
        }
    }

}
