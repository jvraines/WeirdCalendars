using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WeirdCalendars {
    public class ToMadnessCalendar : WeirdCalendar {
        
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/To_Madness_Calendar");

        protected override DateTime SyncDate => new DateTime(2050, 1, 2);
        protected override int SyncOffset => -2050;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 20 : 19;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 20:
                    return 1;
                case 19:
                    return 3;
                case 18:
                    return 5;
                default:
                    return 39 - month * 2;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int y = (year + 2048) % 128 + 1;
            if (y < 61) return y > 15 && y % 4 == 0;
            if (y < 93) return y > 71 && y % 4 == 0;
            switch (y) {
                case 100: case 104: case 108: case 114:
                case 116: case 118: case 121: case 122:
                case 123: case 125: case 126: case 127:
                case 128:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 20;
        }

        private static string[] MonthName = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "Anguish", "Stasis", "Reprieve", "Delusion", "Demons", "Tatters", "Absurdum", "Insanity" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int m = ToLocalDate(time).Month - 1;
            fx.MonthFullName = MonthName[m];
            fx.MonthShortName = MonthName[m].Substring(0, 3);
            FixNegativeYears(fx, time);
            return fx;
        }

    }
}
