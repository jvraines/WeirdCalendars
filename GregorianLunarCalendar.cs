using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class GregorianLunarCalendar : WeirdCalendar {
        
        public override string Author => "David Madore";
        public override Uri Reference => new Uri("http://www.madore.org/~david/misc/calendar.html#gregorian.lunar");
        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        protected override DateTime SyncDate => new DateTime(1999, 12, 8);
        protected override int SyncOffset => 1;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("e", "Epact")
        };

        private int GoldenNumber (int year) {
            return year % 19 + 1;
        }

        /// <summary>
        /// Find the epact of a given year.
        /// </summary>
        /// <param name="year">A year in the Gregorian calendar.</param>
        /// <returns>A number between 0 and 29, or -1 to indicate the special value 25*.</returns>
        public int Epact (int year) {
            int goldenNumber = GoldenNumber(year);
            int julianEpact = 11 * (goldenNumber - 1) % 30;
            int century = year / 100 + 1;
            int S = 3 * century / 4;
            int L = (8 * century + 5) / 25;
            int epact = (julianEpact - S + L + 38) % 30;
            return epact == 25 && goldenNumber > 11 ? -1 : epact;
        }

        private bool IsEmbolismic(int year) {
            int epact = Epact(year);
            if (epact > 15 && epact < 25 || epact == -1) return true;
            if (epact > -1 && epact < 12 || epact > 24) return false;
            return !IsEmbolismic(year + 1);
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsEmbolismic(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 1:
                    return GoldenNumber(year) == 1 && !IsEmbolismic(year - 1) ? 29 : 30;
                case 2:
                    return IsLeapYear(year) ? 30 : 29;
                case 3:
                case 5:
                case 7:
                case 9:
                case 11:
                    return 30;
                case 13:
                    return IsHollow(year) ? 29 : 30;
                default:
                    return 29;
            }
        }

        private bool IsHollow(int year) => GoldenNumber(year) == 19;

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            int days = 354;
            if (IsLeapYear(year)) days++;
            if (IsEmbolismic(year)) days += IsHollow(year) ? 29 : 30;
            if (GoldenNumber(year) == 1 && !IsEmbolismic(year - 1)) days--;
            return days;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (year % 4 == 0) {
                if (year % 100 == 0) {
                    switch(year / 100 % 25) {
                        case 2:
                        case 5:
                        case 8:
                        case 11:
                        case 14:
                        case 18:
                        case 21:
                        case 24:
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Terminus", "Lipidus", "Venuch", "Amber", "Pook", "Jupe", "Tibery", "Claudy", "Septil", "Octil", "Novil", "Decil", "Mercuary" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int e = Epact(ToLocalDate(time).Year);
            string epact = e > -1 ? e.ToString() : "25*";
            fx.Format = fx.Format.ReplaceUnescaped("e", epact);
            return fx;
        }
    }
}
