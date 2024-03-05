using System;
using System.Globalization;

namespace WeirdCalendars {
    public class FortyThree450Calendar : WeirdCalendar {
        
        public override string Author => "Victor S. Engel";
        public override Uri Reference => new Uri("http://the-light.com/calendar.html");

        protected override DateTime SyncDate => new DateTime(2023, 12, 21);
        protected override int SyncOffset => -2011;
        // Months and days appear to be zero-based but the author's calendar displays
        // 1-based years. Its months are also off by -1.

        protected override int FirstMonth => 0;
        protected override int GetFirstDayOfMonth(int year, int month) => 0;

        public FortyThree450Calendar() => Title = "43/450 Calendar";

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return ((year - 1) * 13 + month) * 43 % 450 > 406 ? 29 : 28;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            int accumStart = (year - 1) * 13 * 43 % 450;
            int accumFinish = accumStart + 13 * 43;
            return accumFinish < 900 ? 365 : 366;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            fx.MonthFullName = $"Month {ymd.Month}";
            fx.MonthShortName = $"M{ymd.Month}";
            if (ymd.Month == 0 || ymd.Day == 0) {
                string m1 = ymd.Month.ToString();
                string m2 = ymd.Month.ToString("D2");
                string d1 = ymd.Day.ToString();
                string d2 = ymd.Day.ToString("D2");
                fx.Format = FixDigits(format, null, null, m2, m1, d2, d1);
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, m2, m1, d2, d1);
                fx.LongDatePattern = FixDigits(fx.LongDatePattern, null, null, m2, m1, d2, d1);
            }
            return fx;
        }
    }
}
