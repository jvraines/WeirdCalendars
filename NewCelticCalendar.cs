using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class NewCelticCalendar : WeirdCalendar {

        public override string Author => "Timey and Karl Palmen";
        public override Uri Reference => new Uri("https://time-meddler.co.uk/the-new-celtic-calendar/");

        protected override DateTime SyncDate => new DateTime(2023, 11, 13);
        protected override int SyncOffset => 1027;
        public override DateTime MinSupportedDateTime => new DateTime(33, 11, 9); //first CE grand cycle

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;
        public override CalendarRealization Realization => CalendarRealization.Conjectural;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLongYear(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era){
            ValidateDateParams(year, month, era);
            int y = year - 1060; //offset from first CE grand cycle
            int months = y / 353 * 4366 +
                         y % 353 / 182 * 2251 +
                         y % 353 % 182 / 19 * 235;
            for (int yy = 1;
                 yy <= y % 353 % 182 % 19;
                 yy++) {
                months += IsLongYear(yy) ? 13 : 12;
            }
            months += month;
            //370 is months offset from first Yerm CE cycle to first Celtic CE grand cycle
            int yermMonth = (370 + months) % 850 % 49 % 17 + 1;
            return (yermMonth & 1) == 1 ? 30 : 29;
        }

        private static Dictionary<int, int> YearDays = new Dictionary<int, int>();

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!YearDays.TryGetValue(year, out int d)) {
                ValidationEnabled = false;
                d = 0;
                for (int m = 1; m <= GetMonthsInYear(year); m++) d += GetDaysInMonth(year, m);
                ValidationEnabled = true;
                YearDays.Add(year, d);
            }
            return d;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 0;
        }

        private bool IsLongYear(int year) {
            int cy = year % 353 % 182 % 19;
            switch (cy) {
                case 2:
                case 5:
                case 7:
                case 10:
                case 13:
                case 16:
                case 18:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLongYear(year);
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Samhain", "Dumhainn", "Riùr", "Naghaid", "Uarain", "Cuithe", "Geamhain", "Siùfainn", "Eacha", "Eilmì", "Aodhrain", "Cadal", "Eadràn" }, null, new string[] { "Didòmhnaich", "Diluain", "Dimàirt", "Diciadain", "Diardaoin", "Dihaoine", "Disathairne" });
        }
    }
}
