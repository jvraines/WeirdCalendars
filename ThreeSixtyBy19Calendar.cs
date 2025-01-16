using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class ThreeSixtyBy19Calendar : WeirdCalendar {
 
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://groups.io/g/calendars/message/1584");

        protected override DateTime SyncDate => new DateTime(2024, 9, 21);
        protected override int SyncOffset => 1;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public ThreeSixtyBy19Calendar() => Title = "360 × 19 Lunisolar Calendar";

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month > 7 && IsLeapYear(year)) month--;
            switch (month) {
                case 1:
                case 3:
                case 7:
                case 9:
                case 11:
                    return 30;
                case 5:
                    return year % 19 == 0 ? 29 : 30;
                case 6:
                    return year % 4 == 0 && year % 304 != 0 ? 30 : 29;
                default:
                    return 29;
            }
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            int d = IsLeapYear(year) ? 384 : 354;
            if (year % 19 == 0) d--;
            if (year % 4 == 0 && year % 304 != 0) d++;
            return d;
        }

        protected static int[,] LeapYear = {
            { 1, 3, 6, 9, 11, 14, 17 },
            { 1, 3, 6, 9, 12, 14, 17 },
            { 1, 4, 6, 9, 12, 14, 17 },
            { 1, 4, 6, 9, 12, 15, 17 },
            { 1, 4, 7, 9, 12, 15, 17 },
            { 1, 4, 7, 9, 12, 15, 18 },
            { 1, 4, 7, 10, 12, 15, 18 },
            { 2, 4, 7, 10, 12, 15, 18 },
            { 2, 4, 7, 10, 13, 15, 18 },
            { 2, 5, 7, 10, 13, 15, 18 },
            { 2, 5, 7, 10, 13, 16, 18 },
            { 2, 5, 8, 10, 13, 16, 18 },
            { 2, 5, 8, 10, 13, 16, 19 },
            { 2, 5, 8, 11, 13, 16, 19 },
            { 3, 5, 8, 11, 13, 16, 19 },
            { 3, 5, 8, 11, 14, 16, 19 },
            { 3, 6, 8, 11, 14, 16, 19 },
            { 3, 6, 8, 11, 14, 17, 19 },
            { 3, 6, 9, 11, 14, 17, 19 }
        };

        protected virtual int CycleLength { get; } = 360;

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int eraYear = (year - 1899) % 6840;
            int cycle = eraYear / CycleLength;
            int y = eraYear % CycleLength % 19 + 1;
            for (int i = 0; i < 7; i++) if (LeapYear[cycle, i] == y) return true;
            return false;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return IsLeapYear(year) && month == 7;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 7 : 0;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Arctos Protos", "Arctos Duteros", "Arctos Tritos", "Arctos Tetros", "Arctos Pentos", "Arctos Hektos", "Notos Protos", "Notos Duteros", "Notos Tritos", "Notos Tetros", "Notos Pentos", "Notos Hektos", "Arctos Hektos Embolimos" }, new string[] { "APr", "ADu", "ATr", "ATe", "APe", "AHe", "NPr", "NDu", "NTr", "NTe", "NPe", "NHe", "AHE" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            if (IsLeapYear(ymd.Year)) {
                if (ymd.Month == 7) {
                    fx.MonthFullName = dtfi.MonthNames[12];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[12];
                }
                else if (ymd.Month > 7) {
                    ymd.Month -= 2;
                    fx.MonthFullName = dtfi.MonthNames[ymd.Month];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[ymd.Month];
                }
            }
            return fx;
        }
    }
}
