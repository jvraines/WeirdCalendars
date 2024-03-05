using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class AstronomicSolilunarCalendar : WeirdCalendar {
        
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Astronomic_Soli-Lunar_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 3, 21);
        protected override int SyncOffset => 0;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        // Maximum valid date for season calculation from VSOP87.
        public override DateTime MaxSupportedDateTime => new DateTime(6000, 1, 1);

        public AstronomicSolilunarCalendar() => Title = "Astronomic Soli-Lunar Calendar";

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).BlueMoon == 0 ? 12 : 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Moons[month - 1];
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).BlueMoon > 0;
        }

        private static Dictionary<int, LunarPlot> Plots = new Dictionary<int, LunarPlot>();

        private LunarPlot GetPlot(int year) {
            if (Plots.ContainsKey(year)) return Plots[year];
            LunarPlot p = new LunarPlot(year, AA.Net.Earth.Season.March);
            Plots.Add(year, p);
            return p;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Notos Alpha", "Notos Beta", "Notos Gamma", "Notos Delta", "Notos Epsilon", "Notos Zeta", "Arctos Alpha", "Arctos Beta", "Arctos Gamma", "Arctos Delta", "Arctos Epsilon", "Arctos Zeta", "" }, new string[] { "NAl", "NBe", "NGa", "NDe", "NEp", "NZe", "AAl", "ABe", "AGa", "ADe", "AEp", "AZe", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            int blue = GetPlot(ld.Year).BlueMoon;
            if (blue > 0) {
                string m, ma;
                switch (ld.Month) {
                    case 3:
                        m = "Notos Koppa";
                        ma = "NKo";
                        break;
                    case 6:
                        m = "Notos Digamma";
                        ma = "NDi";
                        break;
                    case 9:
                        m = "Arctos Koppa";
                        ma = "AKo";
                        break;
                    case 12:
                        m = "Arctos Digamma";
                        ma = "ADi";
                        break;
                    default:
                        int mo = ld.Month - (ld.Month >= blue ? 2 : 1);
                        m = dtfi.MonthNames[mo];
                        ma = dtfi.AbbreviatedMonthNames[mo];
                        break;
                }
                fx.MonthFullName = m;
                fx.MonthShortName = ma;
            }
            return fx;
        }
    }
}
