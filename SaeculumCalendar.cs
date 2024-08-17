using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class SaeculumCalendar : WeirdCalendar {

        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Saeculum-Calendar");

        protected override DateTime SyncDate => new DateTime(2013, 9, 25); // This is 36 months before the stated start date 9 September 2016, being the 37th month Janus
        protected override int SyncOffset => -2013;

        public override string Notes => "Using Palmen corrections and reconstructed month names.";

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 76 : 73;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 74 || month == 75 ? 30 : 31;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 2282 : 2190;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year + 9) % 18 == 0;
        }

        private static string[] MonthNames = { "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Fons", "Feronia", "Invictus", "Carmenta", "Februa", "Mars", "Ceres", "Maia", "Juno", "Neptunus", "Diana", "Jupiter", "Raphael", "Nebelung", "Christmond", "Hartung", "Hornung", "Knospenmond", "Gabriel", "Wonnemond", "Brachmond", "Heuert", "Ernting", "Michael" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int month = ToLocalDate(time).Month;
            if (month == 37) {
                fx.MonthFullName = "Janus";
                fx.MonthShortName = "Jan";
            }
            else if (month > 73) {
                fx.MonthFullName = $"Great-month {(month - 73).ToRoman()}";
                fx.MonthShortName = $"GM{month - 73}";
            }
            else {
                int root = month - (month < 37 ? 1 : 38);
                fx.MonthFullName = month < 37 ? MonthNames[root] : $"Re{MonthNames[root].ToLower()}";
                fx.MonthShortName = $"{(month > 37 ? "R" : "")}{MonthNames[root].Substring(0, 3)}";
            }
            return fx;
        }
    }
}
