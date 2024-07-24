using System;
using System.Globalization;

namespace WeirdCalendars {
    public class DarianCalendar : WeirdCalendar {
        
        public override string Author => "Thomas Gangale";
        public override Uri Reference => new Uri("https://ops-alaska.com/time/gangale_mst/darian.htm");

        protected override DateTime SyncDate => new DateTime(2022, 12, 25, 11, 11, 27);
        protected override int SyncOffset => -1802;

        protected override double TimescaleFactor => 1.027491252;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((ToLocalDate(time).Day - 1) % 7);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 6 != 0 || month == 24 && IsLeapYear(year) ? 28 : 27;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 24;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 669: 668;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year & 1) == 1 || year % 10 == 0 && year % 100 != 0 || year % 500 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 28 && month == 24 && IsLeapYear(year);
        }

        private static string[] MonthName = { "Sagittarius", "Dhanus", "Capricornus", "Makara", "Aquarius", "Kumbha", "Pisces", "Mina", "Aries", "Mesha", "Taurus", "Rishabha", "Gemini", "Mithuna", "Cancer", "Karka", "Leo", "Simha", "Virgo", "Kanya", "Libra", "Tula", "Scorpius", "Vrishika" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            for (int i = 0; i < 13; i++) m[i] = MonthName[i];
            SetNames(dtfi, m, null, new string[] { "Sol Solis", "Sol Lunae", "Sol Martis", "Sol Mercurii", "Sol Jovis", "Sol Veneris", "Sol Saturni" }, new string[] { "Sol", "Lun", "Mar", "Mer", "Jov", "Ven", "Sat"});
            dtfi.LongTimePattern += " 'MTC'";
            dtfi.ShortTimePattern += " 'MTC'";
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            CustomizeTimes(fx, time);
            string month = MonthName[ToLocalDate(time).Month - 1];
            fx.LongDatePattern = FixMonth(fx.LongDatePattern);
            fx.ShortDatePattern = FixMonth(fx.ShortDatePattern);
            fx.Format = FixMonth(format);
            return fx;

            string FixMonth(string f) {
                f = f.ReplaceUnescaped("MMMM", $"\"{month}\"").ReplaceUnescaped("MMM", $"\"{month.Substring(0, 3)}\"");
                return f;
            }
        }
    }
}
