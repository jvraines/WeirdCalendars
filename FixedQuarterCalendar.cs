using System;
using System.Globalization;

namespace WeirdCalendars {
    public class FixedQuartersCalendar : WeirdCalendar {
        
        public override string Author => "M. Villoro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Fixed_Quarters_Calendar");

        protected override DateTime SyncDate => new DateTime(2017, 3, 20);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 3 > 0 ? 30 : month < 12 ? 31 : IsLeapYear(year) ? 33 : 32;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 33 && month == 12 && IsLeapYear(year);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = new string[13];
            string[] ma = new string[13];
            for (int i = 0; i < 13; i++) {
                m[i] = $"Month-{i + 1}";
                ma[i] = $"M {i + 1}";
            }
            SetNames(dtfi, m, ma);
        }
    }
}