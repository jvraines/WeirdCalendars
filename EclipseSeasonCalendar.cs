using AA.Net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class EclipseSeasonCalendar : WeirdCalendar {
        
        public override string Author => "Vincent Raines";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Eclipse_Season_Calendar");

        protected override DateTime SyncDate => new DateTime(2023, 9, 27);
        protected override int SyncOffset => 1397;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("e", "Eclipse type")
        };

        public override string SpecialDay(DateTime time) => GetEclipse(time);

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 10;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 2:
                case 7:
                    return 34;
                case 4:
                    return IsLeapYear(year) ? 35 : 34;
                case 9:
                    return IsLongYear(year) ? 35 : 34;
                default:
                    return 35;
            }
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            return !IsLeapYear(year) ? 346 : IsLongYear(year) ? 348 : 347;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return (year & 1) == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return day == 35 && (month == 4 || month == 9);
        }

        private bool IsLongYear(int year) => year % 8 == 0 && (year % 192 != 0 || year % 3840 == 0);

        public string GetEclipse(DateTime time) {
            double jd = time.Date.JulianEphemerisDay();
            var m = Moon.NextEclipse(jd);
            if (m.greatestEclipse < jd + 1) return $"{m.type} lunar eclipse";
            else {
                var s = Sun.NextEclipse(jd);
                if (s.greatestEclipse < jd + 1) return $"{s.type} solar eclipse";
            }
            return NoSpecialDay;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Nyx", "Moros", "Ker", "Thanatos", "Hypnos", "Nox", "Momus", "Nemesis", "Apate", "Eris", "", "", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("e")) fx.Format = format.ReplaceUnescaped("e", $"'{GetEclipse(time)}'");
            return fx;
        }
    }
}
