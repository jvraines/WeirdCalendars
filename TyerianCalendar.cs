using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class TyerianCalendar : FixedCalendar {
        
        public override string Author => "TySkyo";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Tyerian_Calendar");

        // Author's claimed epoch is 1300, but holiday correlations only work if sync happens in the year of authorship.
        protected override DateTime SyncDate => new DateTime(2015, 12, 21);
        protected override int SyncOffset => -1300;
        public override int DaysInWeek => 5;

        protected override double TimescaleFactor => 1 / 0.9972685185;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("n", "Dating system")
        };

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            if (year % 6520 == 0) return 368;
            else if (year % 620 == 0) return 367;
            else if (year % 32 == 0) return 366;
            else if (year % 4 == 0) return 365;
            else return 364;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams (year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams (year, month, era);
            switch (month) {
                case 1:
                    return year % 620 == 0 || year % 6520 == 0 ? 31 : 30;
                case 13:
                    return year % 32 == 0 || year % 620 == 0 ? 31 : year % 6520 == 0 ? 32 : 30;
                case 7:
                    return year % 4 == 0 || year % 32 == 0 || year % 620 == 0 || year % 6520 == 0 ? 5 : 4;
                default:
                    return 30;
            }    
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            return month == 1 && GetDaysInMonth(year, 1) == 31 ? 0 : 1;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0 || day > 30;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 0 ? "First Day" : day > 30 ? "Last Day" : "";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 0 ? "Fst" : day > 30 ? "Lst" : "";
        }

        protected override int WeekdayNumber(DateTime time) {
            return (GetDayOfMonth(time) - 1) % 5 + 1;
        }

        protected override long HourTicks => (long)(TimeSpan.TicksPerDay / TimescaleFactor / 10);      // Myriasecond
        protected override long MinuteTicks => (long)(TimeSpan.TicksPerDay / TimescaleFactor / 1000);   // Hectosecond
        protected override long SecondTicks => (long)(TimeSpan.TicksPerDay / TimescaleFactor / 100000); // Second
        protected override long MilliTicks => (long)(TimeSpan.TicksPerDay / TimescaleFactor / 100000000);// Millisecond

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Decari", "Ianari", "Febrari", "Martari", "Aprilari", "Mai'ari", "Mercidoni", "Iunari", "Iulari", "Augustari", "Septari", "Octari", "Novari" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            if (format.FoundUnescaped("n")) {
                var ld = ToLocalDate(time);
                string ds = $"{ld.Day}/{ld.Month}/{ld.Year}/{ld.Year / 50}";
                fx.Format = fx.Format.ReplaceUnescaped("n", ds);
            }
            return fx;
        }
    }
}
