using System;
using System.Globalization;

namespace WeirdCalendars {
    public class TabotCalendar : WeirdCalendar {

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/tabot.htm");

        protected override DateTime SyncDate => new DateTime(2022, 11, 2);
        protected override int SyncOffset => -1930;
        public override DateTime MinSupportedDateTime => new DateTime(1930, 11, 2);

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month == 12) return 35;
            return IsLeapYear(year) && month == 4 ? 31 : 30;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 4 && day == 31;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return ((year + 3) % 4 == 0 && (year + 31) % 100 != 0) || (year + 331) % 400 == 0;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Anbassa", "Hymanot", "Immanuel", "Ras", "Ta'Berhan", "Manassa", "Danaffa", "Negest", "Tafari", "Emru", "Sawwara", "Negus & Dejazmatch", "" }, new string[] { "Anb", "Hym", "Imm", "Ras", "TaB", "Man", "Dan", "Neg", "Taf", "Emr", "Saw", "N&D", "" }, new string[] { "Ergat", "Tazajenat", "Kedusenant", "Ra'ee", "Makrab", "Mamlak", "Germa" }, new string[] { "Erg", "Taz", "Ked", "Rae", "Mak", "Mam", "Ger" });
        }
    }
}
