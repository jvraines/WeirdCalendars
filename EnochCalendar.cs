using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class EnochCalendar : LeapWeekCalendar {
 
        public override string Author => "John P. Pratt";
        public override Uri Reference => new Uri("https://www.johnpratt.com/items/docs/2020/enoch_cal_model.html");

        protected override DateTime SyncDate => new DateTime(2018, 3, 23, 18, 0, 0); // from https://www.johnpratt.com/items/docs/lds/dates_calc.html
        protected override int SyncOffset => 4054;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)>() {
            ("n", "Holy day")
        };

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month == 12 && IsLeapYear(year) ? 37 : month % 3 == 1 ? 31 : 30;
        }

        protected override int GetFirstDayOfMonth(int year, int month) {
            return month % 3 == 1 ? 0 : 1;
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            int p = year % 1803 % 372 % 152;
            if (p > 68) p -= 68;
            switch (p % 27) {
                case 4:
                case 8:
                case 15:
                case 20:
                case 24:
                    return true;
                default:
                    return false;
            }
        }

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            return (DayOfWeek)((GetDayOfYear(time) % 91 + 5) % 7);
        }

        public string GetHolyDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 1);
            string h = "(none)";
            switch (month) {
                case 1:
                    if (day == 0) h = "Spring Equinox";
                    else if (day == 1) h = "New Year";
                    else if (day == 10) h = "Consecration";
                    else if (day == 13) h = "Lamb Day";
                    else if (day == 14) h = "Passover";
                    else if (day == 15) h = "Easter";
                    else if (day == 21) h = "End Passover";
                    break;
                case 3:
                    if (day == 4) h = "Firstfruits";
                    break;
                case 4:
                    if (day == 0) h = "Summer Solstice";
                    else if (day == 1) h = "Summer";
                    else if (day == 14) h = "Raven Day";
                    else if (day == 17) h = "Decision Day 1";
                    else if (day == 24) h = "Decision Day 2";
                    break;
                case 5:
                    if (day == 1) h = "Decision Day 3";
                    else if (day == 9) h = "Summer Fast";
                    break;
                case 7:
                    if (day == 0) h = "Autumn Equinox";
                    else if (day == 1) h = "Trumpets";
                    else if (day == 10) h = "Atonement";
                    else if (day == 14) h = "Tabernacles";
                    else if (day == 21) h = "End Tabernacles";
                    break;
                case 8:
                    if (day == 17) h = "Deluge";
                    break;
                case 9:
                    if (day == 24) h = "Lights";
                    break;
                case 10:
                    if (day == 0) h = "Winter Solstice";
                    else if (day == 1) h = "Winter";
                    else if (day == 14) h = "Winter Fast";
                    break;
                case 11:
                    if (day == 14) h = "Trees 1";
                    else if (day == 15) h = "Trees 2";
                    break;
                case 12:
                    if (day == 14) h = "Esther 1";
                    else if (day == 15) h = "Esther 2";
                    break;
            }
            return h;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Spring", "Mid Spring", "Late Spring", "Summer", "Mid Summer", "Late Summer", "Autumn", "Mid Autumn", "Late Autumn", "Winter", "Mid Winter", "Late Winter", "" }, new string[] { "Spr", "MSp", "LSup", "Sum", "MSu", "LSu", "Aut", "MAu", "LAu", "Win", "MWi", "LWi", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            CustomizeTimes(fx, time);
            var ld = ToLocalDate(time);
            fx.Format = format.ReplaceUnescaped("n", $"'{GetHolyDay(ld.Year, ld.Month, ld.Day)}'");
            return fx;
        }
    }
}
