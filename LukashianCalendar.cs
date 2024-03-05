using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class LukashianCalendar : WeirdCalendar {

        public override string Author => "Gert-Jan Schouten";
        public override Uri Reference => new Uri("https://www.lukashian.org/howitworks");

        protected override DateTime SyncDate => new DateTime(2023, 12, 22, 21, 02, 4);
        protected override int SyncOffset => 3900;

        public int GetBeeps(DateTime time) {
            ValidateDateTime(time);
            return ToLocalDate(time).Beeps;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override int GetYear(DateTime time) {
            ValidateDateTime(time);
            return ToLocalDate(time).Year;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override int GetDayOfYear(DateTime time) {
            ValidateDateTime(time);
            return ToLocalDate(time).Day;
        }

        public override DateTime AddYears(DateTime time, int years) {
            ValidateDateTime(time);
            var ld = ToLocalDate(time);
            ld.Year += years;
            int lastDay = GetDaysInYear(ld.Year);
            return new DateTime(time.Year + years, time.Month, time.Day - (ld.Day > lastDay ? 1 : 0), time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        private static Dictionary<int, LukashianPlot> Plots = new Dictionary<int, LukashianPlot>();

        private LukashianPlot GetPlot(int year) {
            if (Plots.ContainsKey(year)) return Plots[year];
            LukashianPlot p = new LukashianPlot(year - SyncOffset - 1);
            Plots.Add(year, p);
            return p;
        }

        public DateTime ToDateTime(int year, int day, int beeps) {
            LukashianPlot p = GetPlot(year);
            DateTime d1 = p.Days[day - 1];
            DateTime d2 = p.Days[day];
            return d1.AddTicks((d2.Ticks - d1.Ticks) * beeps / 10000);
        }

        private new (int Year, int Day, int Beeps) ToLocalDate(DateTime time) {
            int year = time.Year + SyncOffset;
            LukashianPlot p = GetPlot(year);
            if (time > p.YearEnd) {
                year++;
                p = GetPlot(year);
            }
            int day = p.Days.Select((d, index) => new { d, index }).First(x => time < x.d).index;
            int beeps = (int)Math.Round((double)(time - p.Days[day - 1]).Ticks / (p.Days[day] - p.Days[day - 1]).Ticks * 10000);
            return (year, day, beeps);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            string d = $"{ld.Day}-{ld.Year}";
            string t = ld.Beeps.ToString("0000");
            fx.LongDatePattern = d;
            fx.ShortDatePattern = d;
            fx.LongTimePattern = t;
            fx.ShortTimePattern = t;
            return fx;
        }
    }
}
