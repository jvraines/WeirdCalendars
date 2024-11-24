using System;
using System.Globalization;
using System.Linq;
using AA.Net;

namespace WeirdCalendars {
    public class TerranComputationalCalendar : WeirdCalendar {
        
        public override string Author => "Jim Hoffman";
        public override Uri Reference => new Uri("https://terrancalendar.com/");

        protected override DateTime SyncDate => new DateTime(1969, 12, 22);
        protected override int SyncOffset => -1969;

        protected override int FirstMonth => 0;
        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 1);
            return 0;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 14;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 2 : 1;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && year % 128 != 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 13 && day == 2;
        }

        protected override void ValidateDateParams(params int[] param) {
            if (param.Length == 4) {
                if (param[1] == 13 && param[2] > GetLastDayOfMonth(param[0], 13)) {
                    // push fictive leap second date into next year
                    param[0]++;
                    param[1] = 0;
                    param[2] = 0;
                }
            }
            base.ValidateDateParams(param);
        }

        private static int latestLeapSecondYear = Time.LeapSecondDates.Max(x => x.date).Year - 1970;
        
        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            if (month == 13 && day > GetLastDayOfMonth(year, 13)) {
                // push fictive leap second date
                year++;
                month = 0;
                day = 0;
            }
            // adjust seconds for differential placement of leap seconds
            if (year >= 0 && year <= latestLeapSecondYear) {
                DateTime start = base.ToDateTime(year, 0, 0, 0, 0, 0, 0, 1);
                DateTime finish = base.ToDateTime(year, month, day, hour, minute, second, millisecond, 1);
                int leaps = (int)Time.LeapSecondDates.Where(x => x.date >= start && x.date <= finish).Sum(x => x.increment);
                return finish.AddSeconds(leaps);
            }
            return base.ToDateTime(year, month, day, hour, minute, second, millisecond, era);
        }

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            var ld = base.ToLocalDate(time);
            // adjust TimeOfDay for differential placement of leap seconds
            if (ld.Year >= 0 && ld.Year <= latestLeapSecondYear) {
                DateTime start = base.ToDateTime(ld.Year, 0, 0, 0, 0, 0, 0, 1);
                DateTime finish = base.ToDateTime(ld.Year, ld.Month, ld.Day, ld.TimeOfDay.Hours, ld.TimeOfDay.Minutes, ld.TimeOfDay.Seconds, ld.TimeOfDay.Milliseconds, 1);
                int leaps = (int)Time.LeapSecondDates.Where(x => x.date >= start && x.date <= finish).Sum(x => x.increment);
                if (leaps > 0) {
                    var ld2 = base.ToLocalDate(finish.AddSeconds(leaps));
                    if (ld2.Year != ld.Year) {
                        ld2.Year--;
                        ld2.Month = 13;
                        ld2.Day = GetLastDayOfMonth(ld2.Year, 13) + 1;
                    }
                    return ld2;
                }
            }
            return ld;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            // Force single date/time format
            fx.Format = $"'{ld.Year}.{ld.Month}.{ld.Day},{ld.TimeOfDay.Hours}.{ld.TimeOfDay.Minutes}.{ld.TimeOfDay.Seconds} TC'";
            return fx;
        }

    }
}
