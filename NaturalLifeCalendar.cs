using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NaturalLifeCalendar : WeirdCalendar {
        
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Natural_Life_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 3, 14);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month % 3) {
                case 1:
                case 2:
                    return 31;
                default:
                    return month < 10 ? 29 : IsLeapYear(year) ? 31 : 30;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 31;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            int c = ymd.Year > 1999 && ymd.Year < 2100 ? -1 : ymd.Year / 100;
            int y = ymd.Year % 100;
            int s = (ymd.Month - 1) / 3;
            int m = (ymd.Month - 1) % 3;
            int d = ymd.Day;
            int ewo = ((int)GetDayOfWeek(new DateTime(ymd.Year, 3, 14)) + 6) % 7;
            ulong packed = (ulong)(ewo + ((uint)d << 3) + ((uint)m << 8) + ((uint)s << 10) + ((uint)y << 12));
            if (c > -1) packed += (uint)c << 19;
            fx.ShortDatePattern = $"'#{packed.ToString("x")}'";
            fx.LongDatePattern = $"'#{packed.ToString("x")} ({(c > -1 ? $"c{c}." : "")}y{y}.s{s}.m{m}.d{d}.ewo{ewo})'";
            return fx;
        }
    }
}
