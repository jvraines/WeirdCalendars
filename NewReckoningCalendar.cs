using System;

namespace WeirdCalendars {
    public class NewReckoningCalendar : GondorCalendar {
        public override string Author => "J.R.R. Tolkien";
        public override Uri Reference => new Uri("https://tolkiengateway.net/wiki/New_Reckoning");

        /// <summary>
        /// False (default) to synchronize with the Gregorian calendar or True to project analeptically.
        /// </summary>
        public NewReckoningCalendar() : this(false) { }

        /// <summary>
        /// Construct with a specified analepticity.
        /// </summary>
        /// <param name="analeptic">False to synchronize with the Gregorian calendar or True to project analeptically.</param>
        public NewReckoningCalendar(bool analeptic) {
            // Following the chronology of James the Just at 
            // https://tolkienforums.activeboard.com/t42820320/middle-earth-chronology
            SyncAnalepetic = (new DateTime(2020, 3, 13), 4006);
            SyncGregorian = (new DateTime(2022, 3, 16), 0);
            IsAnaleptic = analeptic;
        }
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 12:
                    return 31;
                case 6:
                    return IsLeapYear(year, era) ? 34 : IsDoubleLeapYear(year) ? 35 : 33;
                default:
                    return 30;
            }
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && (day == 31 || day == 35) && (IsLeapYear(year) || IsDoubleLeapYear(year));
        }

        /// <summary>
        /// Indicates whether a year is a leap year with 1 leap day.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if year is a leap year with 1 leap day.</returns>
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (IsAnaleptic) return year % 4 == 0 && year % 100 != 0;
            return base.IsLeapYear(year, era);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 1 && day == 0 || month == 12 && day == 31 || month == 6 & day > 30;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            bool isEnglish = Language.Length > 1 && Language.Substring(0, 2) == "en";
            switch (month) {
                case 1:
                    return isEnglish ? "First Day" : Language == "sjn" ? "Iestor" : "Yestarë";
                case 6:
                    if (IsLeapYear(year) || IsDoubleLeapYear(year)) {
                        switch (day) {
                            case 31:
                                return Ringday();
                            case 33:
                                return Midyearday();
                            default:
                                return Middleday();
                        }
                    }
                    if (day == 32) return Midyearday();
                    return Middleday();
                case 12:
                    return isEnglish ? "Last Day" : Language == "sjn" ? "Methor" : "Mettarë";
                default:
                    throw new ArgumentOutOfRangeException("Not an intercalary day.");
            }

            string Ringday() => isEnglish ? "Ringday" : Language == "sjn" ? "Corvor" : "Cormarë";
            string Middleday() => isEnglish ? "Middleday" : Language == "sjn" ? "Enedhor" : "Enderë";
            string Midyearday() => isEnglish ? "Midyear's Day" : Language == "sjn" ? "Lawenedh" : "Loëndë";
        }

        internal override void CustomizeDTFI(System.Globalization.DateTimeFormatInfo dtfi) {
            base.CustomizeDTFI(dtfi);
            dtfi.MonthNames = Rotate(dtfi.MonthNames);
            dtfi.MonthGenitiveNames = dtfi.MonthNames;
            dtfi.AbbreviatedMonthNames = Rotate(dtfi.AbbreviatedMonthNames);
            dtfi.AbbreviatedMonthGenitiveNames = dtfi.AbbreviatedMonthNames;
            
            string[] Rotate(string[] a) {
                string[] m = new string[13];
                for (int i = 0; i < 9; i++) m[i] = a[i + 3];
                for (int i = 9; i < 12; i++) m[i] = a[i - 9];
                m[12] = "";
                return m;
            }
        }
    }
}
