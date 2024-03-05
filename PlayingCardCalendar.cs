using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class PlayingCardCalendar : WeirdCalendar {

        public override string Author => "Karl Palmen";
        public override Uri Reference => new Uri("https://www.hermetic.ch/cal_stud/palmen/playcard.htm");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("w", "Week of year")
        };

        protected override (int Year, int Month, int Day, TimeSpan TimeOfDay) ToLocalDate(DateTime time) {
            return (time.Year, time.Month, time.Day, time.TimeOfDay);
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        private int GetISOWeek(DateTime time) {
            DayOfWeek day = GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday) time = time.AddDays(3);
            return GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private static string[] Index = { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King" };
        private static string[] IndexAbbr = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        private static string[] Suit = { "Clubs", "Diamonds", "Hearts", "Spades", "Joker" };
        private static string[] SuitAbbr = { "♣", "♦", "♥", "♠", "🃏" };
        private static string[] DayAbbr = { "Su", "M", "Tu", "W", "Th", "F", "Sa" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = new FormatWC(format, dtfi);
            int w = GetISOWeek(time) - 1;
            string d = "'", da = "";
            if (w < 52) {
                d += $"{Index[w % 13]} of ";
                da = IndexAbbr[w % 13];
            }
            d += $"{Suit[w / 13]}'";
            da += SuitAbbr[w / 13];
            fx.LongDatePattern = $"dddd, {d} yyyy";
            fx.ShortDatePattern = $"{DayAbbr[(int)GetDayOfWeek(time)]}/{da}/yyyy";
            fx.Format = format.ReplaceUnescaped("w", d);
            return fx;
        }
    }
}
