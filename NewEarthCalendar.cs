using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NewEarthCalendar : LeapWeekCalendar {

        public override string Author => "James A. Reich";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/New_Earth_Calendar");

        protected override DateTime SyncDate => new DateTime(2018, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetMonthsInYear(int year, int era) => 13;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13 && IsLeapYear(year) ? 35 : 28;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 5 == 0 && (year % 40 != 0 || year % 400 == 0);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day > 28;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] a = (string[])dtfi.AbbreviatedMonthNames.Clone();
            for (int i = 11; i >= 5; i--) {
                m[i + 1] = m[i];
                a[i + 1] = a[i];
            }
            m[6] = "Luna";
            a[6] = "Lun";
            SetNames(dtfi, m, a);
        }
    }
}
