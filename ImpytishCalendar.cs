using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {

    public class ImpytishCalendar : WeirdCalendar {
        
        public override string Author => "Trần Thị Sắc";
        public override Uri Reference => new Uri("https://micronations.wiki/wiki/Impytish_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 7, 1);
        protected override int SyncOffset => -1999;

        public override CalendarRealization Realization => CalendarRealization.Fictional;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("n", "Holiday")
        };

        public override string SpecialDay(DateTime time) => GetHoliday(time);

        public override int GetDaysInMonth(int year, int month, int era) {
            return base.GetDaysInMonth(year, (month + 5) % 12 + 1, era);
        }

        public override int GetYear(DateTime time) {
            int y = base.GetYear(time);
            return y > 0 ? y : y - 1;
        }

        public string GetHoliday(DateTime time) {
            string h = NoSpecialDay;
            var (_, month, day, _) = ToLocalDate(time);
            switch (month) {
                case 7:
                    if (day == 1) h = "Northern New Year";
                    break;
                case 8:
                    if (day == 14) h = "Valentine";
                    break;
                case 9:
                    if (day == 1) h = "Zero Discrimination Day";
                    else if (day == 8) h = "Women's Day";
                    break;
                case 10:
                    if (day == 1) h = "April Fools";
                    else if (day == 16) h = "Easter";
                    else if (day == 30) h = "Vietic Unification Day";
                    break;
                case 11:
                    if (day == 1) h = "International Labor's Day";
                    else if (day == 7) h = "Impytish Foundation Day";
                    else if (day == 15) h = "May Fools";
                    break;
                case 12:
                    if (day == 1) h = "International Children's Day";
                    else if (day == 16) h = "Impytish Genders' Day";
                    break;
                case 1:
                    if (day == 1) h = "Impytish New Year Festival";
                    break;
                case 2:
                    if (day == 25) h = "Queen Sarc I's Birthday";
                    break;
                case 3:
                    if (day == 7) h = "Impytish Teachers' Day";
                    break;
                case 5:
                    if (day == 19) h = "Men's Day";
                    break;
                case 6:
                    if (day == 24) h = "Operation Nicholas Day";
                    else if (day == 25) h = "Christmas";
                    break;
            }
            return h;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "'Tháng Lũnh", "'Tháng Chúp", "'Tháng Wẻ", "'Tháng Bồn", "'Tháng Cử", "'Tháng Lao", "'Tháng Pành", "'Tháng Su", "'Tháng Quốt", "'Tháng Mười", "'Tháng Lùm", "'Tháng 'Thảo", "" }, new string[] { "Lũn", "Chú", "Wẻ", "Bồn", "Cử", "Lao", "Pàn", "Su", "Quố", "Mườ", "Lùm", "'Th", "" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            if (format.FoundUnescaped("n")) fx.Format = format.ReplaceUnescaped("n", $"\"{GetHoliday(time)}\"");
            FixNegativeYears(fx, time);
            return fx;
        }
    }
}
