using System;
using System.Globalization;

namespace WeirdCalendars {
    public class SexagesimalCalendar : FixedCalendar {

        public override string Author => "Edouard Vitrant";
        public override Uri Reference => new Uri("http://www.sexagesimal.org/en_propos.php");

        protected override DateTime SyncDate => new DateTime(2020, 12, 21);
        protected override int SyncOffset => -2011;
        public override DateTime MinSupportedDateTime => new DateTime(2012, 12, 21);
        protected override int DaysInWeek => 6;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 6 || IsLeapYear(year) ? 61 : 60;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 6;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 6 && day == 61;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return (month < 6 || IsLeapYear(year)) && day == 61;
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return new string[] { "Bacchanal", "Ceres", "Musica", "Liber", "Memento Mori", "Sext" }[month - 1];
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day).Substring(0, 3);
        }

        protected override int WeekdayNumber(DateTime time) {
            ValidateDateTime(time);
            return base.WeekdayNumber(time) + 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Frigée", "Éclose", "Florée", "Granée", "Récole", "Caduce", "", "", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx =  base.GetFormatWC(dtfi, time, format);
            FixNegativeYears(fx, time);
            return fx;
        }
    }
}
