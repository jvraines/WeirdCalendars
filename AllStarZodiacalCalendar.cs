using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;

namespace WeirdCalendars {
    public class AllStarZodiacalCalendar : WeirdCalendar {
 
        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/All_Star_Zodiacal_Calendar");

        protected override DateTime SyncDate => new DateTime(2025, 4, 15);
        protected override int SyncOffset => -113;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("a", "Zodiacal distance (annual)"),
            ("b", "Zodiacal distance (monthly)")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch(month) {
                case 1:
                case 2:
                case 4:
                case 5:
                    return 31;
                case 3:
                    return 32;
                case 9:
                    return 29;
                case 12:
                    return IsLeapYear(year) ? 31 : 30;
                default:
                    return 30;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 550 % 78 % 4 == 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 31;
        }

        private static readonly double PtolemyEpoch = Time.JulianDay(149);
        
        public double ZodiacalDistanceAnnual(int year) {
            return ZodiacalDistanceMonthly(year, 1);
        }

        public double ZodiacalDistanceMonthly(int year, int month) {
            ValidateDateParams(year, month, 0);
            double month1 = ToDateTime(year, month, 1, 12, 0, 0, 0).JulianEphemerisDay();
            double monthLongitude = Transform.Precession((month - 1) * 30, 0, PtolemyEpoch, month1).longitude;
            double sunLongitude = Sun.Position(month1).longitude;
            return (sunLongitude - monthLongitude).ToPlusMinus180();
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("a") || format.FoundUnescaped("b")) {
                var ymd = ToLocalDate(time);
                fx.Format = format.ReplaceUnescaped("a", $"{ZodiacalDistanceAnnual(ymd.Year):0.000}°").ReplaceUnescaped("b", $"{ZodiacalDistanceMonthly(ymd.Year, ymd.Month):0.000}°");
            }
            return fx;
        }
    }
}
