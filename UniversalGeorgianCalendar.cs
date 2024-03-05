using System;
using System.Globalization;


namespace WeirdCalendars {
    public class UniversalGeorgianCalendar : FixedCalendar {
        
        public override string Author => "Hugh Jones";
        public override Uri Reference => new Uri("https://myweb.ecu.edu/mccartyr/hirossa.html");

        protected override DateTime SyncDate => new DateTime(2023, 12, 21);
        protected override int SyncOffset => -1754;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && year % 132 != 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 30;
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return day == 29 ? "Christmas" : "Thanksgiving Festival";
        }

        internal protected override string IntercalaryAbbreviatedDayName (int year, int month, int day) {
            return day == 29 ? "Xmas" : "Fest";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "St. Peter", "St. Andrew", "St. James", "St. John", "St. Philip", "St. Bartholomew", "St. Thomas", "St. Matthew", "St. James the Less", "St. Jude", "St. Simon", "St. Matthias", "St. Paul" }, new string[] { "Pet", "And", "Jam", "Joh", "Phi", "Bar", "Tho", "Mat", "Jal", "Jud", "Sim", "Mas", "Pau" });
        }
    }
}
