using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class EllelCalendar : WeirdCalendar {
        
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Ellel_Calendar");

        protected override DateTime SyncDate => new DateTime(2001, 1, 1);
        protected override int SyncOffset => -2000;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 11;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 3:
                case 9:
                    return 34;
                case 6:
                    return IsLeapYear(year) ? 34 : 33;
                default:
                    return 33;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int y = (year - (year > 0 ? 1 : 0)) % 351 % 33;
            return year == 0 || y != 0 && y % 4 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day == 34;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Iannel", "Feble", "Mahel", "Aprel", "Junel", "Jewel", "Augle", "Seple", "Octel", "Novel", "Decel", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            FixNegativeYears(fx, time);
            return fx;
        }
    }
}
