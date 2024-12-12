using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class FixedFestivityWeekCalendar : LeapWeekCalendar {
        
        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Fixed_Festivity_Week_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("c", "Compact format")
        };

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 16;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 4 > 0 ? 28 : month == 16 && IsLeapYear(year) ? 14 : 7;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsISOLeapYear(year);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 16 && day > 7;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)(GetDayOfMonth(time) % 7);
        }

        private static string[] ExtMonths = { "November", "December", "Christmas" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "January", "February", "March", "Easter", "April", "May", "June", "Community", "July", "August", "September", "Thanksgiving", "October" });
        }

        private bool InLocalDate = false;

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            if (!InLocalDate) {
                InLocalDate = true;
                int w = (GetDayOfYear(time) - 1) / 7 + 1;
                if (w > 51 || w < 13) time = time.AddHours(-1);
                else if (w > 25 && w < 39) time = time.AddHours(1);
                InLocalDate = false;
            }
            return base.ToLocalDate(time);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            var (_, m, d, _) = ToLocalDate(time);
            if (m > 13) {
                fx.MonthFullName = ExtMonths[m - 14];
                fx.MonthShortName = fx.MonthFullName.Substring(0, 3);
            }
            string iso = $"{(m % 4 == 0 ? $"0-0" : $"{m / 4 + 1}-{m % 4}")}-{(d - 1) / 7 + 1}-{d}";
            fx.Format = format.ReplaceUnescaped("c", iso);
            return fx;
        }
    }
}
