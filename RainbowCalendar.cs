using System;
using System.Globalization;

namespace WeirdCalendars {
    public class RainbowCalendar : WeirdCalendar {
  
        public override string Author => "Alexander Laik and Karl Palmen";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Calendar_of_the_Rainbow");

        protected override DateTime SyncDate => new DateTime(2020, 3, 22); // Chosen to make New Year's Day a Sunday
        protected override int SyncOffset => 0; // There is no evidence to link fictional Year 727 to reality

        public RainbowCalendar() => Title = "Calendar of the Rainbow";

        public override int GetMonthsInYear(int year, int era) => 8;

          public override int GetDaysInMonth(int year, int month) {
            ValidateDateParams(year, month, 1);
            int d = 45;
            switch (month) {
                case 1:
                    d = 47;
                    break;
                case 3:
                case 5:
                case 7:
                    d = 46;
                    break;
            }
            return IsLongMonth(year, month) ? d + 1 : d;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            //Intercalary days except leap day are at the beginning of the month
            switch (month) {
                case 1:
                    return -1;
                case 3:
                case 5:
                case 7:
                    return 0;
                default:
                    return 1;
            }
        }

        private int MonthCycle(int year, int month) {
            //Following Palmen's rule. Leap day will be at the end of the month
            return ((year - 727) * 8 + month) % 33;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return MonthCycle(year, 8) < MonthCycle(year - 1, 8);
        }

        private bool IsLongMonth(int year, int month) => MonthCycle(year, month) == 0;

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLongMonth(year, month) && day == GetDaysInMonth(year, month) + GetFirstDayOfMonth(year, month) - 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[]{"Dripping","Happy Flowers", "Light and Heat", "Noble", "Golden Leaves", "Freezing", "Binding", "Wind-Blowing", "", "", "", "", ""}, new string[] { "Drp", "Hpf", "Lth", "Nob", "Gnl", "Frz", "Bnd", "Wdb", "", "", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            if (ymd.Day == 46) {
                //leap day
                fx.DayFullName = "Equilibrium Day";
                fx.DayShortName = "Equ";
            }
            else if (ymd.Day < 1) {
                //intercalary day
                string dfn = "";
                switch (ymd.Month) {
                    case 1:
                        dfn = ymd.Day == -1 ? "Year" : "Spring";
                        break;
                    case 3:
                        dfn = "Summer";
                        break;
                    case 5:
                        dfn = "Autumn";
                        break;
                    case 7:
                        dfn = "Winter";
                        break;
                }
                fx.DayFullName = $"{dfn} Day";
                fx.DayShortName = dfn.Substring(0, 3);
                string d2 = dfn.Substring(0, 2);
                fx.ShortDatePattern = FixDigits(fx.ShortDatePattern, null, null, null, null, d2, d2);
                //if (ymd.Day < 1) d2 = "\b"; Kludgey and bugs XML output
            }
            return fx;
        }
    }
}
