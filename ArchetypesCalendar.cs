using System;
using System.Globalization;

namespace WeirdCalendars {
    public class ArchetypesCalendar : WeirdCalendar {

        public enum DayOfWeekWC {
            Sun,
            Mercury,
            Venus,
            Earth,
            Mars,
            Jupiter,
            Saturn,
            Uranus,
            Neptune,
            Pluto
        }

        protected override DateTime SyncDate => new DateTime(2022, 2, 1);
        protected override int SyncOffset => 2697;

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/arch_cal/arch_cal.htm");

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;
        public override int DaysInWeek => 10;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int d = WeekdayNumber(time);
            if (d < 7) return (DayOfWeek)d;
            throw BadWeekday;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        private int WeekdayNumber(DateTime time) => (ToLocalDate(time).Day - 1) % 10;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (IsLeapYear(year) && month == 10) return 30;
            else return (month & 1) == 1 ? 30 : 29;
        }

        public override int GetDaysInYear(int year, int era) {
            return (IsLeapYear(year) ? 355 : 354) + (GetMonthsInYear(year) - 12) * 30;
        }

        private int Position(int year) {
            return (year + 1360) % 1803 + 1;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return (664 * Position(year) + 901) % 1803 < 664 ? 13 : 12;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 10 && day == 30;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (350 * Position(year) + 901) % 1803 < 350;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Apollo", "Diana", "Hermes", "Aphrodite", "Ares", "Zeus", "Chronos", "Prometheus", "Orpheus", "Sophia", "Dionysus", "Demeter", "Persephone" }, null, new string[] { "Sun Day", "Mercury Day", "Venus Day", "Earth Day", "Mars Day", "Jupiter Day", "Saturn Day" }, new string[] { "Sun", "Mer", "Ven", "Ear", "Mar", "Jup", "Sat" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int d = WeekdayNumber(time);
            if (d > 6) {
                fx.DayFullName = ((DayOfWeekWC)d).ToString();
                fx.DayShortName = fx.DayFullName.Substring(0, 3);
            }
            return fx;
        }
    }
}
