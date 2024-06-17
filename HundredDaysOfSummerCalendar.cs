using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class HundredDaysOfSummerCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Hundred_Days_of_Summer_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 7);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("k", "Hek format")
        };

        protected override int FirstMonth => 0;
        protected override int GetFirstDayOfMonth(int year, int month) => 0;

        public enum DayOfWeekWC {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sabado
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int w = WeekdayNumber(time);
            if (w == 7) throw BadWeekday;
            return (DayOfWeek)w;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) => (DayOfWeekWC)WeekdayNumber(time);

        private int WeekdayNumber(DateTime time) {
            var ymd = ToLocalDate(time);
            switch (GetHekMont(ymd.Year, ymd.Month)) {
                case 0:
                    return ymd.Day % 7;
                case 1:
                    return ymd.Day == 16 ? 7 : (ymd.Day + (ymd.Day < 16 ? 5 : 4)) % 7;
                default:
                    return ymd.Day == 33 ? 7 : (ymd.Day + 2) % 7;
            }
        }

        private int GetHekMont(int year, int month) => (month + 3 - TriolYear(year)) % 3;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 10 : 11;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetHekMont(year, month) == 2 ? 34 : 33;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            if (IsLeapYear(year)) return 334;
            return TriolYear(year) == 0 ? 366 : 367;
        }

        private (int cycle, int year) GetCycle(int year) {
            int y = (year - 1447) % 351;
            if (y < 161) return (y / 23, y % 23);
            if (y < 187) return (7, y - 161);
            if (y < 325) {
                y -= 187;
                return (y / 23 + 8, y % 23);
            }
            return (14, y - 325);
        }

        private int TriolYear(int year) => GetCycle(year).year % 3;

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            var c = GetCycle(year);
            return c.cycle == 7 || c.cycle == 14 ? c.year == 25 : c.year == 22;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public string GetHekFormat(DateTime time) {
            int y = ToLocalDate(time).Year;
            int cy = GetCycle(y).year;
            int t = cy % 3;
            int d = t == 0 ? 0 : t == 1 ? 366 : 733;
            d += GetDayOfYear(time) - 1;
            return $"{y}-{(char)(65 + cy)} hek {d / 100} day {d % 100}";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Janel", "Feble", "Mahel", "Aprel", "Junel", "Jewel", "Augle", "Seple", "Octel", "Novel", "Decel", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int month = ToLocalDate(time).Month;
            fx.MonthFullName = dtfi.MonthNames[month];
            fx.MonthShortName = dtfi.AbbreviatedMonthNames[month];
            if (WeekdayNumber(time) == 7) {
                fx.DayFullName = "Sabado";
                fx.DayShortName = "Sab";
            }
            fx.Format = format.ReplaceUnescaped("k", $"'{GetHekFormat(time)}'");
            return fx;
        }
    }
}
