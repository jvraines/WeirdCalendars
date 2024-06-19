using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {

    public class ThirteenMoonCalendar : FixedCalendar {

        public override string Author => "José Argüelles";
        public override Uri Reference => new Uri("http://www.lawoftime.org/thirteenmoon/tutorial.html");

        protected override DateTime SyncDate => new DateTime(2022, 7, 26);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)>{
            ("k", "Kin number"),
            ("n", "Galactic signature")
        };

        public ThirteenMoonCalendar() => Title = "13 Moon Calendar";

        public override int GetDayOfMonth(DateTime time) {
            var ymd = ToLocalDate(time);
            if (IsLeapYear(ymd.Year) && ymd.Month == 8 && ymd.Day > 22) {
                return ymd.Day == 23 ? -1 : ymd.Day - 1;
            }
            return ymd.Day;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13 || (month == 8 && IsLeapYear(year)) ? 29 : 28;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 23 && month == 8 && IsLeapYear(year);
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            return day == 29 && month == 13 || IsLeapDay(year, month, day);
        }

        private static string[] ColorName = { "Red", "White", "Blue", "Yellow" };
        private static string[] GalacticToneName = { "Magnetic", "Lunar", "Electric", "Self-Existing", "Overtone", "Rhythmic", "Resonant", "Galactic", "Solar", "Planetary", "Spectral", "Crystal", "Cosmic" };
        private static string[] SolarSealName = { "Dragon", "Wind", "Night", "Seed", "Serpent", "Worldbridger", "Hand", "Star", "Moon", "Dog", "Monkey", "Human", "Skywalker", "Wizard", "Eagle", "Warrior", "Earth", "Mirror", "Storm", "Sun" };
        
        /// <summary>
        /// Get the kin number of a date.
        /// </summary>
        /// <param name="time">A Datetime value.</param>
        /// <returns>An integer from 1 to 260.</returns>
        public int GetKin(DateTime time) {
            return (int)(AA.Net.Time.JulianDay(time) + 0.5 - 119) % 260 + 1;
        }

        /// <summary>
        /// Get the galactic tone of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer from 0 to 12.</returns>
        public int GetGalacticTone(DateTime time) {
            return (GetKin(time) - 1) % 13;
        }

        /// <summary>
        /// Get the solar seal of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>An integer from 0 to 19.</returns>
        public int GetSolarSeal(DateTime time) {
            return (GetKin(time) - 1) % 20;
        }

        /// <summary>
        /// Indicates whether a date is a clear sign from the tomb of Pacal Voltan.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>True if the date is a clear sign.</returns>
        public bool IsClearSign(DateTime time) {
            switch (GetKin(time)) {
                case 20: case 26: case 30: case 40:
                case 57: case 60: case 87: case 106:
                case 132: case 176: case 211: case 245:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether a date is a galactic activation portal.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>True if the date is a galactic activation portal.</returns>
        public bool IsGalacticActivationPortal(DateTime time) {
            switch (GetKin(time)) {
                case 1: case 20: case 22: case 39: case 43: case 50: case 51:
                case 58: case 64: case 69: case 72: case 77: case 85: case 88:
                case 93: case 96: case 106: case 107: case 108: case 109: case 110:
                case 111: case 112: case 113: case 114: case 115: case 146: case 147:
                case 148: case 149: case 150: case 151: case 152: case 153:
                case 154: case 155: case 165: case 168: case 173: case 176:
                case 184: case 189: case 192: case 197: case 203: case 210:
                case 211: case 218: case 222: case 239: case 241: case 260:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the galactic signature of a date.
        /// </summary>
        /// <param name="time">A DateTime value.</param>
        /// <returns>A string containing the color, galactic tone, and solar seal of the date.</returns>
        public string GetGalacticSignature(DateTime time) {
            return $"{ColorName[GetSolarSeal(time) % 4]} {GalacticToneName[GetGalacticTone(time)]} {SolarSealName[GetSolarSeal(time)]}";
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 13 ? "Day Out of Time" : "Hunab Ku";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 13 ? "Out" : "Hun";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, GalacticToneName, null, new string[] { "Dali", "Seli", "Gamma", "Kali", "Alpha", "Limi", "Silio" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx =  base.GetFormatWC(dtfi, time, format);
            string gs = GetGalacticSignature(time);
            int kn = GetKin(time);
            fx.LongDatePattern += $" '{gs} {kn}-Kin{(IsClearSign(time) ? "★" : "")}{(IsGalacticActivationPortal(time) ? " ⦒⦑" : "")}'";
            fx.Format = format.ReplaceUnescaped("n", $"'{gs}'").ReplaceUnescaped("k", $"{kn}");
            return fx;
        }
    }
}
