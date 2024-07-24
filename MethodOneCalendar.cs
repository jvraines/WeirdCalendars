using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class MethodOneCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Method_One_Calendar");

        protected override DateTime SyncDate => new DateTime(2001, 1, 1);
        protected override int SyncOffset => 0;

        /// <summary>
        /// Specify a cycle in short format by using one of these codes alone. The CurrentCycle is not changed.
        /// </summary>
        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("R", "Reference"),
            ("L", "Lunar cycle"),
            ("W", "Week cycle"),
            ("S", "Solar cycle")
        };

        public enum Cycle {
            Reference,
            Lunar,
            Week,
            Solar
        }

        /// <summary>
        /// Determines which cycle is used in method calls.
        /// </summary>
        private Cycle currentCycle = Cycle.Solar;
        public Cycle CurrentCycle {
            get => currentCycle;
            set { 
                currentCycle = value;
                LastTime = DateTime.MaxValue; // force evaluation in ToLocalDate
            }
        }
        
        private Cycle? saveCycle;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) && CurrentCycle == Cycle.Lunar ? 13 : 12;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            switch (CurrentCycle) {
                case Cycle.Reference:
                    return IsLeapYear(year) ? 366 : 360;
                case Cycle.Lunar:
                    return IsLeapYear(year) ? 385 : 354;
                case Cycle.Week:
                    return 364;
                default: //Solar
                    return IsLeapYear(year) ? 366 : 365;
            }
        }
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (CurrentCycle) {
                case Cycle.Reference:
                    return month == 12 && IsLeapYear(year) ? 36 : 30;
                case Cycle.Lunar:
                    return month == 13 ? 31 : (month & 1) == 0 ? 29 : 30;
                case Cycle.Week:
                    return month % 3 == 1 ? 33 : 29;
                default: //Solar
                    switch (month) {
                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            return 30;
                        case 2:
                            return 29;
                        case 12:
                            return IsLeapYear(year) ? 31 : 30;
                        default:
                            return 31;
                    }
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            m[12] = "Luna";
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            ma[12] = "Lun";
            SetNames(dtfi, m, ma);
        }

        private static string[] CycleCode = { "R", "L", "W", "S"};

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            for (int c = 0; c < 4; c++) {
                if (format == CycleCode[c]) {
                    saveCycle = CurrentCycle;
                    CurrentCycle = (Cycle)c;
                    if (CurrentCycle == Cycle.Week) {
                        int dd = GetDayOfYear(time) - 1;
                        int q = dd / 91 + 1;
                        int w = dd % 91 / 7 + 1;
                        int d = dd % 91 % 7 + 1;
                        fx.Format = $@"Yyy-Q{q:00}-W{w:00}-\D{d:00}";
                    }
                    else fx.Format = @"Yyy-\MMM-Ddd";
                    break;
                }
            }
            return fx;
        }

        internal override void OnDateFormatted() {
            if (saveCycle != null) {
                CurrentCycle = (Cycle)saveCycle;
                saveCycle = null;
            }
        }
    }
}
