using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class OldEnglishLunarCalendar : WeirdCalendar {

        public override string Author => "Timey";
        public override Uri Reference => new Uri("https://time-meddler.co.uk/the-old-english-lunar-calendar/");

        protected override DateTime SyncDate => new DateTime(2023, 12, 12);
        protected override int SyncOffset => 1;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunarCalendar;

        public override DateTime MaxSupportedDateTime => new DateTime(6000, 1, 1);

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
            if (!Plots.TryGetValue(year, out LunarPlot p)) {
                p = new LunarPlot(year - 1, AA.Net.Earth.Season.December);
                Plots.Add(year, p);
            }
            return p;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Moon After Yule", "Snow Moon", "Lenten Moon", "Egg Moon", "Milk Moon", "Flower Moon", "Hay Moon", "Grain Moon", "Harvest Moon", "Hunter's Moon", "Blood Moon", "Moon Before Yule", "" }, new string[] { "Aft", "Sno", "Len", "Egg", "Mil", "Flo", "Hay", "Gra", "Har", "Hun", "Blo", "Bef", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            int blue = GetPlot(ld.Year).BlueMoon;
            if (blue > 0) {
                string m, ma;
                if (ld.Month - 1 == blue) {
                    m = "Blue Moon";
                    ma = "Blu";
                }
                else {
                    int mo = ld.Month - (ld.Month > blue ? 2 : 1);
                    m = dtfi.MonthNames[mo];
                    ma = dtfi.AbbreviatedMonthNames[mo];
                }
                fx.MonthFullName = m;
                fx.MonthShortName = ma;
            }
            return fx;
        }
    }
}
