using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class StonehengeCalendar : WeirdCalendar {
        public override string Author => "Timothy Darvill";
        public override Uri Reference => new Uri("https://www.cambridge.org/core/journals/antiquity/article/keeping-time-at-stonehenge/792A5E8E091C8B7CB9C26B4A35A6B399");

        protected override DateTime SyncDate => new DateTime(2025, 1, 26); // Accumulated Julian error of +33 days from author's New Year date
        protected override int SyncOffset => 2400;  // Revised Stonehenge 3ii radiocarbon date estimate

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("i", "Stone data")
        };

        public override int DaysInWeek => 10;

        public enum DayOfWeekWC {
            Stone1,
            Stone2,
            Stone3,
            Stone4,
            Stone5,
            Stone6,
            Stone7,
            Stone8,
            Stone9,
            Stone10
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int w = WeekdayNumber(time);
            if (w > 6) throw BadWeekday;
            return (DayOfWeek)w;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) => (DayOfWeekWC)WeekdayNumber(time);

        private int WeekdayNumber(DateTime time) => (ToLocalDate(time).Day - 1) % 10;

        public override int GetMonthsInYear(int year, int era) {
           ValidateDateParams(year, era);
           return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 30 : IsLeapYear(year) ? 6 : 5;
        }

        //0-29 sarsens, 30-34 trilithons; weight above ground level in tons and height in meters
        //from http://www.stonesofstonehenge.org.uk/
        private static double[,] StoneData = {
            {13.08, 3.96},
            {19.27, 4.27},
            {13.32, 4.16},
            {15.98, 4.27},
            {13.53, 4.27},
            {13.29, 4.19},
            {14.32, 3.96},
            {1.56, 0},
            {0.29, 0},
            {16.96, 3.96},
            {4.87, 2.74},
            {15.16, 0},
            {0, 0},
            {6.98, 0},
            {3.14, 0},
            {23.52, 4.27},
            {0, 0},
            {0, 0},
            {3.96, 0},
            {0, 0},
            {11.06, 3.96},
            {13.128, 3.81},
            {11.8, 3.81},
            {0, 0},
            {7.08, 8},
            {0.96, 0},
            {12.48, 3.96},
            {21.792, 3.96},
            {16.68, 3.96},
            {18.6, 3.96},
            {63.08, 5.92},
            {64.95, 6.02},
            {47.78, 6.55},
            {68.53, 6.02},
            {48.47, 5.03}
        };

        public string GetStoneData(int stone) {
            stone--;
            double weight = StoneData[stone, 0];
            double height = StoneData[stone, 1];
            return weight == 0 && height == 0 ? "missing stone" : $"{(height == 0 ? "fallen" : $"height {height} m")}; above-ground weight {weight} t";
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            if (ymd.Month < 13) {
                fx.MonthFullName = $"Month {ymd.Month}";
                fx.MonthShortName = $"M{ymd.Month}";
                string d1 = $"Sarsen {ymd.Day}";
                string d2 = $"Sarsen {ymd.Day:00}";
                fx.LongDatePattern = FixDigits(s:fx.LongDatePattern, d1:d1, d2:d2);
            }
            else {
                fx.MonthFullName = "Trilithon";
                fx.MonthShortName = "Tri";
                string d = ymd.Day.ToRoman();
                fx.LongDatePattern = FixDigits(s: fx.LongDatePattern, d1: d, d2: d);
                fx.ShortDatePattern = FixDigits(s: fx.ShortDatePattern, d1: d, d2: d);
                fx.Format = FixDigits(s: format, d1: d, d2: d);
            }
            int w = WeekdayNumber(time) + 1;
            fx.DayFullName = $"Stone {w}";
            fx.DayShortName = $"S{w}";
            if (fx.Format.FoundUnescaped("i")) {
                int s = ymd.Month < 13 ? ymd.Day : ymd.Day + 30;
                fx.Format = fx.Format.ReplaceUnescaped("i", $"'{GetStoneData(s)}'");
            }
            return fx;
        }
    }
}
