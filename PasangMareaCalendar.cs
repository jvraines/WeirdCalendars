using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WeirdCalendars {
    public class PasangMareaCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Pasang_Marea_Calendar");

        protected override DateTime SyncDate => new DateTime(2026, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 24 : 25;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month == 24 && IsLeapYear(year) && !base.IsLeapYear(year, 0)) return 14;
            return month % 4 == 2 ? 14 : 15;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? base.IsLeapYear(year, 0) ? 354 : 353 : 369;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        private static string[] Month = { "Nu", "Danu", "Mazu", "Neptu", "Namu", "Egi", "Amansi", "Ebi", "Yami", "Velli", "Kimba", "Kiwa", "Varua", "Sedna", "Ligna", "Ikate", "Sedse", "Ague", "Oke", "Noede", "Yemo", "Bsompo", "Kuko", "Cao", "Aro" };

        private static string[] MonthAbbr = { "Nu", "Dan", "Maz", "Nep", "Nam", "Egi", "Ama", "Ebi", "Yam", "Vel", "Kim", "Kiw", "Var", "Sda", "Lig", "Ika", "Sds", "Agu", "Oke", "Noe", "Yem", "Bso", "Kuk", "Cao", "Aro" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, Month.Take(13).ToArray(), MonthAbbr.Take(13).ToArray());
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int m = ToLocalDate(time).Month;
            if (m > 13) {
                fx.MonthFullName = Month[m - 1];
                fx.MonthShortName = MonthAbbr[m - 1];
            }
            return fx;
        }
    }
}
