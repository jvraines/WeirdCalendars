using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using AA.Net;

namespace WeirdCalendars {
    public class RomulusCalendar : WeirdCalendar {
        
        public override string Author => "Traditional";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/Roman_calendar");

        // Years start with the new moon closest to the March equinox. The eleventh month is "Winter." Official month lengths are reported by GetDaysInMonth, but GetRealDaysInMonth shortens by 1 for every month except Winter to account for new moon overlap. DayOfWeekWC follows the nundinal cycle from 1 Martius through December.

        // Stock time methods (GetHour etc.) return modern values in the Roman time zone. GetTime returns the hour of the "natural day."

        protected override DateTime SyncDate => new DateTime(2024, 3, 9, 22, 0, 0);
        protected override int SyncOffset => 753;

        public override DateTime MaxSupportedDateTime => VSOPLimit;
        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;
        public override CalendarRealization Realization => CalendarRealization.Conjectural;

        public enum DayOfWeekWC {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            int w = WeekdayNumber(time);
            if (w > 6) throw BadWeekday;
            return (DayOfWeek)w;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        private int WeekdayNumber(DateTime time) => (GetRealDayOfYear(time) - 1) % 8;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 11;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 5:
                case 8:
                    return 31;
                case 11:
                    return GetYearDays(year) - 294;
                default:
                    return 30;

            }
        }

        protected override int GetRealDaysInMonth(int year, int month, int era) => GetDaysInMonth(year, month, era) - (month < 11 ? 1 : 0);

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetYearDays(year);
        }

        protected override int GetRealDayOfYear(DateTime time) {
            ValidateDateTime(time);
            var ymd = ToLocalDate(time);
            int d = 0;
            for (int m = 1; m < ymd.Month; m++) d += GetRealDaysInMonth(ymd.Year, m, 0);
            return d + ymd.Day;
        }

        private static Dictionary<int, int> YearDays = new Dictionary<int, int>();

        private int GetYearDays(int year) {
            if (!YearDays.TryGetValue(year, out int days)) {
                double[] yearStart = new double[2];
                for (int i = 0; i < 2; i++) {
                    double equinox = Earth.SeasonStart(year - SyncOffset + i, Earth.Season.March);
                    double candidate1 = Moon.NextPhase(Moon.Phase.NewMoon, equinox - 16);
                    double candidate2 = Moon.NextPhase(Moon.Phase.NewMoon, equinox);
                    yearStart[i] = (equinox - candidate1 < candidate2 - equinox ? candidate1 : candidate2).ToLastUTMidnight();
                }
                days = (int)(yearStart[1] - yearStart[0]);
                YearDays.Add(year, days);
            }
            return days;
        }

        public string GetTime(DateTime time) {
            const double RomeLongitude = -12.486944, RomeLatitude = 41.888333; // Palatine Hill
            ValidateDateTime(time);
            var (r, _, s) = Sky.RiseTransitSet(Bodies.Sun, time, RomeLongitude, RomeLatitude, false);
            double sunrise = ((DateTime)r).JulianDay();
            double sunset = ((DateTime)s).JulianDay();
            double clock = time.JulianDay();
            double prop;
            string ind;
            if (clock >= sunrise) {
                if (clock < sunset) {
                    prop = (clock - sunrise) / (sunset - sunrise);
                    ind = "d.h.";
                }
                else {
                    prop = (clock - sunset) / (sunrise + 1 - sunset);
                    ind = "n.h.";
                }
            }
            else {
                prop = (clock - sunset + 1) / (sunrise - sunset + 1);
                ind = "n.h.";
            }
            return $"{((int)(prop * 12) + 1).ToRoman()} {ind}";
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Martius", "Aprilis", "Maius", "Iunius", "Quintilis", "Sextilis", "September", "October", "November", "December", "Brumalis", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            if (ymd.Day == 1 && ymd.Month > 1) {
                string dfull = $"{GetDaysInMonth(ymd.Year, ymd.Month - 1)}/1";
                string dshort = dfull.Replace("/", "-");
                string mfull = $"{ymd.Month - 1}/{ymd.Month}";
                string mshort = $"{ymd.Month - 1}-{ymd.Month}";
                fx.LongDatePattern = FixDigits(fx.LongDatePattern, null, null, null, null, dfull, dfull);
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, mshort, mshort, dshort, dshort);
                fx.Format = FixDigits(fx.Format, null, null, mfull, mshort, dfull, dshort);
                fx.MonthFullName = $"{dtfi.MonthNames[ymd.Month - 2]}/{dtfi.MonthNames[ymd.Month - 1]}";
                fx.MonthShortName = $"{dtfi.AbbreviatedMonthNames[ymd.Month - 2]}/{dtfi.AbbreviatedMonthNames[ymd.Month - 1]}";
            }
            if (ymd.Month == 11) {
                fx.DayFullName = "Nil";
                fx.DayShortName = "Nil";
            }
            else {
                fx.DayFullName = GetDayOfWeekWC(time).ToString();
                fx.DayShortName = fx.DayFullName;
            }
            fx.LongTimePattern = $"'{GetTime(time)}'";
            fx.ShortTimePattern = fx.LongTimePattern;
            return fx;
        }
    }
}
