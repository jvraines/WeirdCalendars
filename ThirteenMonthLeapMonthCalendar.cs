using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class ThirteenMonthLeapMonthCalendar : WeirdCalendar {
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Thirteen_Month_Leap-Month_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 12, 11);
        protected override int SyncOffset => 1;

        public ThirteenMonthLeapMonthCalendar() => Title = "Thirteen Month Leap-Month Calendar";

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 14 : 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return 28;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 392 : 364;
        }
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetLeapMonth(year) != 0;
        }

        private static readonly Dictionary<int, int> LeapMonths = new Dictionary<int, int> {
            {022, 07},
            {045, 01},
            {067, 08},
            {090, 02},
            {112, 09},
            {135, 03},
            {157, 10},
            {180, 04},
            {202, 11},
            {225, 05},
            {247, 12},
            {270, 06},
            {293, 13}
        };

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            int cycleYear = (year - 1) % 293 + 1;
            return LeapMonths.TryGetValue(cycleYear, out int m) ? m + 1 : 0;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)(GetDayOfMonth(time) % 7);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Unua", "Dua", "Tria", "Kvara", "Kvina", "Sesa", "Sepa", "Oka", "Naua", "Deka", "Dekuna", "Dekdua", "Dektria" }, new string[] { "Unu", "Dua", "Tri", "Kva", "Kvi", "Ses", "Sep", "Oka", "Nau", "Dek", "Dun", "Ddu", "Dtr" }); ;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            int leapMonth = GetLeapMonth(ld.Year);
            if (leapMonth > 0) {
                int offset = ld.Month - 2;
                if (leapMonth == ld.Month) {
                    fx.MonthFullName = $"Salti-{dtfi.MonthNames[offset]}";
                    fx.MonthShortName = $"S-{dtfi.AbbreviatedMonthNames[offset]}";
                }
                else if (ld.Month > leapMonth) {
                    fx.MonthFullName = dtfi.MonthNames[offset];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[offset];
                }
            }
            return fx;
        }
    }
}
