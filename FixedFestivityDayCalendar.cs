using System;
using System.Collections.Generic;
using System.Text;

namespace WeirdCalendars {
    public class FixedFestivityDayCalendar : FixedCalendar {
        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Fixed_Festivity_Day_Calendar");

        protected override DateTime SyncDate => new DateTime(2024, 1, 1);
        protected override int SyncOffset => 0;

        // As defined, common year has 366 days! Something is amiss.
        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return (month & 1) == 1 || month == 12 && IsLeapYear(year) ? 31 : 30;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 31;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day);
            if ((month & 1) == 1) {
                switch (day) {
                    case 1:
                    case 9:
                    case 17:
                    case 24:
                        return true;
                    default:
                        return false;
                }
            }
            else {
                switch (day) {
                    case 8:
                    case 23:
                    case 31:
                        return true;
                    default:
                        return false;
                }
            }
        }

        protected internal override string IntercalaryDayName(int year, int month, int day) {
            return "Holiday";
        }

        protected internal override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return "Hol";
        }

        protected override int WeekdayNumber(DateTime time) {
            var ld = ToLocalDate(time);
            int d = ld.Day;
            if ((ld.Month & 1) == 1) d -= d > 24 ? 3 : d > 9 ? 2 : 1;
            else d -= d > 23 ? 2 : d > 8 ? 1 : 0;
            return d % 7;
        }
    }
}
