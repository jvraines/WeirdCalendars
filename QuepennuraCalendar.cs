using System;
using System.Globalization;

namespace WeirdCalendars {
    public class QuepennuraCalendar : LeapWeekCalendar {
        
        public override string Author => "Yosuké";
        public override Uri Reference => new Uri("https://sites.google.com/view/quepennura-leap-week-calendar/home");

        protected override DateTime SyncDate => new DateTime(2022, 1, 3);
        protected override int SyncOffset => 0;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 2:
                case 7:
                    return 30;
                case 12:
                    return IsLeapYear(year) ? 32 : 25;
                default:
                    return 31;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day > 25;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return Calc(year) > Calc(year - 1);

            int Calc(int y) {
                return (85 * y + 224) / 479;
            }
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }
    }
}
