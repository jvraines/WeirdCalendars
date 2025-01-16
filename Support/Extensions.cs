using AA.Net;
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
                string concat = FormatWC.DateAndTimeSeparator;
                switch (format) {
                    case "d":
                        format = FormatWC.ShortDatePattern;
                        break;
                    case "D":
                        format = FormatWC.LongDatePattern;
                        break;
                    case "f":
                        format = $"{FormatWC.LongDatePattern}{concat}{FormatWC.ShortTimePattern}";
                        break;
                    case "U":
                    case "F":
                        format = $"{FormatWC.LongDatePattern}{concat}{FormatWC.LongTimePattern}";
                        break;
                    case "g":
                        format = $"{FormatWC.ShortDatePattern}{concat}{FormatWC.ShortTimePattern}";
                        break;
                    case "G":
                        format = $"{FormatWC.ShortDatePattern}{concat}{FormatWC.LongTimePattern}";
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
            string f = time.ToString(format, culture);
            cal.OnDateFormatted();
            return f;
        }

        /// <summary>
        /// Converts an integer into Roman numeral notation.
        /// </summary>
        /// <param name="arabic">Number to convert.</param>
        /// <returns>A string of Roman numerals.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToRoman(this int arabic) {
            if (arabic > 9999) throw new ArgumentOutOfRangeException();
            int[] divisor = { 5000, 4000, 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] digit = { "V̅", "I̅V̅", "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
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

        internal static bool FoundUnescaped(this string format, string search) {
            string safeSearch = Regex.Escape(search);
            MatchCollection mm = Regex.Matches(format, $@"[""'][^""']+[""']|\\.|({safeSearch})");
            foreach (Match m in mm) if (m.Groups[1].Value != "") return true;
            return false;
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

        public static string Capitalize(this string s) => $"{s.Substring(0, 1).ToUpper()}{s.Substring(1).ToLower()}";

        public static string ToBase(this int value, int radix) {
            if (radix == 10) return value.ToString();
            if (radix > 16) throw new ArgumentOutOfRangeException("radix", "Maximum value is 16.");
            string result = string.Empty;
            int work = Math.Abs(value);
            do {
                result = "0123456789ABCDEF"[work % radix] + result;
                work /= radix;
            }
            while (work > 0);
            return $"{(value < 0 ? "-" : "")}{result}";
        }

        public static string ToBase(this double value, int radix, int places) {
            if (radix == 10) return value.ToString();
            string wholePart = ToBase((int)value, radix);
            string fracPart = string.Empty;
            value = Math.Abs(value);
            value -= (int)value;
            int p = 0;
            do {
                value *= radix;
                fracPart += "0123456789ABCDEF"[(int)value];
                value -= (int)value;
                p++;
            }
            while (p < places);
            return $"{wholePart}.{fracPart}";
        }

        public static string Dozenal(this int n) {
            return n.ToBase(12).Replace("A", "↊").Replace("B", "↋");
        }

        public static string Dozenal(this double n, int places) {
            return n.ToBase(12, places).Replace("A", "↊").Replace("B", "↋");
        }

        public static double ToLastUTMidnight(this double jd, bool isDynamical = true) {
            return (int)((isDynamical ? jd.JulianUniversalDay() : jd) + 0.5) - 0.5;
        }

        public static double ToClosestUTMidnight(this double jd, bool isDynamical = true) {
            return (int)Math.Round((isDynamical ? jd.JulianUniversalDay() : jd) + 0.5) - 0.5;
        }

        public static int FloorMod(this int a, int n) => a - n * (int)Math.Floor((double)a / n);
    }
}
