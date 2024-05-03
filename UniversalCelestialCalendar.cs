using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class UniversalCelestialCalendar : FixedCalendar {
        
        public new enum DayOfWeekWC {
            Blankday = -1,
            Onesday,
            Twosday,
            Threesday,
            Foursday,
            Fivesday,
            Sixday,
            Sevensday,
            Eightsday,
            Ninesday,
            Tensday
        }

        protected override DateTime SyncDate => new DateTime(2022, 3, 21);
        protected override int SyncOffset => 11501;

        protected override int FirstMonth => 0;
        protected override int DaysInWeek => 10;

        public override string Author => "Litmus A. Freeman";
        public override Uri Reference => new Uri("https://universalcelestialcalendar.com/Universal%20Community%20Calendar%20Wiki.backup.html");

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Festival"),
            ("b", "Symbolized")
        };

        protected override int GetFirstDayOfMonth(int year, int month) {
            ValidateDateParams(year, month, 0);
            return month > 0 ? (month - 1) % 3 == 0 ? 0 : 1 : IsLeapYear(year) ? 0 : 1;
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int wn = WeekdayNumber(time);
            if (wn > 6) throw BadWeekday;
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            return (DayOfWeek)wn;
        }

        public new DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ymd = ToLocalDate(time);
            int w = IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day) ? -1 : WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month > 0 ? day == 0 : true;
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            return month > 0 ? $@"{new string[] { "1st", "2nd", "3rd", "4th" }[month / 3]} Season\'s Day" : day == 0 ? @"Leap Year\'s Day" : @"New Year\'s Day";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return month > 0 ? new string[] { "1st", "2nd", "3rd", "4th" }[month / 3] : day == 0 ? "Leap" : "New";
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month > 0 ? (month - 1) % 3 == 0 ? 31 : 30 : IsLeapYear(year) ? 2 : 1;    
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public string GetFestival(DateTime time, bool symbolized = false) {
            string f = NoSpecialDay;
            var (_, month, day, _) = ToLocalDate(time);
            // cardinal
            string s = "⊕";
            if (month == 12 && day > 28 || month == 0 || month == 1 && day == 0) f = "New Year Festival of Aries♈";
            else if (month == 3 && day == 30 || month == 4 && day < 2) f = "Festival of Cancer♋";
            else if (month == 7 && day < 4) f = "Festival of Libra♎";
            else if (month == 10 && day < 3) f = "Festival of Capricorn♑";
            else if (day > 14 && day < 17) {
                //mid
                s = "⊗";
                if (month == 2) f = "Mid Taurus♉";
                else if (month == 5) f = "Mid Leo♌";
                else if (month == 8) f = "Mid Scorpio♏";
                else if (month == 11) f = "Mid Aquarius♒";
            }
            if (symbolized && f != NoSpecialDay) f = s + f.Substring(f.Length - 1);
            return f;
        }

        private static string Zodiac = "♈♉♊♋♌♍♎♏♐♑♒♓";
        private static string Quarter = "◷◴◵◶";

        public string GetSymbolized(DateTime time) {
            string s = "";
            var ld = ToLocalDate(time);
            if (IsIntercalaryDay(ld.Year, ld.Month, ld.Day)) {
                if (ld.Month == 0) s = ld.Day == 0 ? "✶" : "❂";
                else s = Quarter.Substring(ld.Month / 3, 1);
            }
            else {
                string f = GetFestival(time, true);
                s = f == NoSpecialDay ? $"{ld.Day}{Zodiac.Substring(ld.Month - 1, 1)}" : f;
            }
            return $"{s}{ld.Year}";
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 0 && day == 0;
        }

        public override bool IsLeapYear(int year) {
            ValidateDateParams(year, 0);
            int cy = (year - 12) % 33;
            return (cy - (cy < 21 ? 0 : 1)) % 4 == 0;
            //1, 5, 9, 13, 17, 22, 26, 30 of 1-based cycle
        }

        public override DateTime AddYears(DateTime time, int years) {
            var ld = ToLocalDate(time);
            ld.Year = (ld.Year + years - 1) % 24000 + 1;
            ld.Day = Math.Min(ld.Day, GetDaysInMonth(ld.Year, ld.Month));
            return ToDateTime(ld.Year, ld.Month, ld.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public override DateTime AddMonths(DateTime time, int months) {
            var ld = ToLocalDate(time);
            int lastMonth = GetMonthsInYear(ld.Year);
            for (int i = 0, j = Math.Sign(months); i < Math.Abs(months); i++) {
                ld.Month += j;
                if (ld.Month > lastMonth) {
                    ld.Year++;
                    ld.Month = FirstMonth;
                    lastMonth = GetMonthsInYear(ld.Year);
                }
                else if (ld.Month < FirstMonth) {
                    ld.Year--;
                    ld.Month = GetMonthsInYear(ld.Year);
                }
            }
            ld.Year = (ld.Year - 1) % 24000 + 1;
            ld.Day = Math.Min(ld.Day, GetDaysInMonth(ld.Year, ld.Month));
            return ToDateTime(ld.Year, ld.Month, ld.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (GetMonth(time) == 0) {
                fx.MonthFullName = "ZERO";
                fx.MonthShortName = "ZER";
            }
            if (fx.DayFullName == null) {
                string d = GetDayOfWeekWC(time).ToString();
                fx.DayFullName = d;
                fx.DayShortName = d.Substring(0, 3);
            }
            fx.Format = format.ReplaceUnescaped("n", $"'{GetFestival(time)}'").ReplaceUnescaped("b", $"'{GetSymbolized(time)}'");
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "ONE-Aries♈", "TWO-Taurus♉", "THREE-Gemini♊", "FOUR-Cancer♋", "FIVE-Leo♌", "SIX-Virgo♍", "SEVEN-Libra♎", "EIGHT-Scorpio♏", "NINE-Sagittarius♐", "TEN-Capricorn♑", "ELEVEN-Aquarius♒", "TWELVE-Pisces♓", "" }, null, new string[] { "Onesday", "Twosday", "Threesday", "Foursday", "Fivesday", "Sixday", "Sevensday" });
        }
    }
}