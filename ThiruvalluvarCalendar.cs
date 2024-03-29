using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class ThiruvalluvarCalendar : FixedCalendar {
        public override string Author => "Arivumani Velmurugan";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Thiruvalluvar_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 15);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("q", "Quarter")
        };

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month == 1) return IsLeapYear(year) ? 30 : 29;
            return 28;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day);
            return day > 28;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return day == 29 ? "Thamizh Sarvadesa Thinam" : "Netti";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return day == 29 ? "TST" : "Net";
        }

        public string GetQuarter(DateTime time) {
            int d = GetDayOfYear(time) - (IsLeapYear(ToLocalDate(time).Year) ? 1 : 0);
            if (d <= 91) return "Paavoozh";
            if (d <= 91 * 2) return "Uzharan";
            if (d <= 91 * 3) return "Natparan";
            return "Natkarpu";
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Paamathi", "Ilmathi", "Thuramathi", "Uzhmathi", "Arasumathi", "Amaichumathi", "Aranmathi", "Koozhmathi", "Padaimathi", "Natpumathi", "Kudimathi", "Kalamathi", "Karpumathi" }, new string[] { "Paa", "Ilm", "Thu", "Uzh", "Ars", "Ama", "Arn", "Koo", "Pad", "Nat", "Kud", "Kal", "Kar" }, new string[] { "Aranthinam", "Aalvinam", "Idanthinam", "Eegaithinam", "Uzhavinam", "Ukinam", "Otrinam" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            fx.Format = format.ReplaceUnescaped("q", $"'{GetQuarter(time)}'");
            return fx;
        }
    }
}
