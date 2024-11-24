using System;
using System.Globalization;

namespace WeirdCalendars {
    public class ModernCalendar : WeirdCalendar {
        
        public enum DayOfWeekWC {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Remday,
            Saturday
        }
        
        public override string Author => "Anonymous";
        public override Uri Reference => new Uri("http://moderncalendar.blogspot.com/");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => 0;

        public override int DaysInWeek => 8;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int wn = WeekdayNumber(time);
            if (wn > 6) throw BadWeekday;
            return (DayOfWeek)wn;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        protected int WeekdayNumber(DateTime time) {
            return (GetDayOfMonth(time) - 1) % DaysInWeek;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 10 ? 40 : IsLeapYear(year) ? 6 : 5;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era); 
            return 10;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 10 && day == 6;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            DayOfWeekWC d = GetDayOfWeekWC(time);
            if (d == DayOfWeekWC.Saturday) {
                //restore "lost" name of Saturday
                fx.DayFullName = dtfi.MonthNames[11];
                fx.DayShortName = dtfi.MonthNames[12];
            }
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] m = (string[])dtfi.MonthNames.Clone();
            for (int i = 1, o = 1; i < 11; i++) {
                if (i == 4) o++;
                m[i] = m[i + o];
            }
            //stash name of Saturday for use in GetFormatWC
            m[11] = dtfi.DayNames[6];
            m[12] = m[11].Substring(0, 3);
            string[] w = (string[])dtfi.DayNames.Clone();
            w[6] = "Remday";
            SetNames(dtfi, m, null, w);
        }
    }
}
