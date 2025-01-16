using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class VyomanianCalendar : WeirdCalendar {

        public override string Author => "Maharaja Mohak";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Vyomanian_calendar");

        protected override DateTime SyncDate => new DateTime(2025, 1, 1);
        protected override int SyncOffset => 500;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public enum DayOfWeekWC {
            Manibu,
            Shukrani,
            Prithvi,
            Mangalar,
            Zabbuna,
            Manibri,
            Shanivat,
            Manar,
            Virunar
        }

        public override int DaysInWeek => 9;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Holiday")
        };

        public override string SpecialDay(DateTime time) => GetHoliday(time);

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 14;
        }

        // Author neglects to mention leap days, but since he says this calendar is to be used
        // "alongside the Gregorian," we will add the leap day to the last month.
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 14 ? 27 : IsLeapYear(year) ? 15 : 14;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 14 && day == 15;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            int w = WeekdayNumber(time);
            if (w > 6) throw BadWeekday;
            return (DayOfWeek)w;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        private int WeekdayNumber(DateTime time) {
            return (GetDayOfMonth(time) - 1) % (ToLocalDate(time).Month < 14 ? 9 : 7);
        }

        public string GetHoliday(DateTime time) {
            string h = NoSpecialDay;
            var (_, month, day, _) = ToLocalDate(time);
            if (month == 2 && day == 23) h = "Shivaji Day";
            else if (month == 5 && day == 13) h = "Maharashtra Day";
            else if (month == 6 && day == 19) h = "Vyomania Day";
            else if (month == 9 && day == 11) h = "Independence Day";
            else if (month == 11 && day == 5) h = "Gandhi Jayanti";
            else if (month == 11 && day == 18) h = "Kalam Day";
            else if (month == 14 && day > 13) h = "It's New-Year Day";
            return h;
        }

        // Author does not give month names
        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Month 1", "Month 2", "Month 3", "Month 4", "Month 5", "Month 6", "Month 7", "Month 8", "Month 9", "Month 10", "Month 11", "Month 12", "Month 13", }, new string[] { "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9", "M10", "M11", "M12", "M13" }, new string[] { "Manibu", "Shukrani", "Prithvi", "Mangalar", "Zabbuna", "Manibri", "Shanivat" }, new string[] { "Mnb", "Shu", "Pri", "Mng", "Zab", "Mbr", "Sha" });
        }

        private static string[] ExtraDays = { "Manar", "Virunar" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int w = WeekdayNumber(time) - 7;
            if (w >= 0) {
                fx.DayFullName = ExtraDays[w];
                fx.DayShortName = ExtraDays[w].Substring(0, 3);
            };
            if (ToLocalDate(time).Month == 14) {
                fx.MonthFullName = "Month 14";
                fx.MonthShortName = "M14";
            }
            fx.Format = format.ReplaceUnescaped("n", $"\"{GetHoliday(time)}\"");
            return fx;
        }
    }
}
