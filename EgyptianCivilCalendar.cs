using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class EgyptianCivilCalendar : WeirdCalendar
    {

        public override string Author => "Traditional";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/Egyptian_calendar");

        // From https://en.wikipedia.org/wiki/Sothic_cycle
        protected override DateTime SyncDate => new DateTime(2022, 4, 18);
        protected override int SyncOffset => 2780;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> {
            ("n", "Decan number"),
            ("nn", "Decan deity")
        };
        
        public override int GetDaysInMonth(int year, int month, int era)
        {
            ValidateDateParams(year, month, era);
            return month < 13 ? 30 : 5;
        }

        public override int GetMonthsInYear(int year, int era)
        {
            ValidateDateParams(year, era);
            return 13;
        }

        public override int GetDaysInYear(int year, int era)
        {
            ValidateDateParams(year, era);
            return 365;
        }

        // Decans are divisions of the year
        public int GetDecanNumber(DateTime time) => (GetDayOfYear(time) - 1) / 10 + 1;

        // Names from https://www.occult.live/index.php/Egyptian_decans. Placing Sirius at 1 Thoth.
        private static string[] DecanDeity = new string[] { "Sopdet", "Tepy-a Kenmet", "Kenmet", "Khery Heped En Kenmet", "Hat Djat", "Pehuy Djat", "Temat Heret", "Temat Kheret", "Weshati Bekati", "Ip-Djes", "Tepy-a Khentet", "Khentet heret", "Khentet kheret", "Tjemes en Khentet", "Qedty", "Hery-ib Wia", "Seshmu", "Kenmu", "Tepy-a Semed", "Semed", "Seret", "Sawy Seret", "Khery Heped Seret", "Tepy-a Akhuy", "Akhuy", "Bawy", "Khentu Heru", "Khentu Kheru", "Qed", "Sawy Qed", "Khau", "Aryt", "Remen Hery Sah", "Remen Khery Sah", "Sah", "Tepy-a Sopdet", "Shetwy" };

        // Deities are tied to rising asterisms subject to the Sothic Cycle
        public string GetDecanDeity(DateTime time) {
            const double SothicCycle = 1461;
            int year = ToLocalDate(time).Year;
            int offset = 365 - (int)(year % SothicCycle / SothicCycle * 365);
            return DecanDeity[(GetDayOfYear(time) + offset) % 365 / 10];
        }

        public override bool IsLeapDay(int year, int month, int day, int era)
        {
            ValidateDateParams(year, month, day, year);
            return false;
        }

        public override bool IsLeapYear(int year, int era)
        {
            ValidateDateParams(year, era);
            return false;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi)
        {
            SetNames(dtfi, new string[] { "Thoth", "Phaophi", "Athyr", "Choiak", "Tybi", "Mechir", "Pharmenoth", "Pharmuthi", "Pachons", "Payni", "Epiphi", "Mesore", "Epagomena" }, new string[] { "Tho", "Pho", "Ath", "Cho", "Tyb", "Mec", "Phe", "Phu", "Pac", "Pay", "Epi", "Mes", "Epa" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format)
        {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("nn")) format = format.ReplaceUnescaped("nn", $"'{GetDecanDeity(time)}'");
            if (format.FoundUnescaped("n")) format = format.ReplaceUnescaped("n", GetDecanNumber(time).ToString());
            fx.Format = format;
            return fx;
        }
    }
}
