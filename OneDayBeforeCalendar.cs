using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WeirdCalendars {
    public class OneDayBeforeCalendar : FixedCalendar {

        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/palmen/1db4.htm");

        protected override DateTime SyncDate => new DateTime(2023, 1, 1);
        protected override int SyncOffset => 0;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            UpdateFullMoons(time.Year);
            if (Fullmoons.Contains(time)) throw BadWeekday;
            return (DayOfWeek)WeekdayNumber(time);
        }

        public override DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            UpdateFullMoons(time.Year);
            int w = Fullmoons.Contains(time) ? -1 : WeekdayNumber(time);
            return (DayOfWeekWC)w;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            UpdateFullMoons(year);
            return Fullmoons.Contains(new DateTime(year, month, day));
        }
        
        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return "Fullmoonday";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return "Fmn";
        }

        protected override int WeekdayNumber(DateTime time) {
            int firstFmd;
            try {
                firstFmd = Fullmoons.Where(m => m.Month == time.Month && m.Year == time.Year).Select(m => m.Day).Min();
            }
            catch {
                // Must be February with no fullmoons; first day will be Sunday
                firstFmd = 8;
            }
            int firstWeekday = 8 - firstFmd % 7;
            int dayNum = time.Day - (time.Day < firstFmd ? 0 : 1);
            return (dayNum + firstWeekday - 1) % 7;
        }

        private HashSet<DateTime> Fullmoons = new HashSet<DateTime> {
            new DateTime(2023, 1, 7),
            new DateTime(2023, 2, 5),
            new DateTime(2023, 3, 6),
            new DateTime(2023, 4, 5),
            new DateTime(2023, 5, 4),
            new DateTime(2023, 6, 3),
            new DateTime(2023, 7, 2),
            new DateTime(2023, 7, 31),
            new DateTime(2023, 8, 1),
            new DateTime(2023, 8, 30),
            new DateTime(2023, 9, 29),
            new DateTime(2023, 10, 28),
            new DateTime(2023, 11, 27),
            new DateTime(2023, 12, 26)
        };

        private int FirstPlottedYear = 2023;
        private int LastPlottedYear = 2023;

        private bool MarchException(int year) {
            return (year & 1) == 0 && !IsLeapYear(year) || year % 16 == 15;
        }

        private int[] PlotFullmoons(int year, int[] seed, bool forward = true) {
            int[,] fmDays = new int[14, 2];
            if (forward) {
                fmDays[0, 0] = seed[0];
                fmDays[0, 1] = seed[1];
                for (int thisMonth = 1; thisMonth < 13; thisMonth++) {
                    int inspect = thisMonth - 1;
                    int offset = -1;
                    if (thisMonth == 2) offset = -2;
                    else if (thisMonth == 3) {
                        inspect = 1;
                        if (MarchException(year)) offset = 0;
                    }
                    int candidate = fmDays[inspect, 0] + offset;
                    int lastDay = GetDaysInMonth(year, thisMonth);
                    if (candidate > 0) {
                        fmDays[thisMonth, 0] = candidate;
                        if (candidate + 29 <= lastDay) fmDays[thisMonth, 1] = candidate + 29;
                    }
                    else {
                        candidate = fmDays[inspect, 1] + offset;
                        if (candidate > 0 && candidate <= lastDay) fmDays[thisMonth, 0] = candidate;
                    }
                }
            }
            else {
                fmDays[13, 0] = seed[0];
                fmDays[13, 1] = seed[1];
                for (int thisMonth = 12; thisMonth > 2; thisMonth--) {
                    int candidate = fmDays[thisMonth + 1, 0] + 1;
                    if (candidate - 29 > 0) {
                        fmDays[thisMonth, 1] = candidate;
                        fmDays[thisMonth, 0] = candidate - 29;
                    }
                    else {
                        fmDays[thisMonth, 0] = candidate;
                        if (candidate + 29 <= GetDaysInMonth(year, thisMonth)) fmDays[thisMonth, 1] = candidate + 29;
                    }
                }
                int offset = MarchException(year) ? 0 : 1;
                int JanLast = fmDays[3, 0] + offset;
                fmDays[1, 0] = JanLast;
                if (fmDays[3, 1] > 0) {
                    JanLast = fmDays[3, 1] + offset;
                    fmDays[1, 1] = JanLast;
                }
                if (JanLast - 2 <= GetDaysInMonth(year, 2)) fmDays[2, 0] = JanLast - 2;
            }
            for (int m = 1; m < 13; m++) {
                for (int j = 0; j < 2; j++)if (fmDays[m, j] > 0) Fullmoons.Add(new DateTime(year, m, fmDays[m, j]));
            }
            int i = forward ? 12 : 1;
            return new int[] { fmDays[i, 0], fmDays[i, 1] };
        }

        private void UpdateFullMoons(int year) {
            if (year < FirstPlottedYear) {
                int[] seed = new int[2];
                int i = 0;
                foreach (int d in Fullmoons.Where(m => m.Month == 1 && m.Year == FirstPlottedYear).OrderBy(m => m.Day).Select(m => m.Day)) seed[i++] = d;
                for (int y = FirstPlottedYear - 1; y >= year; y--) seed = PlotFullmoons(y, seed, false);
                FirstPlottedYear = year;
            }
            else if (year > LastPlottedYear) {
                int[] seed = new int[2];
                int i = 0;
                foreach (int d in Fullmoons.Where(m => m.Month == 12 && m.Year == LastPlottedYear).OrderBy(m => m.Day).Select(m => m.Day)) seed[i++] = d;
                for (int y = LastPlottedYear + 1; y <= year; y++) seed = PlotFullmoons(y, seed, true);
                LastPlottedYear = year;
            }
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            string[] days = new string[7];
            string[] da = new string[7];
            for (int d = 0; d < 7; d++) {
                days[d] = $"Day {d + 1}";
                da[d] = $"D{d + 1}";
            }
            dtfi.DayNames = days;
            dtfi.AbbreviatedDayNames = da;
        }
    }
}
  