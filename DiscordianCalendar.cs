using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class DiscordianCalendar : FixedCalendar {

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 1166;
        protected override int DaysInWeek => 5;

        public override string Author => "Malaclypse The Younger";
        public override Uri Reference => new Uri("https://www.principiadiscordia.com/book/41.php");

        public override string Notes => null;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Holyday")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return IsLeapYear(year) && month == 1 ? 74 : 73;
        }

        private static string[] Apostles = { "Mungday", "Mojoday", "Syaday", "Zaraday", "Maladay" };
        private static string[] Seasons = { "Chaoflux", "Discoflux", "Confuflux", "Bureflux", "Afflux"};

        public string GetHolyDay(DateTime time) {
            ValidateDateTime(time);
            var ld = ToLocalDate(time);
            switch(ld.Day) {
                case 5:
                    return Apostles[ld.Month - 1];
                case 50:
                    return Seasons[ld.Month - 1];
                default:
                    return NoSpecialDay;
            }
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            return IsLeapDay(year, month, day);
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return "St. Tib's Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return "Tib";
        }

        protected override int WeekdayNumber(DateTime time) {
            int d = time.DayOfYear;
            int offset = IsLeapYear(time.Year + SyncOffset) && d > 60 ? -2 : -1;
            return (d + offset) % DaysInWeek;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 5;
        }

        public override bool IsLeapYear(int year) {
            return base.IsLeapYear(year - SyncOffset);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 1 && day == 60;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Chaos", "Discord", "Confusion", "Bureaucracy", "The Aftermath", "", "", "", "", "", "", "", "" }, new string[] { "Cha", "Dsc", "Cfn", "Bcy", "Afm", "", "", "", "", "", "", "", "" }, new string[] { "Sweetmorn", "Boomtime", "Pungenday", "Prickle-Prickle", "Setting Orange", "", "" }, new string[] { "SM", "BT", "PD", "PP", "SO", "", "" });
        }
        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (format.FoundUnescaped("n")) fx.Format = format.ReplaceUnescaped("n", $"'{GetHolyDay(time)}'");
            return fx;
        }
    }
}
