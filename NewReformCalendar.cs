using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NewReformCalendar : HankeHenryCalendar {

        public override string Author => "Ferenc Onodi";
        public override Uri Reference => new Uri("https://newreformcalendar.blogspot.com/");

        /*
        Says to start with 2019 but that is an obvious error given that the weekdays
        align in *2018 and a 2019 start does not produce the given leap year sequence.
        */
        protected override DateTime SyncDate => new DateTime(2018, 1, 1);

        public NewReformCalendar() => Title = "New Reform Calendar";
        
        public override bool IsLeapYear(int year, int era) {
            /*
            Says leap years are 2023, 2028, 2034, 2040, 2045, 2051, 2056, 2062, and
            2068. Proleptic leap years: 2017, 2012, 2006, 2000, 1995 ...
             */
            ValidateDateParams(year, era);
            int inc = Math.Sign(year - 2018);
            int d = inc < 0 ? 5 : 0;
            for (int y = 2018; y != year; y += inc) d += Offset(y);
            return d % 7 + Offset(year) > 6;

            int Offset(int y) => y % 4 == 0 && (y % 100 != 0 || y % 400 == 0) ? 2 : 1;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            string[] m = (string[])dtfi.MonthNames.Clone();
            m[12] = "Leap Week";
            string[] ma = (string[])dtfi.AbbreviatedMonthNames.Clone();
            ma[12] = "LW";
            SetNames(dtfi, m, ma);
        }
    }
}
