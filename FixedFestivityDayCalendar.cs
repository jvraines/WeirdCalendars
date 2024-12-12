using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class FixedFestivityDayCalendar : FixedCalendar {
        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Fixed_Festivity_Day_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("c", "Compact format")
        };

        private bool IsMonth30(int year, int month) => (month & 1) == 0 || month == 3 && !IsLeapYear(year);

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return IsMonth30(year, month) ? 30 : 31;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 3 && day == 1;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            if (IsMonth30(year, month)) {
                switch (day) {
                    case 8:
                    case 23:
                    case 31:
                        return true;
                    default:
                        return false;
                }
            }
            else {
                switch (day) {
                    case 1:
                    case 9:
                    case 17:
                    case 24:
                        return true;
                    default:
                        return false;
                }
            }
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return "Holiday";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return "Hol";
        }

        private int AdjustedDay(DateTime time) {
            var ld = ToLocalDate(time);
            int d = ld.Day;
            if (IsMonth30(ld.Year, ld.Month)) d -= d > 23 ? 2 : d > 8 ? 1 : 0;
            else d -= d > 24 ? 3 : d > 9 ? 2 : 1;
            return d;
        }

        protected override int WeekdayNumber(DateTime time) {
            return AdjustedDay(time) % 7;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("c")) {
                int ad = AdjustedDay(time);
                int w = (ad - 1) / 7 + 1;
                var ld = ToLocalDate(time);
                int d;
                if (IsIntercalaryDay(ld.Year, ld.Month, ld.Day)) d = 0;
                else d = (ad - 1) % 7 + 1;
                fx.Format = format.ReplaceUnescaped("c", $"{ld.Month:00}-{w}-{d}");
            }
            return fx;
        }
    }
}
