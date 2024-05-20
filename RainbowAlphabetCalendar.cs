using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class RainbowAlphabetCalendar : WeirdCalendar {

        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/palmen/rainbow.htm");

        protected override DateTime SyncDate => new DateTime(2024, 3, 27);
        protected override int SyncOffset => 0;

        // "Months" are the 52 weeks.

        public enum DayOfWeekWC {
            Redday,
            Orangeday,
            Yellowday,
            Greenday,
            Blueday,
            Indigoday,
            Violetday,
            Purpleday,
            Magentaday
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 52;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 52 ? 7 : IsLeapYear(year) ? 9 : 8;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return base.IsLeapYear(year + 1, era);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 9;
        }

        private int WeekdayNumber(DateTime time) => GetDayOfMonth(time) - 1;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int w = WeekdayNumber(time);
            if (w > 6) throw BadWeekday;
            return (DayOfWeek)w;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) => (DayOfWeekWC)WeekdayNumber(time);

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.LongDatePattern = "dddd MMMM yyyy";
            dtfi.ShortDatePattern = "ddd/MMM/yyyy";
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int m = ToLocalDate(time).Month;
            string a = ((char)((m < 27 ? 64 : 70) + m)).ToString();
            fx.MonthFullName = $"{(m < 27 ? "big" : "little")} {a}";
            fx.MonthShortName = a;
            fx.DayFullName = GetDayOfWeekWC(time).ToString();
            fx.DayShortName = fx.DayFullName.Substring(0, 2).ToUpper();
            return fx;
        }
    }
}
