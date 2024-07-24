using System;
using System.Globalization;

namespace WeirdCalendars {
    public class GreyhawkCalendar : WeirdCalendar {

        public override string Author => "Gary Gygax et al.";
        public override Uri Reference => new Uri("https://greyhawkonline.com/greyhawkwiki/Greyhawk_calendar");

        protected override DateTime SyncDate => new DateTime(1985, 6, 14, 19, 11, 17);
        protected override int SyncOffset => -1406;

        protected override double TimescaleFactor => 1.0 / 28; // per https://greyhawkonline.com/greyhawkwiki/Common_Year#Conversion_to_timelines_of_other_worlds, reckoning Midsummer 579 = Midsummer 1985

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 16;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 4 == 1 ? 7 : 28;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 364;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((ToLocalDate(time).Day - 1) % 7);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Needfest", "Fireseek", "Readying", "Coldeven", "Growfest", "Planting", "Flocktime", "Wealsun", "Richfest", "Reaping", "Goodmonth", "Harvester", "Brewfest" }, new string[] { "Nee", "Fir", "Rdy", "Col", "Gro", "Pla", "Flo", "Wea", "Ric", "Rpg", "Goo", "Har", "Bre" }, new string[] { "Starday", "Sunday", "Moonday", "Godsday", "Waterday", "Earthday", "Freeday" });
        }

        private static string[] ExtraMonths = { "Patchwall", "Ready'reat", "Sunsebb" };
        private static string[] ExtraMonthsAbbr = { "Pat", "Rrt", "Sun" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            if (ld.Month > 13) {
                fx.MonthFullName = ExtraMonths[ld.Month - 14];
                fx.MonthShortName = ExtraMonthsAbbr[ld.Month - 14];
            }
            CustomizeTimes(fx, time);
            return fx;
        }
    }
}
