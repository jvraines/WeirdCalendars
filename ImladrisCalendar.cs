using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {

    public class ImladrisCalendar : FixedCalendar {

        public override string Author => "J.R.R. Tolkien";
        public override Uri Reference => new Uri("https://tolkiengateway.net/wiki/Reckoning_of_Rivendell");

        /// <summary>
        /// False (default) to synchronize with the Gregorian calendar or True to project analeptically.
        /// </summary>
        private bool isAnaleptic;
        public bool IsAnaleptic {
            get => isAnaleptic;
            set {
                isAnaleptic = value;
                Notes = $"According to the {(isAnaleptic ? "constructed chronology of James the Just." : "default modernizing rules of Paul Sarando.")}";
            }
        }

        protected override DateTime SyncDate => IsAnaleptic ? SyncAnalepetic.date : SyncGregorian.date;
        protected override int SyncOffset => IsAnaleptic ? SyncAnalepetic.offset : SyncGregorian.offset;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        // Following the chronology of James the Just at 
        // https://tolkienforums.activeboard.com/t42820320/middle-earth-chronology
        private (DateTime date, int offset) SyncAnalepetic = (new DateTime(2020, 3, 28), 11057);

        // Following the default rules of Paul Sarando at
        //https://psarando.github.io/shire-reckoning/
        private (DateTime date, int offset) SyncGregorian = (new DateTime(2022, 3, 24), 0);

        /// <summary>
        /// Construct with the default value of IsAnaleptic = False.
        /// </summary>
        public ImladrisCalendar() :this(false) { }

        /// <summary>
        /// Construct with a specified analepticity.
        /// </summary>
        /// <param name="analeptic">False to synchronize with the Gregorian calendar or True to project analeptically.</param>
        public ImladrisCalendar(bool analeptic) {
            IsAnaleptic = analeptic;
        }

        public override int DaysInWeek => 6;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Yén")
        };
        
        /// <summary>
        /// Gets the Yén number of a date.
        /// </summary>
        /// <param name="time">A DateTime value</param>
        /// <returns>An integer representing the Yén.</returns>
        public int GetYen(DateTime time) {
            ValidateDateTime(time);
            return ToLocalDate(time).Year / 144;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 6:
                    return 55;
                case 2:
                case 5:
                    return 72;
                case 3:
                    return IsLeapYear(year) ? 60 : 57;
                default:
                    return 54;
            }
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month > 1 ? 1 : 0;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 6;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 368 : 365;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 12 == 0 && year % 432 != 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day > 57 && month == 3 && IsLeapYear(year);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 1 && day == 0 || month == 6 && day == 55 || month == 3 && day > 54;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            bool isEnglish = Language.Length > 1 && Language.Substring(0, 2) == "en";
            if (day == 0) return isEnglish ? "First Day" : Language == "sjn" ? "Iestor" : "Yestarë";
            if (month == 6) return isEnglish ? "Last Day" : Language == "sjn" ? "Methor" : "Mettarë";
            return isEnglish ? "Middleday" : Language == "sjn" ? "Enedhor" : "Enderë";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 0 ? "Frst" : month == 6 ? "Last" : "Mid";
        }

        public override DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        protected override int WeekdayNumber(DateTime time) {
            // DateTime epoch (1/1/1) was DayOfWeek 4; no interruptions in cycle
            return (int)(time.Ticks / TimeSpan.TicksPerDay + 4) % 6;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            if (Language.Length > 1 && Language.Substring(0, 2) == "en") SetNames(dtfi, new string[] { "Spring", "Summer", "Autumn", "Fading", "Winter", "Stirring", "", "", "", "", "", "", "" }, null, new string[] { "Starsday", "Sunday", "Moonday", "Two Trees Day", "Heavensday", "Valarday", "" });
            else if (Language == "sjn") SetNames(dtfi, new string[] { "Ethuil", "Laer", "Iavas", "Firith", "Rhîw", "Echuir", "", "", "", "", "", "", "" }, null, new string[] { "Orgilion", "Oranor", "Orithil", "Orgaladhad", "Ormenel", "Orbelain", "" });
            else SetNames(dtfi, new string[] { "Tuilë", "Lairë", "Yávië", "Quellë", "Hrívë", "Coirë", "", "", "", "", "", "", "" }, null, new string[] { "Elenya", "Anarya", "Isilya", "Aldúya", "Menelya", "Valanya", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (format.FoundUnescaped("n")) fx.Format = fx.Format.ReplaceUnescaped("n", GetYen(time).ToString());
            return fx;
        }
    }
}
