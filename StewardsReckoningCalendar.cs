using System;

namespace WeirdCalendars {
    public class StewardsReckoningCalendar : GondorCalendar {
        public override string Author => "J.R.R. Tolkien";
        public override Uri Reference => new Uri("https://tolkiengateway.net/wiki/Stewards%27_Reckoning");

        /// <summary>
        /// False (default) to synchronize with the Gregorian calendar or True to project analeptically.
        /// </summary>
        public StewardsReckoningCalendar() : this(true) { }

        /// <summary>
        /// Construct with a specified analepticity.
        /// </summary>
        /// <param name="analeptic">False to synchronize with the Gregorian calendar or True to project analeptically.</param>
        public StewardsReckoningCalendar(bool analeptic) {
            SyncAnalepetic = (new DateTime(2020, 12, 19), 7027);
            SyncGregorian = (new DateTime(2022, 12, 21), 0);
            IsAnaleptic = analeptic;
            Title = "Steward's Reckoning Calendar";
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 3:
                case 9:
                case 12:
                    return 31;
                case 6:
                    return IsLeapYear(year, era) ? 32 : IsDoubleLeapYear(year) ? 33 : 31;
                default:
                    return 30;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day > 31 && (IsLeapYear(year) || IsDoubleLeapYear(year));
        }

        /// <summary>
        /// Indicates whether a year is a leap year with 1 leap day.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if year is a leap year with 1 leap day.</returns>
        public override bool IsLeapYear(int year, int era) {
            if (IsAnaleptic) {
                ValidateDateParams(year, era);
                return year % 4 == 0 && year % 100 != 0 && year != 2360;
            }
            return base.IsLeapYear(year, era);
        }

        /// <summary>
        /// Indicates whether a year is a leap year with 2 leap days.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if year is a leap year with 2 leap days.</returns>
        public override bool IsDoubleLeapYear(int year) {
            ValidateDateParams(year, 0);
            if (IsAnaleptic) return year % 1000 == 0 && year != 3000 || year == 2059 || year == 2360;
            return false;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            switch (month) {
                case 1:
                    return day == 0;
                case 3:
                case 9:
                case 12:
                    return day == 31;
                case 6:
                    return day > 30;
                default:
                    return false;
            };
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            bool isEnglish = Language.Length > 1 && Language.Substring(0, 2) == "en";
            switch (month) {
                case 1:
                    return isEnglish ? "First Day" : Language == "sjn" ? "Iestor" : "Yestarë";
                case 3:
                    return isEnglish ? "Springday" : Language == "sjn" ? "Ethuilor" : "Tuilérë";
                case 6:
                    if (IsLeapYear(year) || IsDoubleLeapYear(year)) return isEnglish ? "Middleday" : Language == "sjn" ? "Enedhor" : "Enderë";
                    return isEnglish ? "Midyear's Day" : Language == "sjn" ? "Lawenedh" : "Loëndë";
                case 9:
                    return isEnglish ? "Autumnday" : Language == "sjn" ? "Iavassor" : "Yáviérë";
                case 12:
                    return isEnglish ? "Last Day" : Language == "sjn" ? "Methor" : "Mettarë";
                default:
                    throw new ArgumentOutOfRangeException("month", "Month does not contain an intercalary day.");
            }
        }
    }
}
