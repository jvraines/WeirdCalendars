using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WeirdCalendars {
    public class DiscworldCalendar : WeirdCalendar {
        public override string Author => "Terry Pratchett et al.";
        public override Uri Reference => new Uri("https://discworld.starturtle.net/lpc/playing/documentation.c?path=/concepts/calendar");

        protected override DateTime SyncDate => new DateTime(2023, 3, 24);
        protected override int SyncOffset => 24 ;

        public override string Notes => "According to the Discworld MUD.";
        protected override double TimescaleFactor => 0.3;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Special day")
        };

        protected override int DaysInWeek => 8;

        public enum DayOfWeekWC {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday,
            Octeday
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int n = WeekdayNumber(time);
            if (n == 7) throw BadWeekday;
            return (DayOfWeek)n;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) => (DayOfWeekWC)WeekdayNumber(time);

        private int WeekdayNumber(DateTime time) {
            return (GetDayOfMonth(time) + 4) % 8;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 26;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 13 == 0 ? 16 : 32;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 800;
        }

        public string GetSpecialDay(int month, int day) {
            string result = NoSpecialDay;
            if (day == 1) {
                if (month == 1) result = "Hogswatchday";
                else if (month == 14) result = "Crueltide";
                else if (month == 10 || month == 23) result = "Sektober Fools' Day";
            }
            else if (day == 24) {
                if (month == 6) result = "Small Gods Day";
                else if (month == 19) result = "Alls Fallow";
            }
            else if (day == 23 && (month == 8 || month == 21)) result = "Soul Cake Tuesday";
            return result;
        }

        private static string[] MonthNames = { "Offle Prime", "February Prime", "March Prime", "April Prime", "May Prime", "June Prime", "Grune Prime", "August Prime", "Spune Prime", "Sektober Prime", "Ember Prime", "December Prime", "Ick Prime", "Offle Secundus", "February Secundus", "March Secundus", "April Secundus", "May Secundus", "June Secundus", "Grune Secundus", "August Secundus", "Spune Secundus", "Sektober Secundus", "Ember Secundus", "December Secundus", "Ick Secundus" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, MonthNames.Take(13).ToArray());
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            var ymd = ToLocalDate(time);
            fx.MonthFullName = MonthNames[ymd.Month - 1];
            fx.MonthShortName = $"{fx.MonthFullName.Substring(0, 3)}{(ymd.Month < 14 ? "P" : "S")}";
            if (WeekdayNumber(time) == 7) {
                fx.DayFullName = "Octeday";
                fx.DayShortName = "Oct";
            }
            fx.Format = fx.Format.ReplaceUnescaped("n", $"'{GetSpecialDay(ymd.Month, ymd.Day)}'");
            return fx;
        }
    }
}
