using System;

namespace WeirdCalendars {
    public abstract class GondorCalendar : FixedCalendar {

        protected override DateTime SyncDate => IsAnaleptic ? SyncAnalepetic.date : SyncGregorian.date;
        protected override int SyncOffset => IsAnaleptic ? SyncAnalepetic.offset : SyncGregorian.offset;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        private bool isAnaleptic;
        public bool IsAnaleptic {
            get => isAnaleptic;
            set {
                isAnaleptic = value;
                Notes = $"According to the {(isAnaleptic ? "constructed chronology of James the Just." : "default modernizing rules of Paul Sarando.")}";
            }
        }

        // Following the chronology of James the Just at 
        // https://tolkienforums.activeboard.com/t42820320/middle-earth-chronology
        protected (DateTime date, int offset) SyncAnalepetic;
        
        // Following the default rules of Paul Sarando at 
        // https://psarando.github.io/shire-reckoning/
        protected (DateTime date, int offset) SyncGregorian;

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month > 1 ? 1 : 0;
        }

        public override int GetDaysInYear(int year, int era) {
            return IsLeapYear(year) ? 366 : IsDoubleLeapYear(year) ? 367 : 365;
        }

        /// <summary>
        /// Indicates whether a year is a leap year with 2 leap days.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if year is a leap year with 2 leap days.</returns>
        public virtual bool IsDoubleLeapYear(int year) {
            ValidateDateParams(year, 0);
            if (IsAnaleptic) return year % 1000 == 0;
            return false;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return time == DateTime.MinValue ? DayOfWeek.Sunday : time.AddDays(-1).DayOfWeek;
        }

        public override DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return time == DateTime.MinValue ? DayOfWeekWC.Sunday : (DayOfWeekWC)time.AddDays(-1).DayOfWeek;
        }

        internal override void CustomizeDTFI(System.Globalization.DateTimeFormatInfo dtfi) {
            if (Language.Length > 1 && Language.Substring(0, 2) == "en") SetNames(dtfi, new string[] { "New-Sun", "Wet", "Windy", "Budding", "Flower", "Sunny", "Reaping", "Hot", "Fruit-Giving", "Sun-Waning", "Misty", "Cold", "" }, new string[] { "New", "Wet", "Win", "Bud", "Flo", "Sun", "Rea", "Hot", "Fru", "Wan", "Mis", "Col", "" }, new string[] { "Starsday", "Sunday", "Moonday", "White Tree Day", "Heavensday", "Seaday", "Valarday" });
            else if (Language == "sjn") SetNames(dtfi, new string[] { "Narwain", "Nínui", "Gwaeron", "Gwirith", "Lothron", "Nórui", "Cerveth", "Urui", "Ivanneth", "Narbeleth", "Hithui", "Girithron", "" }, new string[] { "Nrw", "Nín", "Gwa", "Gwi", "Lot", "Nór", "Cer", "Uru", "Iva", "Nrb", "Hit", "Gir", "" }, new string[] { "Orgilion", "Oranor", "Orithil", "Orgaladh", "Ormenel", "Oraearon", "Orbelain" });
            else SetNames(dtfi, new string[] { "Narvinyë", "Nénimë", "Súlimë", "Víressë", "Lótessë", "Nárië", "Cermië", "Urimë", "Yavannië", "Narquelië", "Hísimë", "Ringarë", "" }, new string[] { "Nrv", "Nén", "Súl", "Vír", "Lót", "Nár", "Cer", "Uri", "Yav", "Nrq", "Hís", "Rin", "" }, new string[] { "Elenya", "Anarya", "Isilya", "Aldëa", "Menelya", "Eärenya", "Valanya" });
        }
    }
}
