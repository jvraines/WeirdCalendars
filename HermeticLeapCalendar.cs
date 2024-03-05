using System;
using System.Globalization;

namespace WeirdCalendars {
    public class HermeticLeapCalendar : LeapWeekCalendar {
        
        protected override DateTime SyncDate => new DateTime(2022, 12, 26);
        protected override int SyncOffset => 1;

        public override string Author => "Peter Meyer";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/hlpwk/hlpwk.htm");

        public HermeticLeapCalendar() => Title = "Hermetic Leap Week Calendar";
        
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return ((month - 1) % 3 == 0) || (IsLeapYear(year) && month == 12) ? 35 : 28;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 12 && day > 28;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (71 * year + 203) % 400 < 71;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Arcturus", "Bellatrix", "Canopus", "Deneb", "Elnath", "Fomalhaut", "Girtab", "Hadar", "Izar", "Jabbah", "Kochab", "Lesath", "" });
        }
    }
}