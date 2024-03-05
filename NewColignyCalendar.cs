using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NewColignyCalendar : WeirdCalendar {

        public override string Author => "Timey";
        public override Uri Reference => new Uri("https://time-meddler.co.uk/the-coligny-calendar/");
        // Author appears to have incorrect date on this web page (off by -1).

        protected override DateTime SyncDate => new DateTime(2023, 11, 12);
        protected override int SyncOffset => -1998;
        public override DateTime MinSupportedDateTime => new DateTime(1999, 10, 9);

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era); 
            int c = CycleYear(year);
            if (c == 1 || c == 3 && month > 6 && !IsSixth3(year)) month--;
            switch (month) {
                case 0:
                case 1:
                case 3:
                case 5:
                case 6:
                case 8:
                case 11:
                    return 30;
                case 9:
                    return c == 5 ? 30 : 29;
                default:
                    return 29;
            }
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            int c = CycleYear(year);
            return c == 1 || c == 3 && !IsSixth3(year) ? 13 : 12;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            switch (CycleYear(year)) {
                case 1:
                    return 384;
                case 2:
                case 4:
                    return 354;
                case 3:
                    return IsSixth3(year) ? 354 : 384;
                case 5:
                    return 355;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            int c = CycleYear(year);
            return c == 1 ? 1 : c == 3 && !IsSixth3(year) ? 7 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int c = CycleYear(year);
            return c == 1 && month == 1 || c == 3 && month == 7 && !IsSixth3(year);
        }

        private int CycleYear(int year) => ((year - 1) % 5 + 5) % 5 + 1;

        private bool IsSixth3(int year) => (year + 2) % 30 == 0;

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Samonios", "Dumannos", "Rivros", "Anagantios", "Ogronios", "Cutios", "Giamonios", "Semivisonna", "Equos", "Elembivos", "Edrinios", "Cantlos", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            var ymd = ToLocalDate(time);
            int c = CycleYear(ymd.Year);
            if (c == 1) {
                if (ymd.Month == 1) {
                    fx.MonthFullName = "Quimonios";
                    fx.MonthShortName = "Qui";
                }
                else {
                    fx.MonthFullName = dtfi.MonthNames[ymd.Month - 2];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[ymd.Month - 2];
                }
            }
            else if (c == 3 && !IsSixth3(ymd.Year)) {
                if (ymd.Month == 7) {
                    fx.MonthFullName = "Rantaranos";
                    fx.MonthShortName = "Ran";
                }
                else if (ymd.Month > 7) {
                    fx.MonthFullName = dtfi.MonthNames[ymd.Month - 2];
                    fx.MonthShortName = dtfi.AbbreviatedMonthNames[ymd.Month - 2];
                }
            }
            return fx;
        }
    }
}