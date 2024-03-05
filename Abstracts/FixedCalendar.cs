using System;
using System.Globalization;

namespace WeirdCalendars {
    
    /// <summary>
    /// Abstract class representing a calendar which employs intercalary days.
    /// </summary>
    public abstract class FixedCalendar : WeirdCalendar {

        public enum DayOfWeekWC {
            Blankday = -1,
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            return (DayOfWeek)WeekdayNumber(time);
        }

        public virtual DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            var ymd = ToLocalDate(time);
            int w = IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day) ? -1 : WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }
        
        protected virtual int WeekdayNumber(DateTime time) {
            //Default to Sunday start with fixed months
            return (GetDayOfMonth(time) - 1) % DaysInWeek;
        }

        /// <summary>
        /// Indicates whether a date is an intercalary day, i.e. does not belong to a particular month and/or the ordinary cycle of weekdays.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="month">An integer that represents the month.</param>
        /// <param name="day">An integer that represents the day.</param>
        /// <remarks>Intercalary days are always assigned to a month and given a day-of-month number for compatibility with the Globalization API.</remarks>
        public virtual bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return (month == 12 && day == 31) || IsLeapDay(year, month, day);
        }

        internal protected virtual string IntercalaryDayName(int year, int month, int day) {
            return IsLeapDay(year, month, day) ?  "Leap Day" : "Festival Day";
        }

        internal protected virtual string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            string n = IntercalaryDayName(year, month, day);
            return n == "" ? n : n.Substring(0, 4).Trim();
        }

        public override DateTime AddWeeks(DateTime time, int weeks) {
            ValidateDateTime(time);
            int j = Math.Sign(weeks);
            TimeSpan weekAdd = TimeSpan.FromDays(j * DaysInWeek);
            TimeSpan dayAdd = TimeSpan.FromDays(j);
            DayOfWeekWC target = GetDayOfWeekWC(time);
            // cannot count from a blank day
            while (target == DayOfWeekWC.Blankday) {
                time += dayAdd;
                target = GetDayOfWeekWC(time);
            }
            for (int i = 0; i < Math.Abs(weeks); i++) {
                time += weekAdd;
                DayOfWeekWC dow = GetDayOfWeekWC(time);
                // adjust if didn't land on target day
                while (dow != target) {
                    time += dayAdd;
                    dow = GetDayOfWeekWC(time);
                }
            }
            return time;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            //If this is an intercalary day, fix up day name
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) {
                fx.DayFullName = IntercalaryDayName(ymd.Year, ymd.Month, ymd.Day);
                fx.DayShortName = IntercalaryAbbreviatedDayName(ymd.Year, ymd.Month, ymd.Day);
            }
            return fx;
        }
    }
}