using System;
using System.Globalization;

namespace WeirdCalendars {
    public class WholeWeeksCalendar : Symmetry454Calendar {

        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Whole_Weeks_Calendar");

        public WholeWeeksCalendar() => Title = "Whole Weeks Calendar";

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 5 == 0 && (year % 40 != 0 || year % 400 == 0);
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Protose", "Deuterose", "Tritose", "Tetrakal", "Pentakal", "Hexakal", "Heptador", "Oktador", "Ennador", "Dekataire", "Hendekataire", "Dodekataire", "" }, new string[] {"Pts", "Dts", "Tts", "Tkl", "Pkl", "Hkl", "Hpr", "Odr", "Edr", "Dtr", "Hdr", "Ddr", ""});
        }
    }
}
