using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WeirdCalendars {
    public class SimpleLunisolarCalendar : WeirdCalendar {

        public override string Author => "Robert Pontisso";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Pontisso_Simple_Lunisolar_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 29);
        protected override int SyncOffset => 1;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            if (month == 6) return IsAbundantYear(year) ? 30 : 29;
            return (month & 1) == 1 ? 30 : 29; 
        }

        private bool IsAbundantYear(int year) => year % 5 == 0 && year % 200 != 0 && year % 500 != 0;

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return (IsLeapYear(year) ? 384 : 354) + (IsAbundantYear(year) ? 1 : 0);
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>() {
            {2025, false }
        };

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (!LeapYears.TryGetValue(year, out bool isLeap)) {
                int direction, testYear;
                DateTime testDate;
                Func<DateTime, bool> janTest;
                int extreme = LeapYears.Max(x => x.Key);
                if (year > extreme) {
                    direction = 1;
                    testYear = extreme;
                    testDate = ToDateTime(testYear, 1, 1, 0, 0, 0, 0).AddDays(GetDaysInYear(testYear));
                    janTest = new Func<DateTime, bool>(d => d.AddDays(29).Month == 12);
                }
                else {
                    direction = -1;
                    testYear = LeapYears.Min(x => x.Key);
                    testDate = ToDateTime(testYear, 1, 1, 0, 0, 0, 0);
                    janTest = new Func<DateTime, bool>(d => d.Month == 1 && d.Day > 1);
                }
                do {
                    int estDays = IsAbundantYear(testYear) ? 355 : 354;
                    testDate = testDate.AddDays(estDays * direction);
                    testYear += direction;
                    if (janTest(testDate)) {
                        isLeap = true;
                        testDate = testDate.AddDays(30 * direction);
                    }
                    else isLeap = false;
                    LeapYears.Add(testYear, isLeap);
                }
                while (testYear != year);
            }
            return isLeap;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 13;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            return IsLeapYear(year) ? 13 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 6 && day == 30;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa", "Lambda", "Mu", "Nu" });
        }
    }
}
