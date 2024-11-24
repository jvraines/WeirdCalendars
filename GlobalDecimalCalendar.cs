using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class GlobalDecimalCalendar : FixedCalendar {

        public override string Author => "Robert Erasmus";
        public override Uri Reference => new Uri("https://docs.google.com/file/d/0B279bivvtgNVcm9ONkFkYXlsLVk/edit?resourcekey=0-tNa7yXW5FjiPxp0XQnj5qQ");

        public override string Notes => "Adopting 169 West longitude as the Prime Meridian.";

        protected override DateTime SyncDate => new DateTime(2022, 3, 21, 11, 16, 0);
        protected override int SyncOffset => -2012;
        public override DateTime MinSupportedDateTime => new DateTime(2012, 3, 20);
        
        public override int DaysInWeek => 6;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> { ("c", "Time in crons") };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 1 || (IsLeapYear(year) && month == 6) ? 37 : 36;
        }
        
        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 10;
        }

        /// <summary>
        /// Gets the cron number of a time.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>A double representing the cron value of the time of day.</returns>
        public double GetCrons(DateTime time) {
            ValidateDateTime(time);
            var l = ToLocalDate(time);
            return l.TimeOfDay.Ticks * 100 / (double)TimeSpan.TicksPerDay;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0;
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            string n = base.IntercalaryDayName(year, month, day);
            if (n == "Leap Day") return n;
            return $"{new[] { "1st", "2nd", "3rd", "4th", "5th" }[month / 2]} Festival";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            string n = base.IntercalaryDayName(year, month, day);
            if (n == "Leap Day") return "Leap";
            return $"Fs{month / 2}";
        }

        public override bool IsLeapYear(int year, int era) {
            return base.IsLeapYear(year - 1, era);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 6 && day == 0;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month == 6 && IsLeapYear(year) || (month & 1) == 1 ? 0 : 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Primadis", "Secundis", "Tertidis", "Quadridis", "Quintidis", "Sesidis", "Septidis", "Octodis", "Nonadis", "Decidis", "", "", "" }, null, new string[] { "Oneday", "Twoday", "Threeday", "Fourday", "Fiveday", "Sixday", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            FixNegativeYears(fx, time);
            string crons = GetCrons(time).ToString("F3");
            fx.Format = format.ReplaceUnescaped("c", crons);
            fx.LongTimePattern = crons;
            fx.ShortTimePattern = crons;
            return fx;
        }
    }
}