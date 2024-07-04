using System;
using System.Globalization;

namespace WeirdCalendars {
    public class GlobalCalendar : FixedCalendar {

        public override string Author => "Miklos Lente";
        public override Uri Reference => new Uri("https://web.archive.org/web/20050511212308/http://calendarreform.org:80/calendarproposals.pdf");

        protected override DateTime SyncDate => new DateTime(2023, 12, 31);
        protected override int SyncOffset => 1;

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % 7;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                    return 29;
                case 13:
                    return IsLeapYear(year) ? 29 : 28;
                default:
                    return 28;
            }
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            return month > 1 ? 1 : 0;
        }


        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 29;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0 || day == 29;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 0 ? "New Year's Day" : "Leapday";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            string[] m = (string[])dtfi.MonthNames.Clone();
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            for(int i = 12; i > 5; i--) {
                m[i] = m[i - 1];
                ma[i] = ma[i - 1];
            }
            m[5] = "Midi";
            ma[5] = "Mid";
            SetNames(dtfi, m, ma);
        }
    }
}
