using System;
using System.Globalization;

namespace WeirdCalendars {
    public class OlympiadCalendar : WeirdCalendar {
        
        public override string Author => "Walter Ziobro";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Olympiad_Calendar_with_Smoothly_Spread_Month_Lengths");

        protected override DateTime SyncDate => new DateTime(2021, 1, 1);
        protected override int SyncOffset => 0;

        private static int[,] MonthDays = new int[,] {
            {30, 30, 31, 31},
            {30, 31, 30, 30},
            {31, 30, 30, 31},
            {30, 31, 31, 30},
            {31, 30, 30, 31},
            {30, 30, 31, 30},
            {31, 31, 30, 30},
            {30, 30, 31, 31},
            {31, 31, 30, 30},
            {30, 30, 30, 31},
            {30, 31, 31, 30},
            {31, 30, 30, 31}
        };
        public override int GetDaysInMonth(int year, int month, int era) {
            int y = year % 100 == 0 && year % 400 != 0 ? 1 : (year - 1) % 4;
            return MonthDays[month - 1, y];
        }

        public override bool IsLeapDay(int year, int month, int day, int era) => false;

        private static string[,] MonthNames = new string[,] {
            { "Undembro", "Dodembro", "Primembro", "Secondembro", "Terzembro", "Quartembro", "Qunintembro", "Sestembro", "Settembro", "Ottobro", "Novembro", "Dicembro" },
            { "Undembra", "Dodembra", "Primembra", "Secondembra", "Terzembra", "Quartembra", "Qunintembra", "Sestembra", "Settembra", "Ottobra", "Novembra", "Dicembra" }
        };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            var ymd = ToLocalDate(time);
            fx.MonthFullName = MonthNames[GetDaysInMonth(ymd.Year, ymd.Month, 0) - 30, ymd.Month - 1];
            fx.MonthShortName = fx.MonthFullName.Substring(0, 3);
            return fx;
        }
    }
}
