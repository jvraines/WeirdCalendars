using System;
using System.Globalization;

namespace WeirdCalendars {
    public class BlanxartCalendar : WeirdCalendar {
        
        public override string Author => "Albert Blanxart Pàmies";
        public override Uri Reference => new Uri("https://commons.wikimedia.org/wiki/File:Blanxart_calendar.jpg");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public enum DayOfWeekWC {
            Sunday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }

        public override int DaysInWeek => 6;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfMonth(time) - 1) % 6);
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return (DayOfWeekWC)((GetDayOfMonth(time) - 1) % 6);
        }
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 12 ? 30 : IsLeapYear(year) ? 36 : 35;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 36;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] d = (string[])dtfi.DayNames.Clone();
            string[] da = (string[])dtfi.AbbreviatedDayNames.Clone();
            for (int i = 1; i < 6; i++) {
                d[i] = d[i + 1];
                da[i] = da[i + 1];
            }
            d[6] = ""; da[6] = "";
            SetNames(dtfi, null, null, d, da);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int d = ToLocalDate(time).Day;
            string dd;
            if (d > 30) {
                dd = $"D{d - 6}";
                fx.Format = FixDigits(format, null, null, null, null, dd, dd);
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, null, null, dd, dd);
                fx.LongDatePattern = FixDigits(fx.LongDatePattern, null, null, null, null, dd, dd);
            }
            return fx;
        }
    }
}
