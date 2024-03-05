using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class OldEnglishSolarCalendar : WeirdCalendar {

        public override string Author => "Timey";
        public override Uri Reference => new Uri("https://time-meddler.co.uk/the-old-english-solar-calendar/");

        protected override DateTime SyncDate => new DateTime(2023, 12, 22);
        protected override int SyncOffset => -443;

        public override DateTime MaxSupportedDateTime => new DateTime(3000, 1, 1);

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Festival")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 10:
                case 11:
                case 12:
                    return 30;
                case 2:
                    return IsLeapYear(year) ? 30 : 29;
                default:
                    return 31;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 2 && day == 30;
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (LeapYears.ContainsKey(year)) return LeapYears[year];
            int gYear = year + 443;
            int yearStart = (int)Math.Round(Earth.SeasonStart(gYear, Earth.Season.December) + 0.5);
            int yearEnd = (int)Math.Round(Earth.SeasonStart(gYear + 1, Earth.Season.December) + 0.5);
            bool ly = yearEnd - yearStart == 366;
            LeapYears.Add(year, ly);
            return ly;
        }

        private static Dictionary<int, (int, int, int, int)> Movables = new Dictionary<int, (int, int, int, int)>();

        public string GetFestival(int year, int month, int day) {
            string f = null;
            switch (month) {
                case 1:
                    if (day == 1) f = "Yule Day";
                    else if (day < 13) f = "Yuletide";
                    break;
                case 2:
                    if (day == 15) f = "Winter Cross Quarter";
                    break;
                case 4:
                    if (day == 1) f = "Ostara";
                    break;
                case 5:
                    if (day == 5) f = "Spring Cross Quarter";
                    break;
                case 6:
                case 7:
                    if (month == 6 && day == 31 || month == 7 && day == 1) f = "Litha";
                    break;
                case 8:
                    if (day == 16) f = "Summer Cross Quarter";
                    break;
                case 9:
                    if (day == 31) f = "Mabon/Harvest Home";
                    break;
                case 11:
                    if (day == 16) f = "Autumn Cross Quarter";
                    break;
                case 12:
                    if (day == 30) f = "Modraniht (Mothers' Night)";
                    break;
            }
            //check movable feasts
            if (month == 4 || month == 5 || month == 9 || month == 10) {
                (int Eggmonth, int Eggday, int Harvestmonth, int Harvestday) m;
                if (Movables.ContainsKey(year)) {
                    m = Movables[year];
                }
                else {
                    int gYear = year + 444;
                    double s = Earth.SeasonStart(gYear, Earth.Season.March);
                    var ld = ToLocalDate(Moon.NextPhase(Moon.Phase.FullMoon, s).ToDateTime());
                    int eggMonth = ld.Month;
                    int eggDay = ld.Day;
                    s = Earth.SeasonStart(gYear, Earth.Season.September);
                    ld = ToLocalDate(Moon.NextPhase(Moon.Phase.FullMoon, s - 30).ToDateTime());
                    int harvestMonth = ld.Month;
                    int harvestDay = ld.Day;
                    m = (eggMonth, eggDay, harvestMonth, harvestDay);
                    Movables.Add(year, m);
                }
                if (month == m.Eggmonth && day == m.Eggday) f += $"{(f != null ? ", " : "")}Egg Moon";
                else if (month == m.Harvestmonth && day == m.Harvestday) f += $"{(f != null ? ", " : "")}Harvest Moon";
            }
            return f ?? "(none)";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Aeftergiuli", "Solmonath", "Hrethmonath", "Eostremonath", "Thrimilchi", "Aerlitha", "Aefterlitha", "Weodmonath", "Halegmonath", "Winterfylleth", "Blotmonath", "Aergiuli", "" }, new string[] { "Afg", "Sol", "Hre", "Eos", "Thr", "Arl", "Afl", "Weo", "Hal", "Win", "Blo", "Arg", ""}, new string[] { "Sunnandæg", "Monandæg", "Tiwesdæg", "Wodnesdæg", "Þunresdæg", "Frigedæg", "Sæternesdæg" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.Contains("n")) {
                var ld = ToLocalDate(time);
                string f = GetFestival(ld.Year, ld.Month, ld.Day);
                if (f != null) fx.Format = format.ReplaceUnescaped("n", $"'{f}'");
            }
            return fx;
        }
    }
}
