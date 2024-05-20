using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class WeekendRestCalendar : WeirdCalendar {

        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Weekend_Rest_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        private int StartDay(int year) {
            int c = (year - 1) % 62;
            return (c + (c + 14) / 17) % 6;
        }

        private int StartDay(int year, int month) {
            return (StartDay(year) + (month - 1) * 2) % 6;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int d;
            if (month < 12) d = StartDay(year, month) < 4 ? 30 : 31;
            else {
                int s = StartDay(year);
                if (IsLeapYear(year)) {
                    d = s == 2 || s == 3 ? 32 : 33;
                }
                else {
                    d = s == 2 || s == 3 || s == 4 ? 31 : 32;
                }
            }
            return d;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return StartDay(year) == 5 || IsLeapYear(year) ? 366 : 365;
        }

        public override int GetDayOfMonth(DateTime time) {
            var ymd = ToLocalDate(time);
            return ymd.Day - (ymd.Day + StartDay(ymd.Year, ymd.Month)) / 7;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 62 % 17 == 3;
        }

        public override bool IsLeapDay(int year, int month, int date, int era) {
            ValidateDateParams(year, month, ToDay(year, month, date), era);
            return date == 28;
        }

        private int ToDay(int year, int month, int date) {
            int d = Math.Abs(date);
            return d + (d + StartDay(year, month)) / 6;
        }

        /// <param name="date">Negative to indicate that Sunday should be returned from a weekend value.</param>
        /// <returns>A sequential day Monday-Saturday. If <paramref name="date"/> is passed as a negative number, then Sunday is returned from a weekend date.</returns>
        public override DateTime ToDateTime(int year, int month, int date, int hour, int minute, int second, int millisecond, int era) {
            DateTime d = base.ToDateTime(year, month, ToDay(year, month, date), hour, minute, second, millisecond, era);
             return date > 0 && GetDayOfWeek(d) == DayOfWeek.Sunday ? d.AddDays(-1) : d;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }
    }
}