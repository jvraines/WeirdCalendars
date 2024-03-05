using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NewUniversalCalendar : FixedCalendar {
        
        public override string Author => "Antumi Toasijé";
        public override Uri Reference => new Uri("https://www.nucalendar.org/2018/01/nucalendar.html");

        protected override DateTime SyncDate => new DateTime(2021, 12, 21, 6, 0, 0);
        protected override int SyncOffset => 10000;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 30;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day);
            return day > 28;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 29 ? "Year's Day" : "Second Year's Day";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 29 ? "Year" : "2Year";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Equality/Impartiality", "Freedom/Democracy", "Life/Nature", "Education/Knowledge", "Integrity/Health", "Justice/Redistribution", "Migrations/Travels", "Welfare/Progress", "Identities/Diversity", "Family/Fraternity", "Work/Entreprenuership", "Beliefs/Ideals", "Peace/Security" }, null, new string[] { "Africa", "Asia", "Europe", "North America", "South America", "Australia", "Antarctica"} );
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
