using System;
using System.Globalization;

namespace WeirdCalendars {
    public class SiroccanAtomicCalendar : WeirdCalendar {
        public override string Author => "Daniel Anderson";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Atomic_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 7, 16);
        protected override int SyncOffset => -1945;

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 3:
                case 5:
                case 9:
                case 11:
                    return 30;
                case 7:
                    return IsLeapYear(year) ? 29 : 28;
                default:
                    return 31;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return base.IsLeapYear(year + 1946, era);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 7 && day == 29;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Trinity", "Baker", "Hurricane", "Grable", "Castle", "Argus", "Plowshare", "Sedan", "Prime", "Manhattan", "Nevada", "Julin", "" }, new string[] { "Tri", "Bak", "Hur", "Gra", "Cas", "Arg", "Plo", "Sed", "Pri", "Man", "Nev", "Jln", "" });
        }
    }
}
