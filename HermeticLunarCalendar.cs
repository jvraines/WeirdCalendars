using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {

    public class HermeticLunarCalendar : WeirdCalendar {
        
        public enum DayOfWeekWC {
            Dayone,
            Daytwo,
            Daythree,
            Dayfour,
            Dayfive,
            Nineday,
            Herday,
            Freeday,
            Moonday
        }

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/hlwc/hlwc.htm");

        protected override DateTime SyncDate => new DateTime(2020, 3, 25);
        protected override int SyncOffset => 3000;
        public override DateTime MaxSupportedDateTime => new DateTime(6000, 1, 1); //bound for astronomical accuracy

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {("w", "Week number")};

        public HermeticLunarCalendar() => Title = "Hermetic Lunar Week Calendar";

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int d = (int)GetDayOfWeekWC(time);
            if (d < 7) return (DayOfWeek)d;
            throw BadWeekday;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            ValidateDateTime(time);
            var wad = GetWeekAndDay(time);
            if (wad.Day > 5) wad.Day += 9 - wad.DaysInWeek;
            return (DayOfWeekWC)(wad.Day - 1);
        }

        private (int Week, int Day, int DaysInWeek) GetWeekAndDay(DateTime time) {
            var ymd = ToLocalDate(time);
            int[] wd = GetPlot(ymd.Year).Months[ymd.Month - 1].WeekDays;
            int days = ymd.Day;
            int week;
            for (week = 0; week < 4; week++) {
                int d = days - wd[week];
                if (d < 1) break;
                days = d;
            }
            return (week + 1, days, wd[week]);
        }

        public int GetWeek(DateTime time) {
            ValidateDateTime(time);
            return GetWeekAndDay(time).Week;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Months[month - 1].MonthDays;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).Months.Count;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        private static Dictionary<int, HermeticLunarPlot> Plots = new Dictionary<int, HermeticLunarPlot>();

        private HermeticLunarPlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out HermeticLunarPlot p)) {
                p = new HermeticLunarPlot(year - SyncOffset);
                Plots.Add(year, p);
            }
            return p;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            int d = (int)GetDayOfWeekWC(time);
            fx.DayFullName = Enum.GetName(typeof(DayOfWeekWC), d);
            fx.DayShortName = "12345NHFM".Substring(d, 1);
            var wd = GetWeekAndDay(time);
            fx.ShortDatePattern = $"yyyy-MM-{wd.Week}-{wd.Day}";
            string number = new string[] { "one", "two", "three", "four" }[wd.Week - 1];
            fx.LongDatePattern = $"dddd 'in Week{number} of' MMMM, yyyy";
            fx.Format = format.ReplaceUnescaped("w", wd.Week.ToString());
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Artaud", "Benjamin", "Clark", "De Quincy", "Ellis", "Furst", "Grof", "Hofmann", "Izumi", "Janiger", "Kesey", "Lilly", "McKenna" }, new string[] { "Art", "Ben", "Cla", "DeQ", "Ell", "Fur", "Gro", "Hof", "Izu", "Jan", "Kes", "Lil", "McK" }, new string[] { "Dayone", "Daytwo", "Daythree", "Dayfour", "Dayfive", "Various", "Various" }, new string[] { "One", "Two", "Thr", "Fou", "Fiv", "?", "?" });
        }
    }
}