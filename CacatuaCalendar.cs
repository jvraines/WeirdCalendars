using System;

namespace WeirdCalendars {
    public class CacatuaCalendar : WeirdCalendar {

        public override string Author => "Denis Bredelet";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Cacatua_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 12, 27);
        protected override int SyncOffset => 1;

        private enum LeapType {
            None,
            Short,
            Full,
            Long
        }

        private LeapType GetLeapType(int year) {
            if (year % 5 != 0) return LeapType.None;
            if (year % 500 == 0) return LeapType.Long;
            if (year % 25 == 0) return LeapType.Full;
            return LeapType.Short;
        }

        public override int GetDaysInYear(int year, int era) {
            ValidateDateParams(year, era);
            switch (GetLeapType(year)) {
                case LeapType.None: return 364;
                case LeapType.Short: return 370;
                case LeapType.Full: return 371;
                default: return 372;
            }
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            switch (month) {
                case 1:
                case 4:
                case 6:
                case 7:
                case 9:
                case 10:
                    return GetLeapType(year) == LeapType.None ? 30 : 31;
                case 3:
                case 5:
                case 8:
                case 12:
                    return 31;
                case 2:
                    return GetLeapType(year) != LeapType.Long ? 30 : 31;
                default:
                    LeapType l = GetLeapType(year);
                    return l == LeapType.None || l == LeapType.Short ? 30 : 31;
            }
        }

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            return GetLeapType(year) != LeapType.None;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return !(day != 31 || month == 3 || month == 5 || month == 8 || month == 12);
        }
    }
}
