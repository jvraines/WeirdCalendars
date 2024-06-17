using System;
using System.Globalization;

namespace WeirdCalendars {
    public class AnnuaryCalendar : WeirdCalendar {

        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/palmen/anry.htm");
        
        protected override DateTime SyncDate => new DateTime(2023, 12, 30);
        protected override int SyncOffset => 2801;

        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        private int GetDaysInLeapMonth(int year) {
            int c = year % 100;
            return c == 99 || c == 0 && year % 400 != 0 ? 29 : 30;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            if (IsLeapMonth(year, month)) return GetDaysInLeapMonth(year);
            int r = year % 8;
            if (r == 3 && month > 9 || r == 6 && month > 5) month++;
            return (month & 1) == 1 ? 29 : 30;
        }

        public override int GetDaysInYear(int year, int era) {
            return 354 + (IsLeapYear(year) ? GetDaysInLeapMonth(year) : 0);
        }

        public override int GetMonthsInYear(int year, int era) {
            return IsLeapYear(year) ? 13 : 12;
        }

        public override int GetLeapMonth(int year, int era) {
            ValidateDateParams(year, era);
            int r = year % 8;
            return r == 0 ? 13 : r == 3 ? 9 : r == 6 ? 5 : 0;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return false;
        }

        public override bool IsLeapMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            int r = year % 8;
            return r == 0 && month == 13 || r == 3 && month == 9 || r == 6 && month == 5;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return year % 8 % 3 == 0;
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            var ymd = ToLocalDate(time);
            FormatWC fx = new FormatWC(format, dtfi);
            if (IsLeapYear(ymd.Year)) {
                int r = ymd.Year % 8;
                if (r == 0 && ymd.Month == 13) {
                    fx.MonthFullName = "Ocember";
                    fx.MonthShortName = "Oce";
                }
                else if (r == 3) {
                    if (ymd.Month == 9) {
                        fx.MonthFullName = "Jawgust";
                        fx.MonthShortName = "Jaw";
                    }
                    else if (ymd.Month > 9) {
                        fx.MonthFullName = dtfi.MonthNames[ymd.Month - 2];
                        fx.MonthShortName = dtfi.AbbreviatedMonthNames[ymd.Month - 2];
                    }
                }
                else if (r == 6) {
                    if (ymd.Month == 5) {
                        fx.MonthFullName = "Eapril";
                        fx.MonthShortName = "Eap";
                    }
                    else if (ymd.Month > 5) {
                        fx.MonthFullName = dtfi.MonthNames[ymd.Month - 2];
                        fx.MonthShortName = dtfi.AbbreviatedMonthNames[ymd.Month - 2];
                    }
                }
            }
            return fx;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Annuary", "Bebry", "Carch", "Daipril", "Fay", "Gyne", "Huly", "Igust", "Keptember", "Luctober", "Myvember", "Nicember", "" });
        }
    }
}