using System;

namespace WeirdCalendars {
    public class KingsReckoningCalendar : GondorCalendar {

        public override string Author => "J.R.R. Tolkien";
        public override Uri Reference => new Uri("https://tolkiengateway.net/wiki/Kings%27_Reckoning");

        /// <summary>
        /// False (default) to synchronize with the Gregorian calendar or True to project analeptically.
        /// </summary>
        public KingsReckoningCalendar() : this(false) { }

        /// <summary>
        /// Construct with a specified analepticity.
        /// </summary>
        /// <param name="analeptic">False to synchronize with the Gregorian calendar or True to project analeptically.</param>
        public KingsReckoningCalendar(bool analeptic) {
            SyncAnalepetic = (new DateTime(2020, 12, 19), 10468);
            SyncGregorian = (new DateTime(2022, 12, 21), 1);
            IsAnaleptic = analeptic;
            Title = "Kings' Reckoning Calendar";
        }

        /// <summary>
        /// Era 1 is the Second Age. Era 2 is the Third Age (from 3442 S.A.).
        /// </summary>
        public override int[] Eras => new int[] { 2, 1 };

        private int FixupYear(int year) {
            if (IsAnaleptic) year += 3441;
            return year;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(FixupYear(year), month, era);
            switch (month) {
                case 1:
                case 7:
                case 12:
                    return 31;
                case 6:
                    return IsLeapYear(year, era) ? 33 : IsDoubleLeapYear(year, era) ? 34 : 32;
                default:
                    return 30;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(FixupYear(year), month, day, era);
            return month == 6 && day > 32 && (IsLeapYear(year) || IsDoubleLeapYear(year, era));
        }

        /// <summary>
        /// Indicates whether a year is a leap year with 1 leap day.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if year is a leap year with 1 leap day.</returns>
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(FixupYear(year), era);
            if (IsAnaleptic) return year % 4 == 0 && year % 100 != 0;
            return base.IsLeapYear(year, era);
        }

        /// <summary>
        /// Indicates whether a year is a leap year with 2 leap days.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if year is a leap year with 2 leap days.</returns>
        public bool IsDoubleLeapYear(int year, int era) {
            ValidateDateParams(FixupYear(year), era);
            if (IsAnaleptic) return false;
            return year % 1000 == 0 || year == 2059 && era == 2 || year == 5500 && era == 0;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(FixupYear(year), month, day, 0);
            return month == 1 && day == 0 || month == 6 && day > 31 || month == 12 && day == 31;
        }

        private string IntercalaryDayName(int year, int month, int day, int era) {
            bool isEnglish = Language.Length > 1 && Language.Substring(0, 2) == "en";
            if (day == 0) return isEnglish ? "First Day" : Language == "sjn" ? "Iestor" : "Yestarë";
            if (month == 12) return isEnglish ? "Last Day" : Language == "sjn" ? "Methor" : "Mettarë";
            if (IsLeapYear(year) || IsDoubleLeapYear(year, era)) return isEnglish ? "Middleday" : Language == "sjn" ? "Enedhor" : "Enderë";
            return isEnglish ? "Midyear's Day" : Language == "sjn" ? "Lawenedh": "Loëndë";
        }

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            ValidateDateTime(time);
            var ymd = base.ToLocalDate(time);
            // Problem: this offset does not roundtrip through validation.
            if (IsAnaleptic) {
                // DateTimes are all in Era 2
                ymd.Year -= 3441;
            }
            return ymd;
        }

        internal override FormatWC GetFormatWC(System.Globalization.DateTimeFormatInfo dtfi, DateTime time, string format) {
            //If this is an intercalary day, fix up day name
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day, 2);
                fx.DayShortName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day, 2).Substring(0, 3);
            }
            return fx;
        }
    }
}
