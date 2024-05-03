﻿using AA.Net;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WeirdCalendars {

    public static class Extensions {

        /// <summary>
        /// An exception indicating that the current culture is not a CurrentCultureWC, which is required when calling ToStringWC without a specified culture object.
        /// </summary>
        public static Exception BadCurrentCulture = new Exception("The current culture must be of derived type CultureInfoWC.");

        /// <summary>
        /// Extension method replacement for ToString.
        /// </summary>
        /// <param name="time">A DateTime value within the valid range for this calendar.</param>
        /// <returns>A string formatted according to the "G" standard format specifier for the current culture.</returns>
        /// 
        /// <remarks>Use this method to avoid exceptions and to handle calendar-specific custom formats.</remarks>
        public static string ToStringWC(this DateTime time) {
            try {
                return ToStringWC(time, "G", (CultureInfoWC)CultureInfo.CurrentCulture);
            }
            catch {
                throw BadCurrentCulture;
            }
        }

        /// <summary>
        /// Extension method replacement for ToString.
        /// </summary>
        /// <param name="time">A DateTime value within the valid range for this calendar.</param>
        /// <param name="format">A custom format string.</param>
        /// <returns>A string formatted according to the contents of the <paramref name="format"/> parameter for the current culture.</returns>
        /// <remarks>Use this method to avoid exceptions and to handle calendar-specific custom formats.</remarks>
        public static string ToStringWC(this DateTime time, string format) {
            try {
                return ToStringWC(time, format, (CultureInfoWC)CultureInfo.CurrentCulture);
            }
            catch {
                throw BadCurrentCulture;
            }
        }

        /// <summary>
        /// Extension method replacement for ToString.
        /// </summary>
        /// <param name="time">A DateTime value within the valid range for this calendar.</param>
        /// <param name="culture">A CultureInfoWC object.</param>
        /// <returns>A string formatted according to the "G" standard format specifier for the specified culture.</returns>
        /// <remarks>Use this method to avoid exceptions and to handle calendar-specific custom formats.</remarks>
        public static string ToStringWC(this DateTime time, CultureInfoWC culture) {
            return ToStringWC(time, "G", culture);
        }

        /// <summary>
        /// Extension method replacement for ToString.
        /// </summary>
        /// <param name="time">A DateTime value within the valid range for this calendar.</param>
        /// <param name="format">A custom format string.</param>
        /// <param name="culture">A CultureInfoWC object.</param>
        /// <returns>A string formatted according to the contents of the <paramref name="format"/> parameter for the specified culture.</returns>
        /// <remarks>Use this method to avoid exceptions and to handle calendar-specific custom formats.</remarks>
        public static string ToStringWC(this DateTime time, string format, CultureInfoWC culture) {
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            WeirdCalendar cal;
            //try {
                cal = (WeirdCalendar)dtfi.Calendar;
                FormatWC FormatWC = cal.GetFormatWC(dtfi, time, format);
                format = FormatWC.Format;
                switch (format) {
                    case "d":
                        format = FormatWC.ShortDatePattern;
                        break;
                    case "D":
                        format = FormatWC.LongDatePattern;
                        break;
                    case "f":
                        format = $"{FormatWC.LongDatePattern} {FormatWC.ShortTimePattern}";
                        break;
                    case "U":
                    case "F":
                        format = $"{FormatWC.LongDatePattern} {FormatWC.LongTimePattern}";
                        break;
                    case "g":
                        format = $"{FormatWC.ShortDatePattern} {FormatWC.ShortTimePattern}";
                        break;
                    case "G":
                        format = $"{FormatWC.ShortDatePattern} {FormatWC.LongTimePattern}";
                        break;
                    case "R":
                    case "r":
                        format = dtfi.RFC1123Pattern;
                        break;
                    case "t":
                        format = FormatWC.ShortTimePattern;
                        break;
                    case "T":
                        format = FormatWC.LongTimePattern;
                        break;
                };
                if (FormatWC.MonthFullName != null) format = format.ReplaceUnescaped("MMMM", $"\"{FormatWC.MonthFullName}\"").ReplaceUnescaped("MMM", $"\"{FormatWC.MonthShortName}\"");
                if (FormatWC.DayFullName != null) format = format.ReplaceUnescaped("dddd", $"\"{FormatWC.DayFullName}\"").ReplaceUnescaped("ddd", $"\"{FormatWC.DayShortName}\"");
            //}
            //catch { }
            //Call the regular method with possibly fixed-up format
            if (format.Length == 1) format = "%" + format;
            return time.ToString(format, culture);
        }

        /// <summary>
        /// Converts an integer into Roman numeral notation.
        /// </summary>
        /// <param name="arabic">Number to convert.</param>
        /// <returns>A string of Roman numerals.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToRoman(this int arabic) {
            if (arabic > 9999) throw new ArgumentOutOfRangeException();
            int[] divisor = new int[] { 5000, 4000, 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] digit = new string[] { "V̅", "I̅V̅", "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            string roman = "";
            int d = 0;
            while (arabic > 0) {
                while (arabic < divisor[d]) d++;
                int quotient = arabic / divisor[d];
                arabic %= divisor[d];
                for (int i = 0; i < quotient; i++) roman += digit[d];
            }
            return roman;
        }

        internal static string ReplaceUnescaped(this string format, string search, string replace) {
            if (format.Length == 1 && "dDfFUgGrRtT".Contains(format)) return format;
            string safeSearch = Regex.Escape(search);
            return Regex.Replace(format, $@"[""'][^""']+[""']|\\.|({safeSearch})", new MatchEvaluator(match => match.Groups[1].Value == "" ? match.Value : replace));
        }

        
        /// <summary>
        /// Parses a Pascal case string into a phrase based on capitals.
        /// </summary>
        /// <param name="s">Pascal case string to parse.</param>
        /// <returns>String parsed into a phrase with spaces separating the words.</returns>
        public static string ExpandPascalCase(this string s) {
            StringBuilder sb = new StringBuilder(s.Length + 5);
            bool lastNumber = false;
            bool pastFirst = false;
            foreach (char c in s) {
                bool thisUpper = char.IsUpper(c);
                bool thisNumber = char.IsNumber(c);
                if (thisUpper && pastFirst || thisNumber && !lastNumber) sb.Append(" ");
                sb.Append(c);
                pastFirst = true;
                lastNumber = thisNumber;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts an integer into an string representing the ordinal number.
        /// </summary>
        /// <param name="i">Integer to convert.</param>
        /// <returns>An ordinal number string.</returns>
        public static string ToOrdinal(this int i) {
            string suffix;
            if (i > 3 && i < 21) suffix = "th";
            else {
                switch (i % 10) {
                    case 1:
                        suffix = "st";
                        break;
                    case 2:
                        suffix = "nd";
                        break;
                    case 3:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
            }
            return $"{i}{suffix}";
        }

        public static string ToBase(this int value, int radix) {
            if (radix == 10) return value.ToString();
            string result = string.Empty;
            do {
                result = "0123456789ABCDEF"[value % radix] + result;
                value /= radix;
            }
            while (value > 0);
            return result;
        }

        public static double ToLastUTMidnight(this double jd, bool isDynamical = true) {
            return (int)((isDynamical ? jd.JulianUniversalDay() : jd) + 0.5) - 0.5;
        }
    }
}
