using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class BabylonianCalendar : WeirdCalendar {

        public override string Author => "Traditional";
        public override Uri Reference => new Uri("https://webspace.science.uu.nl/~gent0113/babylon/babycal.htm");

        public override string Notes => "Timekeeping scheme from https://web.archive.org/web/20240927040019/https://www.babylonianhours.com/.";

        protected override DateTime SyncDate => new DateTime(2024, 4, 10);
        protected override int SyncOffset => 748;

        // Maximum valid date for season calculation from VSOP87.
        public override DateTime MaxSupportedDateTime => VSOPLimit;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;
        
        private static Dictionary<int, BabylonianPlot> Plots = new Dictionary<int, BabylonianPlot>();

        private BabylonianPlot GetPlot(int year) {
            if (!Plots.TryGetValue(year, out BabylonianPlot p)) {
                p = new BabylonianPlot(year - SyncOffset);
                Plots.Add(year, p);
            }
            return p;
        }

        private static Dictionary<DateTime, (long, long, long, long)> SimanuTicks = new Dictionary<DateTime, (long, long, long, long)>();

        private (long rise, long set, long dayHour, long nightHour) GetSimanuTicks(DateTime time) {
            DateTime d = time.Date;
            if (!SimanuTicks.TryGetValue(d, out (long rise, long set, long dayHour, long nightHour) ticks )) {
                var rts = Sky.RiseTransitSet(Bodies.Sun, d, BabylonianPlot.BabylonLongitude, BabylonianPlot.BabylonLatitude, false);
                ticks.rise = ((DateTime)rts.rise).TimeOfDay.Ticks;
                ticks.set = ((DateTime)rts.set).TimeOfDay.Ticks;
                long dayDuration = ticks.set - ticks.rise;
                ticks.dayHour = dayDuration / 12;
                ticks.nightHour = (TimeSpan.TicksPerDay - dayDuration) / 12;
                SimanuTicks.Add(d, ticks);
            }
            return ticks;
        }

        public override int GetHour(DateTime time) {
            var sim = GetSimanuTicks(time);
            long t = time.TimeOfDay.Ticks;
            if (t >= sim.rise && t < sim.set) return (int)((t - sim.rise) / sim.dayHour);
            else return (int)((t + TimeSpan.TicksPerDay - sim.set) % TimeSpan.TicksPerDay / sim.nightHour) + 12;
        }

        public override int GetMinute(DateTime time) {
            var sim = GetSimanuTicks(time);
            long t = time.TimeOfDay.Ticks;
            long ush = TimeSpan.TicksPerMinute * 4;
            if (t >= sim.rise && t < sim.set) return (int)((t - sim.rise) % sim.dayHour / ush);
            else return (int)((t + TimeSpan.TicksPerDay - sim.set) % TimeSpan.TicksPerDay % sim.nightHour / ush);
        }

        public override int GetSecond(DateTime time) {
            var sim = GetSimanuTicks(time);
            long t = time.TimeOfDay.Ticks;
            long ush = TimeSpan.TicksPerMinute * 4;
            long gar = TimeSpan.TicksPerSecond * 4;
            if (t >= sim.rise && t < sim.set) return (int)((t - sim.rise) % sim.dayHour % ush / gar);
            else return (int)((t + TimeSpan.TicksPerDay - sim.set) % TimeSpan.TicksPerDay % sim.nightHour % ush / gar);
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return GetPlot(year).Moons[month - 1];
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetPlot(year).YearDays;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            switch (year % 19) {
                case 2:
                case 5:
                case 7:
                case 10:
                case 13:
                case 16:
                case 18:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return year % 19 == 16 ? month == 7 : month == 13;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            switch (year % 19) {
                case 2:
                case 5:
                case 7:
                case 10:
                case 13:
                case 18:
                    return 13;
                case 16:
                    return 7;
                default:
                    return 0;
            }
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Nīsannu", "Ayyāru", "Sīmannu", "Duʾūzu", "Ābu", "Ulūlū", "Tašrītu", "Araḫsamna", "Kisilīmu", "Ṭebētu", "Šabāṭu", "Addāru", "Addāru šanû" }, new string[] { "Nīs", "Ayy", "Sīm", "Duū", "Ābu", "Ulū", "Taš", "Ara", "Kis", "Ṭeb", "Šab", "Add", "Ads" }, new string[] { "Šamaš", "Suen", "Nergal", "Nabû", "Marduk", "Ishtar", "Ninurta"});
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            if (GetLeapMonth(ld.Year) == 7) {
                if (ld.Month == 7) {
                    fx.MonthFullName = "Ulūlū šanû";
                    fx.MonthShortName = "Uls";
                }
                else if (ld.Month > 7) {
                    fx.MonthFullName = dtfi.MonthNames[ld.Month - 2];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[ld.Month - 2];
                }
            }
            CustomizeTimes(fx, time, new string[] { "ASR", "BST", "AST", "BSR" }[GetHour(time) / 6]);
            return fx;
        }
    }
}
