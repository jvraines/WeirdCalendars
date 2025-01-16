using System;
using System.Collections.Generic;
using System.Globalization;
using AA.Net;

namespace WeirdCalendars {
    public class FrenchRepublicanCalendar : WeirdCalendar {

        public override string Author => "Charles-Gilbert Romme et al.";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/French_Republican_calendar");

        private bool isRevised = false;
        public bool IsRevised {
            get => isRevised;
            private set {
                isRevised = value;
                Notes = $"Reckoning leap years according to the {(isRevised ? "Delambre-Romme proposal." : "original equinox rule.")}";
            }
        }

        /// <param name="revised">False (default) for leap years based on the fall equinox or true for leap years following the Delambre-Romme proposal.</param>
        public FrenchRepublicanCalendar(bool revised) {
            IsRevised = revised;
        }

        public FrenchRepublicanCalendar() {
            IsRevised = false;
        }

        protected override DateTime SyncDate => new DateTime(2020, 9, 22);
        protected override int SyncOffset => -1791;

        public override CalendarRealization Realization => CalendarRealization.Archaic;

        public override DateTime MaxSupportedDateTime => IsRevised ? DateTime.MaxValue : VSOPLimit; //bound for equinox accuracy
        public override DateTime MinSupportedDateTime => new DateTime(1792, 9, 22);

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string, string)> {
            ("n", "Rural day name")
        };

        public enum DayOfWeekWC {
            Blank = -1,
            primidi,
            duodi,
            tridi,
            quartidi,
            quintidi,
            sextidi,
            septidi,
            octidi,
            nonidi,
            décadi
        }

        public override int DaysInWeek => 10;

        public override DayOfWeek GetDayOfWeek(DateTime time) {
            ValidateDateTime(time);
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) throw BadWeekday;
            int wn = WeekdayNumber(time);
            if (wn > 6) throw BadWeekday;
            return (DayOfWeek)wn;
        }

        public DayOfWeekWC GetDayOfWeekWC(DateTime time) {
            ValidateDateTime(time); 
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) return DayOfWeekWC.Blank;
            return (DayOfWeekWC)WeekdayNumber(time);
        }

        public bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return month == 12 && day > 30;
        }

        private int WeekdayNumber(DateTime time) {
            return (GetDayOfMonth(time) - 1) % 10;
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 12 ? 30 : IsLeapYear(year) ? 36 : 35;
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return month == 12 && day == 36 && IsLeapYear(year);
        }

        private static Dictionary<int, bool> LeapYears = new Dictionary<int, bool>();

        public override bool IsLeapYear(int year, int era) {
            ValidateDateParams(year, era);
            if (year < 15) return year == 3 || year == 7 || year == 11;
            if (IsRevised) return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0 && year % 4000 != 0);
            int gYear = year - SyncOffset;
            if(!LeapYears.TryGetValue(gYear, out bool ly)) {
                const double ParisOffset = 2.0 / 24;
                int yearStart = (int)(Earth.SeasonStart(gYear, Earth.Season.September) + 0.5 + ParisOffset);
                int yearEnd = (int)(Earth.SeasonStart(gYear + 1, Earth.Season.September) + 0.5 + ParisOffset);
                ly = yearEnd - yearStart == 366;
                LeapYears.Add(gYear, ly);
            }
            return ly;
        }

        protected override long HourTicks => TimeSpan.TicksPerDay / 10;
        protected override long MinuteTicks => TimeSpan.TicksPerDay / 1000;
        protected override long SecondTicks => TimeSpan.TicksPerDay / 100000;
        protected override long MilliTicks => TimeSpan.TicksPerDay / 100000000;

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era) {
            if (hour < 0 || hour > 9) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 99) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 99) throw new ArgumentOutOfRangeException("second");
            TimeSpan clock = TimeSpan.FromTicks(hour * HourTicks + minute * MinuteTicks + second * SecondTicks + millisecond * MilliTicks);
            return base.ToDateTime(year, month, day, clock.Hours, clock.Minutes, clock.Seconds, clock.Milliseconds, era);
        }

        /// <summary>
        /// Gets the name of a date on the Rural Calendar.
        /// </summary>
        /// <param name="month">A positive integer that represents the month.</param>
        /// <param name="day">A positive integer that represents the day.</param>
        /// <returns>A string representing the Rural day name.</returns>
        public string GetRuralDayName(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            if (month == 12 && day > 30) return ""; 
            return RuralName[month - 1, day - 1];
        }

        private static string[,] RuralName = new string[,] {
            { "Raisin", "Safran", "Châtaigne", "Colchique", "Cheval", "Balsamine", "Carotte", "Amaranthe", "Panais", "Cuve", "Pomme de terre", "Immortelle", "Potiron", "Réséda", "Âne", "Belle de nuit", "Citrouille", "Sarrasin", "Tournesol", "Pressoir", "Chanvre", "Pêche", "Navet", "Amaryllis", "Boeuf", "Aubergine", "Piment", "Tomate", "Orge", "Tonneau" },
            { "Pomme", "Céleri", "Poire", "Betterave", "Oie", "Héliotrope", "Figue", "Scorsonère", "Alisier", "Charrue", "Salsifis", "Mâcre", "Topinambour", "Endive", "Dindon", "Chervis", "Cresson", "Dentelaire", "Grenade", "Herse", "Bacchante", "Azerole", "Garance", "Orange", "Faisan", "Pistache", "Macjonc", "Coing", "Cormier", "Rouleau" },
            { "Raiponce", "Turneps", "Chicorée", "Nèfle", "Cochon", "Mâche", "Chou-fleur", "Miel", "Genièvre", "Pioche", "Cire", "Raifort", "Cèdre", "Sapin", "Chevreuil", "Ajonc", "Cyprès", "Lierre", "Sabine", "Hoyau", "Érable à sucre", "Bruyère", "Roseau", "Oseille", "Grillon", "Pignon", "Liège", "Truffe", "Olive", "Pelle" },
            { "Tourbe", "Houille", "Bitume", "Soufre", "Chien", "Lave", "Terre végétale", "Fumier", "Salpêtre", "Fléau", "Granit", "Argile", "Ardoise", "Grès", "Lapin", "Silex", "Marne", "Pierre à chaux", "Marbre", "Van", "Pierre à plâtre", "Sel", "Fer", "Cuivre", "Chat", "Étain", "Plomb", "Zinc", "Mercure", "Crible" },
            { "Lauréole", "Mousse", "Fragon", "Perce-neige", "Taureau", "Laurier-thym", "Amadouvier", "Mézéréon", "Peuplier", "Coignée", "Ellébore", "Brocoli", "Laurier", "Avelinier", "Vache", "Buis", "Lichen", "If", "Pulmonaire", "Serpette", "Thlaspi", "Thimelé", "Chiendent", "Trainasse", "Lièvre", "Guède", "Noisetier", "Cyclamen", "Chélidoine", "Traîneau" },
            { "Tussilage", "Cornouiller", "Violier", "Troène", "Bouc", "Asaret", "Alaterne", "Violette", "Marceau", "Bêche", "Narcisse", "Orme", "Fumeterre", "Vélar", "Chèvre", "Épinard", "Doronic", "Mouron", "Cerfeuil", "Cordeau", "Mandragore", "Persil", "Cochléaria", "Pâquerette", "Thon", "Pissenlit", "Sylvie", "Capillaire", "Frêne", "Plantoir" },
            { "Primevère", "Platane", "Asperge", "Tulipe", "Poule", "Bette", "Bouleau", "Jonquille", "Aulne", "Couvoir", "Pervenche", "Charme", "Morille", "Hêtre", "Abeille", "Laitue", "Mélèze", "Ciguë", "Radis", "Ruche", "Gainier", "Romaine", "Marronnier", "Roquette", "Pigeon", "Lilas", "Anémone", "Pensée", "Myrtille", "Greffoir" },
            { "Rose", "Chêne", "Fougère", "Aubépine", "Rossignol", "Ancolie", "Muguet", "Champignon", "Hyacinthe", "Râteau", "Rhubarbe", "Sainfoin", "Bâton d'or", "Chamerisier", "Ver à soie", "Consoude", "Pimprenelle", "Corbeille d'or", "Arroche", "Sarcloir", "Statice", "Fritillaire", "Bourrache", "Valériane", "Carpe", "Fusain", "Civette", "Buglosse", "Sénevé", "Houlette" },
            { "Luzerne", "Hémérocalle", "Trèfle", "Angélique", "Canard", "Mélisse", "Fromental", "Martagon", "Serpolet", "Faux", "Fraise", "Bétoine", "Pois", "Acacia", "Caille", "oeillet", "Sureau", "Pavot", "Tilleul", "Fourche", "Barbeau", "Camomille", "Chèvrefeuille", "Caille-lait", "Tanche", "Jasmin", "Verveine", "Thym", "Pivoine", "Chariot" },
            { "Seigle", "Avoine", "Oignon", "Véronique", "Mulet", "Romarin", "Concombre", "Échalote", "Absinthe", "Faucille", "Coriandre", "Artichaut", "Girofle", "Lavande", "Chamois", "Tabac", "Groseille", "Gesse", "Cerise", "Parc", "Menthe", "Cumin", "Haricot", "Orcanète", "Pintade", "Sauge", "Ail", "Vesce", "Blé", "Chalémie" },
            { "Épeautre", "Bouillon blanc", "Melon", "Ivraie", "Bélier", "Prêle", "Armoise", "Carthame", "Mûre", "Arrosoir", "Panic", "Salicorne", "Abricot", "Basilic", "Brebis", "Guimauve", "Lin", "Amande", "Gentiane", "Écluse", "Carline", "Câprier", "Lentille", "Aunée", "Loutre", "Myrte", "Colza", "Lupin", "Coton", "Moulin" },{ "Prune", "Millet", "Lycoperdon", "Escourgeon", "Saumon", "Tubéreuse", "Sucrion", "Apocyn", "Réglisse", "Échelle", "Pastèque", "Fenouil", "Épine vinette", "Noix", "Truite", "Citron", "Cardère", "Nerprun", "Tagette", "Hotte", "Églantier", "Noisette", "Houblon", "Sorgho", "Écrevisse", "Bigarade", "Verge d'or", "Maïs", "Marron", "Panier" } };

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Vendémiaire", "Brumaire", "Frimaire", "Nivôse", "Pluvôse", "Ventôse", "Germinal", "Floréal", "Prairial", "Messidor", "Thermidor", "Fructidor", "" }, null, new string[] { "primidi", "duodi", "tridi", "quartidi", "quintidi", "sextidi", "septidi" });
        }

        private static string[] Festival = { "Fête de la vertu", "Fête du génie", "Fête du travail", "Fête de l'opinion", "Fête des récompenses", "Fête de la révolution" };

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            CustomizeTimes(fx, time);
            var ymd = ToLocalDate(time);
            if (IsIntercalaryDay(ymd.Year, ymd.Month, ymd.Day)) fx.DayFullName = Festival[ymd.Day - 31];
            else {
                fx.DayFullName = Enum.GetName(typeof(DayOfWeekWC), GetDayOfWeekWC(time));
            }
            fx.Format = format.ReplaceUnescaped("n", $"'{GetRuralDayName(ymd.Year, ymd.Month, ymd.Day)}'");
            fx.DayShortName = fx.DayFullName.Substring(0, 3);
            string roman = $"'{ymd.Year.ToRoman()}'";
            fx.LongDatePattern = FixDigits(dtfi.LongDatePattern, roman, roman);
            fx.ShortDatePattern = FixDigits(dtfi.ShortDatePattern, roman, roman);
            fx.Format = FixDigits(fx.Format, roman, roman);
            return fx;
        }
    }
}
