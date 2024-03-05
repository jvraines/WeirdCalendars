using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NewRomanCalendar : WeirdCalendar {
        
        public enum DayOfWeekWC {
            Blank = -1,
            A,
            B,
            C,
            D,
            E,
            F
        }

        public override string Author => "Hellerick Ferlibay";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/New_Roman_Lunisolar_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 12, 13);
        protected override int SyncOffset => 753;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        protected override int DaysInWeek => 6;

        public NewRomanCalendar() => Title = "New Roman Lunisolar Calendar";
        
        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            var ymd = ToLocalDate(time);
            if (IsLeapDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            return (DayOfWeek)WeekdayNumber(ymd.Year, ymd.Month, ymd.Day);
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            ValidateDateTime(time);
            var ymd = ToLocalDate(time);
            if (IsLeapDay(ymd.Year, ymd.Month, ymd.Day)) return DayOfWeekWC.Blank;
            return (DayOfWeekWC)WeekdayNumber(ymd.Year, ymd.Month, ymd.Day);
        }

        private int WeekdayNumber(int year, int month, int day) {
            return (day - (month == 1 && IsShortFebruariae(year) ? -2 : 1)) % 6;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                    return IsShortFebruariae(year) ? 27 : 42;
                case 12:
                    return year % 334 % 19 % 11 % 3 == 0 ? 42 : IsLeapYear(year) ? 28 : 27;
                default:
                    return 30;
            }
        }

        private bool IsShortFebruariae(int year) {
            return year % 334 % 19 % 11 % 3 != 1;
        }

        public override int GetDaysInYear(int year, int era) {
            return 300 + GetDaysInMonth(year, 1) + GetDaysInMonth(year, 12);
        }

        public override bool IsLeapYear(int year) {
            ValidateDateParams(year, 0);
            return (4 * (year % 334) - 2 * (year % 334 / 19) - 3 * (year % 334 % 19 / 11) - 4 * (year % 334 % 19 % 11 / 3) - 4) % 13 < 4;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era); 
            return month == 12 && day == 28 && IsLeapYear(year);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            DayOfWeekWC d = GetDayOfWeekWC(time);
            if (d == DayOfWeekWC.Blank) {
                fx.DayFullName = "Brumia Intercalaris";
                fx.DayShortName = "Int";
            }
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Februariae", "Martiae", "Apriliae", "Maiae", "Juniae", "Quintiliae", "Sextiliae", "Septembiae", "Octobriae", "Novembriae", "Decembriae", "Januariae", "" }, null, new string[] { "Day A", "Day B", "Day C", "Day D", "Day E", "Day F", "" }, new string[] { "A", "B", "C", "D", "E", "F", "" });
        }
    }
}
