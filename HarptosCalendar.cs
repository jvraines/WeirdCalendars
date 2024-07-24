using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class HarptosCalendar : FixedCalendar {
        
        public override string Author => "Ed Greenwood et al.";
        public override Uri Reference => new Uri("https://forgottenrealms.fandom.com/wiki/Calendar_of_Harptos");

        protected override DateTime SyncDate => new DateTime(2024, 1, 2);
        protected override int SyncOffset => -524;

        public override string Notes => "Dale Reckoning at latitude of Waterdeep";

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Festival")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 4:
                case 9:
                case 11:
                    return 31;
                case 7:
                    return IsLeapYear(year) ? 32 : 31;
                default:
                    return 30;
            }
        }

        private static Dictionary<int, DateTime[]> AstroTimes = new Dictionary<int, DateTime[]>();
        private static string[] Hours = { "Midnight", "Moondark", "Nightsend", "Dawn", "Morning", "Highsun", "Afternoon", "Dusk", "Sunset", "Evening", "Midnight" };
        
        public string GetTime(DateTime time) {
            ValidateDateTime(time);
            int jd = (int)time.JulianDay();
            if (!AstroTimes.TryGetValue(jd, out DateTime[] times)) times = UpdateAstroTimes(time, jd);
            int h;
            for (h = 0; h < 10; h++) {
                if (time < times[h]) break;
            }
            return Hours[h];
        }
        
        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 32;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day > 30;
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            switch (month) {
                case 1:
                    return "Midwinter";
                case 4:
                    return "Greengrass";
                case 7:
                    return day == 31 ? "Midsummer" : "Shieldmeet";
                case 9:
                    return "Highharvestide";
                case 11:
                    return "Feast of the Moon";
                default:
                    throw new ArgumentException("Not an intercalary day");
            };
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            switch (month) {
                case 1:
                    return "Mwn";
                case 4:
                    return "Grn";
                case 7:
                    return day == 31 ? "Msm" : "Shd";
                case 9:
                    return "Hig";
                case 11:
                    return "Mon";
                default:
                    throw new ArgumentException("Not an intercalary day");
            };
        }

        public string GetFestival(DateTime time) {
            string f = NoSpecialDay;
            var ld = ToLocalDate(time);
            if (IsIntercalaryDay(ld.Year, ld.Month, ld.Day)) f = IntercalaryDayName(ld.Year, ld.Month, ld.Day);
            else if (ld.Month == 3 && ld.Day == 19) f = "Spring Equinox";
            else if (ld.Month == 6 && ld.Day == 20) f = "Summer Solstice";
            else if (ld.Month == 9 && ld.Day == 21) f = "Autumn Equinox";
            else if (ld.Month == 12 && ld.Day == 20) f = "Winter Solstice";
            return f;
        }
        
        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Hammer", "Alturiak", "Ches", "Tarsakh", "Mirtul", "Kythorn", "Flamerule", "Eleasis", "Eleint", "Marpenoth", "Uktar", "Nightal", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.DayShortName = IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day);
            }
            else {
                string ordday = ((ymd.Day - 1) % 10 + 1).ToOrdinal();
                string tenday = ((ymd.Day - 1) / 10 + 1).ToOrdinal();
                fx.DayFullName = $"{ordday} day of the {tenday} tenday";
                fx.DayShortName = $"{ordday} of {tenday}";
            }
            fx.Format = format.ReplaceUnescaped("n", $"'{GetFestival(time)}'");
            string hour = $"'{GetTime(time)}'";
            fx.LongTimePattern = hour;
            fx.ShortTimePattern = hour;
            fx.Format = FixTimeFormat(fx.Format);
            return fx;

            string FixTimeFormat(string f) {
                string ff = f.ReplaceUnescaped("HH", hour).ReplaceUnescaped("H", hour).ReplaceUnescaped("hh", hour).ReplaceUnescaped("h", hour).ReplaceUnescaped("mm", "").ReplaceUnescaped("m", "").ReplaceUnescaped("ss", "").ReplaceUnescaped("s", "");
                return ff;
            }
        }

        DateTime[] UpdateAstroTimes(DateTime time, int jd) {
            DateTime[] times = new DateTime[10];
            var sun = Sky.RiseTransitSet(Bodies.Sun, time, 0, 45, false); //latitude of Waterdeep
            DateTime rise = (DateTime)sun.rise;
            DateTime set = (DateTime)sun.set;
            times[0] = sun.transit.AddHours(-11.5); //Moondark
            times[1] = rise.AddHours(-2); //Night's End
            times[2] = rise.AddHours(-1); //Dawn
            times[3] = rise; //Morning
            times[4] = sun.transit.AddHours(-0.5); //Highsun
            times[5] = sun.transit.AddHours(0.5); //Afternoon
            times[6] = set.AddHours(-1); //Dusk
            times[7] = set; //Sunset
            times[8] = set.AddHours(1); //Evening
            times[9] = sun.transit.AddHours(11.5); //Midnight
            AstroTimes.Add(jd, times);
            return times;
        }
    }
}
