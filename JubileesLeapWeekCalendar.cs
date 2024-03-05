using System;
using System.Globalization;
using System.Linq;

namespace WeirdCalendars {
    public class JubileesLeapWeekCalendar : LeapWeekCalendar {
        
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Jubilees_Leap_Week_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 9, 22);
        protected override int SyncOffset => 4001;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 16;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month - 1) % 4 == 0 ? 1 :
                   IsLeapYear(year) && (month == 12 && year < 5101 || month == 16 && year > 5100) ? 37 :
                   30;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfYear(time) - 1) % 7);
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            switch(year % 400) {
                case 001: case 007: case 012: case 018: case 023: case 029: case 035: case 040: case 046:
                case 052: case 058: case 063: case 069: case 074: case 080: case 086: case 091: case 097:
                case 102: case 108: case 113: case 119: case 124: case 130: case 136: case 141: case 147:
                case 153: case 159: case 164: case 170: case 175: case 181: case 187: case 192: case 198:
                case 204: case 210: case 215: case 221: case 226: case 232: case 238: case 243: case 249:
                case 255: case 261: case 266: case 272: case 277: case 283: case 289: case 294: case 300:
                case 305: case 311: case 316: case 322: case 327: case 333: case 339: case 344: case 350:
                case 356: case 362: case 367: case 373: case 378: case 384: case 390: case 395:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day > 30;
        }

        private static string[] Months = { "Sunday of the West", "Alef", "Bet", "Gimel", "Sunday of the South", "Dalet", "He", "Vav", "Sunday of the East", "Zayin", "Chet", "Tet", "Sunday of the North", "Yad", "Kaf", "Lamed" };

        private static string[] AbbrMonths = { "Wes", "Ale", "Bet", "Gim", "Sou", "Dalet", "He", "Vav", "Eas", "Zay", "Che", "Tet", "Nor", "Yad", "Kaf", "Lam" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, Months.Take(13).ToArray(), AbbrMonths.Take(13).ToArray());
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int m = ToLocalDate(time).Month - 1;
            fx.MonthFullName = Months[m];
            fx.MonthShortName = AbbrMonths[m];
            return fx;
        }
    }
}
