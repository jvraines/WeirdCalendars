using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;
using System.Linq;
using WeirdCalendars.Support;

namespace WeirdCalendars {
    public class ThellidCalendar : FixedCalendar {
        
        public override string Author => "Warren Mars";
        public override Uri Reference => new Uri("https://infinitedimensions.org/devices/thellid_calendar/thellid_structure.htm");

        protected override DateTime SyncDate => new DateTime (2022, 12, 22);
        protected override int SyncOffset => 10001;
        public override DateTime MaxSupportedDateTime => new DateTime(6000, 1, 1); //Limit of VSOP87 accuracy

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 14 ? 28 : 1;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 15 : 14;
        }

        private Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int gYear = year - SyncOffset;
            if (!LeapYears.TryGetValue(gYear, out bool ly)) {
                int yearStart = (int)(Earth.SeasonStart(gYear, Earth.Season.December) + 0.5);
                int yearEnd = (int)(Earth.SeasonStart(gYear + 1, Earth.Season.December) + 0.5);
                ly = yearEnd - yearStart == 366;
                LeapYears.Add(gYear, ly);
            }
            return ly;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 1 && month == 14 && IsLeapYear(year);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 1 && month > 13;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return IsLeapYear(year) && month == 14 ? "Leap Day" : "Old Year's Day";
        }

        private static string[] MonthName = new string[] { "Alvakku", "Bethanis", "Duvadda", "Emovvi", "Forkithal", "Kalvazzi", "Irentos", "Jukennuk", "Miskullen", "Ossakov", "Raikkaved", "Underro", "Zithebbe", "Werrimul", "Nabbakan" };
        
        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, MonthName.Take(13).ToArray(), null, new string[] { "Pasku", "Lokkan", "Gunji", "Hithed", "Shevro", "Teijal", "Vaira" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            var ymd = ToLocalDate(time);
            if (ymd.Month > 13) {
                string m = MonthName[IsLeapYear(ymd.Year) && ymd.Month == 14 ? 14 : 13];
                fx.MonthFullName = m;
                fx.MonthShortName = m.Substring(0, 3);
            }
            return fx;
        }
    }
}
