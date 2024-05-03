using System;
using System.Globalization;
using System.Collections.Generic;

namespace WeirdCalendars {
    public abstract class WeirdCalendar : Calendar {

        /// <summary>
        /// Author(s) of this calendar.
        /// </summary>
        public abstract string Author { get; }
        /// <summary>
        /// A URI pointing to original documentation of this calendar.
        /// </summary>
        public abstract Uri Reference { get; }

        /*
        // <summary>
        /// Name of the Fandom Calendars wiki page associated with this calendar.
        /// </summary>
        private string fandomPage;
        public string FandomPage {
            get => PageNameE(fandomPage ?? Title);
            protected set => fandomPage = value;
        }

        private string PageNameE(string s) {
            // MediaWiki standard URL encoder for page names
            StringBuilder sb = new StringBuilder();
            UTF8Encoding enc = new UTF8Encoding();
            foreach(char c in s) {
                if (c < 0x0080) {
                    if ("\"%&'+=?\\^`~".Contains(c)) sb.Append($"%{(int)c:X}");
                    else if (c == 32) sb.Append("_");
                    else sb.Append(c);
                }
                else {
                    foreach (byte b in enc.GetBytes(new char[] { c })) sb.Append($"%{b:X}");
                }
            }
            return sb.ToString();
        }
        */

        public virtual string Notes { get; protected set; }
        
        private string title;
        public string Title { 
            get => title ?? GetType().Name.ExpandPascalCase();
            protected set => title = value;
        }

        protected readonly string NoSpecialDay = "(none)";

        /// <summary>
        /// Exception indicating that the requested day of week cannot be represented as a value of the DateTime.DayOfWeek enumeration.
        /// </summary>
        public InvalidOperationException BadWeekday = new InvalidOperationException ("Date cannot be represented as DayOfWeek. Use GetDayOfWeekWC() or ToStringWC() instead.");

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.SolarCalendar;

        // Gregorian date of first day of a year in this calendar.
        // Choose a year around 2020 to speed calculations.
        protected abstract DateTime SyncDate { get; }
        
        // Offset of year count from year of SyncDate
        protected abstract int SyncOffset { get; }

        // Customizable max year offset
        protected virtual int MaxYearOffset => SyncOffset;
        
        /// <summary>
        /// A List of tuples describing custom format strings available for use with DateTime.ToStringWC.
        /// </summary>
        public virtual List<(string FormatString, string Description)> CustomFormats => new List<(string, string)>();

        public override int[] Eras => new int[] { 1 };

        public override int GetEra(DateTime time) => Eras[0];
        internal virtual int GetEra(int year) => Eras[0];

        /// <summary>
        /// Number of first month of the year.
        /// </summary>
        protected virtual int FirstMonth => 1;

        /// <summary>
        /// Number of last month of the year.
        /// </summary>
        protected int GetLastMonthOfYear(int year) {
            return GetMonthsInYear(year) - 1 + FirstMonth;
        }

        /// <summary>
        /// Number of the first day of a particular month.
        /// </summary>
        protected virtual int GetFirstDayOfMonth(int year, int month) => 1;

        /// <summary>
        /// Number of the last day of a particular month.
        /// </summary>
        protected int GetLastDayOfMonth(int year, int month) {
            return GetDaysInMonth(year, month) - 1 + GetFirstDayOfMonth(year, month);
        }

        /// <summary>
        /// Number of days in one week.
        /// </summary>
        protected virtual int DaysInWeek => 7;
        
        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            return time.DayOfWeek;
        }

        public override int GetDayOfMonth(DateTime time) {
            ValidateDateTime(time);
            return ToLocalDate(time).Day;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2:
                    return IsLeapYear(year) ? 29 : 28;
                default:
                    return 31;
            }
        }

        public override int GetMonth(DateTime time) {
            ValidateDateTime(time); 
            return ToLocalDate(time).Month;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 12;
        }
        
        public override int GetYear(DateTime time) {
            ValidateDateTime(time); 
            return ToLocalDate(time).Year;
        }

        public override int GetDayOfYear(DateTime time) {
            ValidateDateTime(time); 
            var ymd = ToLocalDate(time);
            int d = 0;
            for (int m = FirstMonth; m < ymd.Month; m++) d += GetDaysInMonth(ymd.Year, m);
            return d + ymd.Day + 1 - GetFirstDayOfMonth(ymd.Year, ymd.Month);
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 366 : 365;
        }

        /// <summary>
        /// Scaling from normal Earth time.
        /// </summary>
        protected virtual double TimescaleFactor => 1;
        
        protected virtual long HourTicks => TimeSpan.TicksPerDay / 24;
        protected virtual long MinuteTicks => TimeSpan.TicksPerDay / 1440;
        protected virtual long SecondTicks => TimeSpan.TicksPerDay / 86400;
        protected virtual long MilliTicks => TimeSpan.TicksPerDay / 8640000;

        public override int GetHour(DateTime time) {
            ValidateDateTime(time);
            TimeSpan clock = ToLocalDate(time).TimeOfDay;
            return (int)(clock.Ticks / HourTicks);
        }

        public override int GetMinute(DateTime time) {
            ValidateDateTime(time);
            TimeSpan clock = ToLocalDate(time).TimeOfDay;
            return (int)(clock.Ticks % HourTicks / MinuteTicks);
        }

        public override int GetSecond(DateTime time) {
            ValidateDateTime(time);
            TimeSpan clock = ToLocalDate(time).TimeOfDay;
            return (int)(clock.Ticks % MinuteTicks / SecondTicks);
        }

        public override double GetMilliseconds(DateTime time) {
            ValidateDateTime(time);
            TimeSpan clock = ToLocalDate(time).TimeOfDay;
            return clock.Ticks % SecondTicks / MilliTicks;
        }

        public override DateTime AddHours(DateTime time, int hours) {
            ValidateDateTime(time);
            return time.AddTicks((long)(hours * TimescaleFactor * HourTicks));
        }

        public override DateTime AddMinutes(DateTime time, int minutes) {
            ValidateDateTime(time);
            return time.AddTicks((long)(minutes * TimescaleFactor * MinuteTicks));
        }

        public override DateTime AddSeconds(DateTime time, int seconds) {
            ValidateDateTime(time);
            return time.AddTicks((long)(seconds * TimescaleFactor * SecondTicks));
        }

        public override DateTime AddMilliseconds(DateTime time, double milliseconds) {
            ValidateDateTime(time);
            return time.AddTicks((long)(milliseconds * TimescaleFactor * MilliTicks));
        }
        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
        }

        protected bool IsISOLeapYear (int year) {
            return new DateTime(year, 1, 1).DayOfWeek == DayOfWeek.Thursday || new DateTime(year, 12, 31).DayOfWeek == DayOfWeek.Thursday;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 2 && day == 29;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return false;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return 0;
        }

        public override DateTime AddDays(DateTime time, int days) {
            ValidateDateTime(time);
            long t = (long)(days * TimeSpan.TicksPerDay * TimescaleFactor);
            return time + TimeSpan.FromTicks(t);
        }

        public override DateTime AddWeeks(DateTime time, int weeks) {
            ValidateDateTime(time);
            return time.AddDays(weeks * DaysInWeek);
        }

        public override DateTime AddMonths(DateTime time, int months) {
            ValidateDateTime(time); 
            var ld = ToLocalDate(time);
            int lastMonth = GetLastMonthOfYear(ld.Year);
            for (int i = 0, j = Math.Sign(months); i < Math.Abs(months); i++) {
                ld.Month += j;
                if (ld.Month > lastMonth) {
                    ld.Year++;
                    ld.Month = FirstMonth;
                    lastMonth = GetLastMonthOfYear(ld.Year);
                }
                else if (ld.Month < FirstMonth) {
                    ld.Year--;
                    ld.Month = GetLastMonthOfYear(ld.Year);
                }
            }
            ld.Day = Math.Max(Math.Min(ld.Day, GetLastDayOfMonth(ld.Year, ld.Month)), GetFirstDayOfMonth(ld.Year, ld.Month));
            return ToDateTime(ld.Year, ld.Month, ld.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public override DateTime AddYears(DateTime time, int years) {
            ValidateDateTime(time); 
            var ld = ToLocalDate(time);
            ld.Year += years;
            int lastMonth = GetLastMonthOfYear(ld.Year);
            if (ld.Month > lastMonth) {
                ld.Month = lastMonth;
                ld.Day = GetLastDayOfMonth(ld.Year, ld.Month);
            }
            else ld.Day = Math.Max(Math.Min(ld.Day, GetLastDayOfMonth(ld.Year, ld.Month)), GetFirstDayOfMonth(ld.Year, ld.Month));
            return ToDateTime(ld.Year, ld.Month, ld.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        internal bool ValidationEnabled = true;

        /// <summary>
        /// Ensures that date arguments are valid.
        /// </summary>
        /// <param name="param">Integer parameter array containing year and era; or year, month, and era; or year, month, day, and era.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void ValidateDateParams(params int[] param) {
            if (!ValidationEnabled) return;
            int era = param[param.Length - 1];
            // Era 0 means use latest era
            if (era == 0) era = Eras[0];
            int firstEra = Eras[Eras.Length - 1];
            int lastEra = Eras[0];
            if (era < firstEra || era > lastEra) throw new ArgumentOutOfRangeException("era", $"Era must be in the range {firstEra} to {lastEra}. Era {era} was passed.");
            ValidationEnabled = false;
            ArgumentOutOfRangeException oops = default;
            for (int p = 0; p < param.Length - 1; p++) {
                switch (p) {
                    case 0:
                        int minYear = MinSupportedDateTime.Year + SyncOffset;
                        int maxYear = MaxSupportedDateTime.Year + MaxYearOffset;
                        if (param[0] < minYear || param[0] > maxYear) oops = new ArgumentOutOfRangeException("year", $"Calendar year must be in the range {minYear} to {maxYear}. Year {param[0]} was passed.");
                        break;
                    case 1:
                        int lastMonth = GetMonthsInYear(param[0], era);
                        if (param[1] < FirstMonth || param[1] > lastMonth) oops = new ArgumentOutOfRangeException("month", $"Calendar month must be in the range {FirstMonth} to {lastMonth}. Month {param[1]} was passed.");
                        break;
                    case 2:
                        int firstDay = GetFirstDayOfMonth(param[0], param[1]);
                        int lastDay = GetDaysInMonth(param[0], param[1], era) + firstDay - 1;
                        if (param[2] < firstDay || param[2] > lastDay) oops = new ArgumentOutOfRangeException("day", $"Calendar day must be in the range {firstDay} to {lastDay}. Day {param[2]} was passed.");
                        break;
                }
                if (oops != null) {
                    ValidationEnabled = true;
                    throw oops;
                }
            }
            ValidationEnabled = true;
        }

        /// <summary>
        /// Ensures that DateTime is valid.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected void ValidateDateTime(DateTime time) {
            if (time < MinSupportedDateTime) throw new ArgumentOutOfRangeException("time", $"Time must be greater than or equal to {MinSupportedDateTime}.");
            else if (time > MaxSupportedDateTime) throw new ArgumentOutOfRangeException("time", $"Time must be less than or equal to {MaxSupportedDateTime}.");
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            ValidateDateParams(year, month, day, era);
            ValidationEnabled = false;
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException("second");
            if (millisecond < 0 || millisecond > 999) throw new ArgumentOutOfRangeException("millisecond");

            int eYear = SyncDate.Year;
            int cYear = year - SyncOffset;
            double diffDays = new TimeSpan(day - GetFirstDayOfMonth(year, month), hour, minute, second, millisecond).TotalDays / TimescaleFactor;
            while (month != FirstMonth) {
                month--;
                diffDays += GetDaysInMonth(year, month, era);
            }
            while (cYear < eYear) {
                diffDays -= GetDaysInYear(year, era);
                cYear++;
                year++;
            }
            while (cYear > eYear) {
                cYear--;
                year--;
                diffDays += GetDaysInYear(year, era);
            }
            ValidationEnabled = true;
            try {
                return SyncDate.AddDays(diffDays);
            }
            catch (Exception x) {
                throw x;
            }
        }

        private DateTime LastTime = DateTime.MaxValue;
        private (int Year, int Month, int Day, TimeSpan TimeOfDay) LastLocalDate;

        /// <summary>
        /// Converts DateTime to date and time values.
        /// </summary>
        protected virtual (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            // This method disregards eras and works strictly with SyncDate-based years
            if (time == LastTime) return LastLocalDate;
            ValidationEnabled = false;
            double diffDays = (time - SyncDate).TotalDays / TimescaleFactor;
            int year = SyncDate.Year + SyncOffset;
            while (diffDays < 0) {
                year--;
                diffDays += GetDaysInYear(year);
            }
            int yearDays = GetDaysInYear(year);
            while (diffDays >= yearDays) {
                year++;
                diffDays -= yearDays;
                yearDays = GetDaysInYear(year);
            }
            int month = FirstMonth;
            while (diffDays > 0) {
                double monthDays = GetDaysInMonth(year, month);
                if (diffDays >= monthDays) {
                    diffDays -= monthDays;
                    month++;
                }
                else break;
            }
            TimeSpan clock = TimeSpan.FromSeconds(Math.Round((diffDays - Math.Truncate(diffDays)) * 86400));
            LastLocalDate = (year, month, (int)diffDays + GetFirstDayOfMonth(year, month), clock);
            LastTime = time;
            return LastLocalDate;
        }
        
        internal virtual FormatWC GetFormatWC (DateTimeFormatInfo dtfi, DateTime time, string format) {
            return new FormatWC(format, dtfi);
        }

        internal string Language;
        internal virtual void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            //Any tweaks needed by specific calendars
        }

        /// <summary>
        /// Custom names for months and optionally days of the week. If abbreviations are not supplied, they default to the first three characters of the full name.
        /// </summary>
        internal void SetNames(DateTimeFormatInfo dtfi, string[] months, string[] monthsAbbr = null, string[] days = null, string[] daysAbbr = null) {
            if (months != null) {
                dtfi.MonthNames = months;
                dtfi.MonthGenitiveNames = months;
                if (monthsAbbr == null) {
                    monthsAbbr = new string[13];
                    for (int i = 0; i < 13; i++) monthsAbbr[i] = months[i].Substring(0, Math.Min(3, months[i].Length));
                }
                dtfi.AbbreviatedMonthNames = monthsAbbr;
                dtfi.AbbreviatedMonthGenitiveNames = monthsAbbr;
            }
            if (days != null) {
                dtfi.DayNames = days;
                if (daysAbbr == null) {
                    daysAbbr = new string[7];
                    for (int i = 0; i < 7; i++) daysAbbr[i] = days[i].Substring(0, Math.Min(3, days[i].Length));
                }
                dtfi.AbbreviatedDayNames = daysAbbr;
            }
        }

        internal virtual void CustomizeTimes(FormatWC fx, DateTime time, string designator = null, int radix = 10, int halfCycle = 12) {
            int v = GetHour(time);
            string x = v.ToBase(radix);
            string H = x;
            string HH = x.PadLeft(2, '0');
            v = v % halfCycle == 0 ? halfCycle : v % halfCycle;
            x = v.ToBase(radix);
            string h = x;
            string hh = x.PadLeft(2, '0');
            x = GetMinute(time).ToBase(radix);
            string m = x;
            string mm = x.PadLeft(2, '0');
            x = GetSecond(time).ToBase(radix);
            string s = x;
            string ss = x.PadLeft(2, '0');
            if ("fFUgGtT".Contains(fx.Format)) {
                // fix up long and short time formats
                fx.LongTimePattern = FixTimeFormat(fx.LongTimePattern);
                fx.ShortTimePattern = FixTimeFormat(fx.ShortTimePattern);
            }
            else fx.Format = FixTimeFormat(fx.Format);

            string FixTimeFormat(string f) {
                string ff = f.ReplaceUnescaped("HH", HH).ReplaceUnescaped("H", H).ReplaceUnescaped("hh", hh).ReplaceUnescaped("h", h).ReplaceUnescaped("mm", mm).ReplaceUnescaped("m", m).ReplaceUnescaped("ss", ss).ReplaceUnescaped("s", s);
                if (designator != null) ff = ff.ReplaceUnescaped("tt", $"'{designator}'").ReplaceUnescaped("t", $"'{designator.Substring(0, 1)}'");
                return ff;
            }
        }

        internal void FixNegativeYears(FormatWC fx, DateTime time) {
            int y = ToLocalDate(time).Year;
            if (y < 0 ) {
                fx.LongDatePattern = FixYearFormat(fx.LongDatePattern);
                fx.ShortDatePattern = FixYearFormat(fx.ShortTimePattern);
                fx.Format = FixYearFormat(fx.Format);
            }

            string FixYearFormat(string f) {
                return f.ReplaceUnescaped("yyyyy", y.ToString("D5")).ReplaceUnescaped(
                "yyyy", y.ToString("D4")).ReplaceUnescaped("yyy", y.ToString("D3")).ReplaceUnescaped("yy", (y % 100).ToString("D2"));
            }
        }

        internal string FixDigits(string s, object y4 = null, object y2 = null, object m2 = null, object m1 = null, object d2 = null, object d1 = null) {
            if (y4 != null) s = s.ReplaceUnescaped("yyyy", $"'{y4}'").ReplaceUnescaped("yy", $"'{y2}'");
            if (m2 != null) s = s.ReplaceUnescaped("MMMM", "~~~~").ReplaceUnescaped("MMM", "~~~").ReplaceUnescaped("MM", $"'{m2}'").ReplaceUnescaped("M", $"'{m1}'").ReplaceUnescaped("~~~~", "MMMM").ReplaceUnescaped("~~~", "MMM");
            if (d2 != null) s = s.ReplaceUnescaped("dddd", "~~~~").ReplaceUnescaped("ddd", "~~~").ReplaceUnescaped("dd", $"'{d2}'").ReplaceUnescaped("d", $"'{d1}'").ReplaceUnescaped("~~~~", "dddd").ReplaceUnescaped("~~~", "ddd");
            return s;
        }
    }
}