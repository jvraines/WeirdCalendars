using System;
using System.Globalization;
using System.Linq;

namespace WeirdCalendars {
    public class MessiahsCalendar : WeirdCalendar {
        
        public enum DayOfWeekWC {
            festDay,
            fterDay,
            firdDay,
            fothDay,
            freeDay
        }
        
        public override string Author => "Grant Hardy";
        public override Uri Reference => new Uri("http://www.messiahpsychoanalyst.org/Documents/Calendar.html");

        protected override DateTime SyncDate => new DateTime(2022, 3, 21);
        protected override int SyncOffset => -2000;
        public override DateTime MinSupportedDateTime => new DateTime(2000, 3, 20);

        protected override int DaysInWeek => 5;

        public MessiahsCalendar() => Title = "Messiah's Calendar";

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeek)WeekdayNumber(time);
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            ValidateDateTime(time);
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        private int WeekdayNumber(DateTime time) {
            ValidateDateTime(time);
            return (int)((time.Date - MinSupportedDateTime).TotalDays - 1) % DaysInWeek;
        }
        
        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 20;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 20 ? IsLeapYear(year) ? 21 : 20 : month % 5 == 0 ? 19 : 18;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 21;
        }

        protected override long HourTicks => TimeSpan.TicksPerDay / 20;
        protected override long MinuteTicks => TimeSpan.TicksPerDay / 2000;
        protected override long SecondTicks => TimeSpan.TicksPerDay / 200000;
        protected override long MilliTicks => TimeSpan.TicksPerDay / 200000000;

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            if (hour < 0 || hour > 19) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 99) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 99) throw new ArgumentOutOfRangeException("second");
            TimeSpan clock = TimeSpan.FromTicks(hour * HourTicks + minute * MinuteTicks + second * SecondTicks + millisecond * MilliTicks);
            return base.ToDateTime(year, month, day, clock.Hours, clock.Minutes, clock.Seconds, clock.Milliseconds, era);
        }
        
        private static string[] MonthFullNames = { "Unum", "Doyum", "Sayum", "Karum", "Pentum", "Sisum", "Heptum", "Oktum", "Nonum", "Tenum", "Ununum", "Undoyum", "Unsayum", "Unkarum", "Unpentum", "Unsisum", "Unheptum", "Unoktum", "Unnonum", "Lentum" };

        private static string[] MonthShortNames = { "Unu", "Doy", "Say", "Kar", "Pen", "Sis", "Hep", "Okt", "Non", "Ten", "Unm", "Und", "Uns", "Unk", "Unp", "Uni", "Unh", "Uno", "Unn", "Len" };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, MonthFullNames.Take(13).ToArray(), MonthShortNames.Take(13).ToArray(), new string[] { "festDay", "fterDay", "firdDay", "fothDay", "freeDay", "", ""});
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            CustomizeTimes(fx, time);
            int m = GetMonth(time) - 1;
            fx.MonthFullName = MonthFullNames[m];
            fx.MonthShortName = MonthShortNames[m];
            return fx;
        }
    }
}
