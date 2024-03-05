using System;
using System.Globalization;

namespace WeirdCalendars {
    public class ShireCalendar : FixedCalendar {
        
        public override string Author => "J.R.R. Tolkien";
        public override Uri Reference => new Uri("https://tolkiengateway.net/wiki/Shire_Calendar");

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

        // Following the chronology of James the Just at 
        // https://tolkienforums.activeboard.com/t42820320/middle-earth-chronology
        private (DateTime date, int offset) SyncAnalepetic = (new DateTime(2020, 12, 5), 5427);

        // Following the default rules of Paul Sarando at
        //https://psarando.github.io/shire-reckoning/
        private (DateTime date, int offset) SyncGregorian = (new DateTime(2022, 12, 21), 0);
        
        /// <summary>
        /// Construct with the default value of IsAnaleptic = True                                   
        /// </summary>
        public ShireCalendar() : this(true) { }

        /// <summary>
        /// Construct with a specified analepticity.
        /// </summary>
        /// <param name="analeptic">False to synchronize with the Gregorian calendar or True to project analeptically.</param>
        public ShireCalendar(bool analeptic) {
            IsAnaleptic = analeptic;
        }

        private bool IsBlankDay(int year, int month, int day) {
            return month == 6 && (day == 32 || day == 33 && IsLeapYear(year));
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            var ymd = ToLocalDate(time);
            if (IsBlankDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            return (DayOfWeek)WeekdayNumber(time);
        }

        public override DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ymd = ToLocalDate(time);
            int w = IsBlankDay(ymd.Year, ymd.Month, ymd.Day) ? -1 : WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }

        protected override int WeekdayNumber(DateTime time) {
            int dayOfYear = GetDayOfYear(time);
            if (dayOfYear > 182) dayOfYear -= IsLeapYear(ToLocalDate(time).Year) ? 2 : 1;
            return (dayOfYear - 1) % 7;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month > 1 ? 1 : 0;
        }
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 12:
                    return 31;
                case 6:
                    return IsLeapYear(year) ? 34 : 33;
                default:
                    return 30;
            }
        }
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (IsAnaleptic) return year % 4 == 0 && year % 100 != 0;
            return base.IsLeapYear(year, era);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 33 && month == 6 && IsLeapYear(year);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 1 && day == 0 || month == 12 && day == 31 || month == 6 && day > 30;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            if (month == 1 || month == 12) return $"{(month == 12 ? 1 : 2)} Yule";
            if (day == 31 || day == 34) return $"{(day == 31 ? 1 : 2)} Lithe";
            if (day == 32) return "Mid-year's Day";
            return IsLeapYear(year) ? "Overlithe" : "2 Lithe";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Afteryule", "Solmath", "Rethe", "Astron", "Thrimidge", "Forelithe", "Afterlithe", "Wedmath", "Halimath", "Winterfilth", "Blotmath", "Foreyule", "" }, null, new string[] { "Sterday", "Sunday", "Monday", "Trewsday", "Hevensday", "Mersday", "Highday" });
        }

        private static string[] BreeMonth = new string[] { "Frery", "Solmath", "Rethe", "Chithing", "Thrimidge", "Lithe", "Mede", "Wedmath", "Harvestmath", "Wintring", "Blooting", "Yulemath" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (Language == "en-BR") {
                string month = BreeMonth[ToLocalDate(time).Month - 1];
                fx.LongDatePattern = FixMonth(fx.LongDatePattern);
                fx.ShortDatePattern = FixMonth(fx.ShortDatePattern);
                fx.Format = FixMonth(format);
                
                string FixMonth(string f) {
                    f = f.ReplaceUnescaped("MMMM", $"\"{month}\"").ReplaceUnescaped("MMM", $"\"{month.Substring(0, 3)}\"");
                    return f;
                }
            }
            return fx;
        }

    }
}
