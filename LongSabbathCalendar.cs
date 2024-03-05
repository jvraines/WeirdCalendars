﻿using System;
using System.Globalization;

namespace WeirdCalendars {
    public class LongSabbathCalendar : WeirdCalendar {
        
        public override string Author => "Rick McCarty";
        public override Uri Reference => new Uri("https://myweb.ecu.edu/mccartyr/lspc.html");

        // Author mentions a year 141 but never correlates it
        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;

        public override int GetDaysInYear(int year, int era) {
            return 365;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            switch(month) {
                case 2:
                    return 28;
                case 12:
                    return 30;
                default:
                    return base.GetDaysInMonth(year, month, era);
            } 
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            int i = 0;
            if (IsLeapYear(time.Year) && time.Month > 2) i = -1;
            if (time.Month == 12 && time.Day == 31 && time.Hour >= 12 && i == -1) i = 0;
            return (DayOfWeek)((GetDayOfYear(time) + i - 1) % 7);
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            ValidateDateParams(year, month, day, era);
            int maxHour = month == 12 && day == 30 || month == 1 && day == 1 || month == 2 && IsLeapYear(year) && (day == 25 || day == 26) ? 35 : 23;
            if (hour < 0 || hour > maxHour) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException("second");
            if (millisecond < 0 || millisecond > 999) throw new ArgumentOutOfRangeException("millisecond");

            if (month == 12 && day == 30 && hour > 23) {
                day = 31;
                hour -= 24;
            }
            else if (month == 1 && day == 1) {
                if (hour < 12) {
                    year--;
                    month = 12;
                    day = 31;
                    hour += 12;
                }
                else hour -= 12;
            }
            else if (month == 2 && day > 24 && IsLeapYear(year)) {
                if (day == 25) {
                    if (hour > 23) {
                        day = 26;
                        hour -= 24;
                    }
                }
                else if (day == 26) {
                    if (hour > 11) {
                        day = 27;
                        hour -= 12;
                    }
                    else hour += 12;
                }
                else day++;
            }
            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        private DateTime LastTime;
        private (int Year, int Month, int Day, TimeSpan TimeOfDay) LastLocalDate;

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            if (time == LastTime) return LastLocalDate;
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            int hour = time.Hour;
            if (month == 12 && day == 31) {
                if (hour < 12) {
                    day = 30;
                    hour += 24;
                }
                else {
                    year++;
                    month = 1;
                    day = 1;
                    hour -= 12;
                }
            }
            else if (month == 1 && day == 1) hour += 12;
            else if (month == 2 && day > 25 && IsLeapYear(year)) {
                if (day == 26) {
                    if (hour < 12) {
                        hour += 24;
                    }
                    else {
                        day = 27;
                        hour -= 12;
                    }
                }
                else if (day == 27) hour += 12;
                day--;
            }
            LastLocalDate = (year, month, day, new TimeSpan(0, hour, time.Minute, time.Second, time.Millisecond));
            LastTime = time;
            return LastLocalDate;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ld = ToLocalDate(time);
            string ds = null;
            int h = (int)ld.TimeOfDay.TotalHours;
            if (h != time.Hour) {
                if (ld.Month == 12 && h > 23) ds = "AA";
                else if (ld.Month == 1 && h < 12) ds = "PA";
                else if (ld.Month == 2) {
                    if (ld.Day == 25 && h > 23) ds = "AQ";
                    else if (h < 12) ds = "PQ";
                }
                CustomizeTimes(fx, time, ds);
            }
            return fx;
        }
    }
}
