using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeirdCalendars {
    public class Observational6DayWeekCalendar : WeirdCalendar {
        
        public override string Author => "Achilleas Vourvopoulos";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Observational_6-day_week_solar_calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 21);
        protected override int SyncOffset => 1;
        public override DateTime MaxSupportedDateTime => VSOPLimit;
        public override int DaysInWeek => 6;

        public override string Notes => "Version 1";

        public Observational6DayWeekCalendar() => Title = "Observational 6-Day Week Solar Calendar";

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeek)((ToLocalDate(time).Day - 1) % 6 + 1);
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 1:
                    return IsLeapYear(year) ? 30 : 29;
                case 7:
                    return 36;
                default:
                    return 30;
            }
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool isLeap)) {
                double thisStart = Earth.SeasonStart(year - 1, Earth.Season.December).ToLastUTMidnight();
                double nextStart = Earth.SeasonStart(year, Earth.Season.December).ToLastUTMidnight();
                isLeap = nextStart - thisStart > 365;
                LeapYears.Add(year, isLeap);
            }
            return isLeap;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 1 && day == 30;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
        }
    }
}
