using System;
using System.Globalization;
using System.Reflection;

namespace WeirdCalendars {

    /// <summary>
    /// A replacement class for Globalization.CultureInfo.
    /// </summary>
    public class CultureInfoWC : CultureInfo {

        /// <param name="calendar">A WeirdCalendar object.</param>
        /// <param name="name">A region name.</param>
        public CultureInfoWC(WeirdCalendar calendar, string name = "") : base(name) {
            SetupCulture(calendar);
        }

        /// <param name="calendarType">The System.Type of a Weird Calendar.</param>
        /// <param name="name">A region name.</param>
        public CultureInfoWC(Type calendarType, string name = "") : base(name) {
            WeirdCalendar cal;
            cal = (WeirdCalendar)Activator.CreateInstance(calendarType);
            SetupCulture(cal);
        }

        /// <param name="calendarName">The class name of a Weird Calendar.</param>
        /// <param name="name">A region name.</param>
        /// <exception cref="ArgumentOutOfRangeException">The class name was not found.</exception>
        public CultureInfoWC(string calendarName, string name = "") :base(name) {
            WeirdCalendar cal;
            try {
                cal = (WeirdCalendar)Activator.CreateInstance(Type.GetType($"WeirdCalendars.{calendarName},WeirdCalendars"));
            }
            catch {
                throw new ArgumentOutOfRangeException("calendarName", "Calendar type with that name not found.");
            }
            SetupCulture(cal);
        }

        private void SetupCulture(WeirdCalendar cal) {
            /*
             * Pull in default values according to name and set calendar.
             * Lots of weirdness here. DTFI must be cloned to make it writeable.
             * DTFI.Calendar cannot be directly set because CI.OptionalCalendars is read-only.
             * Instead, use reflection workaround on private field.
            */
            cal.Language = Name;
            CultureInfo clone = (CultureInfo)Clone();
            DateTimeFormatInfo dtfi = clone.DateTimeFormat;
            typeof(DateTimeFormatInfo).GetField("calendar", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(dtfi, cal);

            //Default deeper properties from this instantiation
            dtfi.MonthNames = (string[])DateTimeFormat.MonthNames.Clone();
            dtfi.MonthGenitiveNames = (string[])DateTimeFormat.MonthNames.Clone();
            dtfi.AbbreviatedMonthNames = (string[])DateTimeFormat.AbbreviatedMonthNames.Clone();
            dtfi.AbbreviatedMonthGenitiveNames = (string[])DateTimeFormat.AbbreviatedMonthNames.Clone();
            dtfi.DayNames = (string[])DateTimeFormat.DayNames.Clone();
            dtfi.AbbreviatedDayNames = (string[])DateTimeFormat.AbbreviatedDayNames.Clone();
            dtfi.ShortestDayNames = (string[])DateTimeFormat.ShortestDayNames.Clone();
            dtfi.MonthDayPattern = DateTimeFormat.MonthDayPattern;

            //Call calendar-specific formatting
            cal.CustomizeDTFI(dtfi);
            DateTimeFormat = dtfi;
        }
    }
}
