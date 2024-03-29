using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static System.Net.WebRequestMethods;

namespace WeirdCalendars {
    public class BalancedFrenchCalendar : FixedCalendar {

        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Balanced_French_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 9, 21);
        protected override int SyncOffset => 1;

        public new enum DayOfWeekWC {
            Blankday = -1,
            primidi,
            duodi,
            tridi,
            quartidi,
            quintidi,
            sextidi,
            septidi,
            octidi,
            nonidi,
            décadi
        }

        protected override int DaysInWeek => 10;

        protected override int GetFirstDayOfMonth(int year, int month) => month % 3 == 1 ? 0 : 1;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            int wn = WeekdayNumber(time);
            if (wn > 6) throw BadWeekday;
            return (DayOfWeek)wn;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month % 3 == 1 || month == 12 || month == 6 && IsLeapYear(year) ? 31 : 30;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day == 0 || day == 31;
        }

        private static string[] Complementary = {"Sudior", "Sudjour", "Nordose", "Nordjour" };
        private static string[] ComplementaryAbbr = { "Sdr", "Sjr", "Nds", "Njr" };

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            if (day == 0) return Complementary[(month - 1) / 3];
            return month == 12 ? "Nordaire" : "Sudial";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            if (day == 0) return ComplementaryAbbr[(month - 1) / 3];
            return month == 12 ? "Ndr" : "Sdl";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Unsudaire", "Dusudaire", "Tresudaire", "Unsudose", "Dusudose", "Tresudose", "Unordial", "Dunordial", "Trenordial", "Unordior", "Dunordior", "Trenordior", "" }, new string[] { "Usr", "Dsr", "Tsr", "Uss", "Dss", "Tss", "Unl", "Dnl", "Tnl", "Udr", "Ddr", "Tdr", "" });
        }

        private static string[] Days = { "primidi", "duodi", "tridi", "quartidi", "quintidi", "sextidi", "septidi", "octidi", "nonidi", "décadi" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (fx.DayFullName == null) {
                int w = WeekdayNumber(time);
                fx.DayFullName = Days[w];
                fx.DayShortName = Days[w].Substring(0, 3);
            }
            return fx;
        }
    }
}
