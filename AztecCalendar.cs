using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class AztecCalendar : WeirdCalendar {
        
        public override string Author => "Traditional and Ruben Ochoa";
        public override Uri Reference => new Uri("http://www.calmecacanahuac.com/tlaahcicacaquiliztli/Ruben_Ochoa_Count");

        protected override DateTime SyncDate => new DateTime(2022, 3, 21); // Day Count 10-16, Trecena 6 according to source
        protected override int SyncOffset => 0;
 
        // Maximum valid date for season calculation from AA.
        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Trecena")
        };

        public override int GetMonthsInYear(int year, int era) => 19;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 19 ? 20 : IsLeapYear(year) ? 6 : 5;
        }

        private static string[] YearSign = { "Tōchtli", "Ācatl", "Tēcpatl", "Calli" };

        public string GetYearCount(int year) {
            ValidateDateParams(year, 0);
            int n = (year + 2) % 13 + 1;
            int s = (year + 2) % 4;
            return $"{n}-{YearSign[s]}";
        }

        private int DiffDays(int year, int month, int day) {
            int eYear = SyncDate.Year;
            int diffDays = day - 1 - (month == 19 && day == 6 ? 1 : 0);
            while (month != 1) {
                month--;
                diffDays += GetDaysInMonth(year, month, 0);
            }
            while (year < eYear) {
                diffDays -= GetDaysInYear(year, 0) - (IsLeapYear(year) ? 1 : 0);
                year++;
            }
            while (year > eYear) {
                year--;
                diffDays += GetDaysInYear(year, 0) - (IsLeapYear(year) ? 1 : 0);
            }
            return diffDays;
        }

        private static string[] DaySignName = { "Cipactli", "Ehēcatl", "Calli", "Cuetzpalin", "Cōātl", "Miquiztli", "Mazātl", "Tōchtli", "Ātl", "Itzcuīntli", "Ozomahtli", "Malīnalli", "Ācatl", "Ocēlōtl", "Cuāuhtli", "Cōzcacuāuhtli", "Ōlīn", "Tecpatl", "Quiyahuitl", "Xōchitl" };

        private int DayNumber(int d) => (9 + d).FloorMod(13) + 1;
        private int DaySign(int d) => (15 + d).FloorMod(20);

        public string GetDayCount(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            int d = DiffDays(year, month, day);
            return $"{DayNumber(d)}-{DaySignName[DaySign(d)]}";
        }

        public string GetTrecena (int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            int d = DiffDays(year, month, day);
            int n = DayNumber(d);
            int s = DaySign(d);
            return $"1-{DaySignName[(s - n + 1).FloorMod(20)]}";
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 19 && day == 6;
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool ly)) {
                int yearStart = YearBegins(year);
                int yearEnd = YearBegins(year + 1);
                ly = yearEnd - yearStart == 366;
                LeapYears.Add(year, ly);
            }
            return ly;

            int YearBegins(int y) {
                // One day after the day of the first sunrise following or within 6 hours of the March equinox
                const double longMC = 99.1332; //positive for AA convention
                const double latMC = 19.4326;
                DateTime eq = Earth.SeasonStart(y, Earth.Season.March).ToDateTime();
                DateTime start = (DateTime)Sky.RiseTransitSet(Bodies.Sun, eq, longMC, latMC).rise;
                if (start - eq > TimeSpan.FromHours(18)) start -= TimeSpan.FromDays(1);
                return (int)start.JulianDay() + 1;
            }
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Tlacaxipehualiztli", "Tozoztontli", "Huey Tozoztli", "Toxcatl", "Etzalcualiztli", "Tecuilhuitontli", "Huey Tecuilhuitl", "Miccailhuitontli", "Huey Miccailhuitontli", "Ochpaniztli", "Teotleco", "Tepeilhuitl", "Quecholli" }, new string[] { "Tla", "Toz", "HTz", "Tox", "Etz", "Tec", "HTc", "Mic", "HMc", "Och", "Teo", "Tep", "Que" });
        }

        private static string[] ExtraMonth = { "Panquetzaliztli", "Atemoztli", "Tititl", "Izcalli", "Atlcahualo", "Nemontemi" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            if (ld.Month > 13) {
                fx.MonthFullName = ExtraMonth[ld.Month - 14];
                fx.MonthShortName = fx.MonthFullName.Substring(0, 3);
            }
            string yearName = $"'{GetYearCount(ld.Year)}'";
            string dayName = $"'{GetDayCount(ld.Year, ld.Month, ld.Day)}'";
            fx.LongDatePattern = fx.LongDatePattern.ReplaceUnescaped("yyyy", yearName).ReplaceUnescaped("dddd", dayName);
            fx.Format = fx.Format.ReplaceUnescaped("yyyy", yearName).ReplaceUnescaped("dddd", dayName).ReplaceUnescaped("n", $"'{GetTrecena(ld.Year, ld.Month, ld.Day)}'");
            return fx;
        }
    }
}
