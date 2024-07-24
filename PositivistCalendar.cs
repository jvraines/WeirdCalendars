using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeirdCalendars {
    public class PositivistCalendar : FixedCalendar {

        public override string Author => "Auguste Comte";
        public override Uri Reference => new Uri("https://en.wikipedia.org/wiki/Positivist_calendar");

        protected override DateTime SyncDate => new DateTime(2020, 1, 1);
        protected override int SyncOffset => -1788;
        public override DateTime MinSupportedDateTime => new DateTime(1789, 1, 1);
        
        public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.LunisolarCalendar;

        public override List<(string FormatString, string Description)> CustomFormats => new List<(string FormatString, string Description)> {
            ("e", "Month theme"),
            ("c", "Day commemoration")
        };

        private static string[] MonthTheme = { "The early theocracy", "Ancient poets", "Ancient philosophy", "Ancient science", "Military civilization", "Catholicism", "Feudal civilization", "Modern epics", "Modern industry", "Modern drama", "Modern philosophy", "Modern politics", "Modern science" };
        public string GetTheme(int month) {
            if (month < 1 || month > 13) throw new ArgumentOutOfRangeException("month", "Month must be greater than 0 and less than 14.");
            return MonthTheme[month - 1];
        }

        private static string[,] Commemoration = new string[,] {
            { "Prometheus","Hesiod","Anaximander","Theophrastus","Militiades","St. Luke","Theodoric the Great","The Troubadours","Marco Polo","Lope de Vega","Albert the Great","Marie de Molina","Copernicus" },
            { "Hercules","Tyrtaeus","Anaximenes","Herophilus","Leonides","St. Cyprian","Pelayo","Bocaccio","Jacques Coeur","Moreto","Roger Bacon","Cosimo de Medici","Kepler" },
            { "Orpheus","Anacreon","Heraclitus","Eristratus","Aristides","St. Athanasius","Otho the Great","Cervantes","da Gama","Rojas","St. Bonaventure","Philippe de Comines","Huygens" },
            { "Ulysses","Pindar","Anaxagoras","Celsus","Cimon","St. Jerome","St. Henry","Rabelais","Napier","Otway","Ramus","Isabella of Castille","Jacques Bernoulli" },
            { "Lycurgus","Sophocles","Democritus","Galen","Xenophon","St. Ambrose","Villiers","La Fontaine","Lacaille","Lessing","Montaigne","Charles V","Bradley" },
            { "Romulus","Theocritus","Herodotus","Avicenna","Phocion","St. Monica","Don Juan de Austria","de Foe","Cook","Goëthe","Campanella","Henry IV","Volta" },
            { "Numa","Aeschylus","Thales","Hippocrates","Themistocles","St. Augustine","Alfred (the Great)","Ariosto","Columbus","Calderón","Thomas Aquinas","Louis XI","Galileo" },
            { "Belus","Scopas","Solon","Euclid","Pericles","Constantine","Charles Martel","Leonardo da Vinci","Benvenuto Cellini","Tirso","Thomas Hobbes","Coligny","Viète" },
            { "Sesostris","Zeuxis","Xenophanes","Aristarchus","Philip (of Macedon)","Theodosius","El Cid","Michelangelo","Amontons","Vondel","Pascal","Barneveldt","Wallis" },
            { "Menu","Ictinus","Empodocles","Theodosius of Bithynia","Demosthenes","St. Chrysostom","Richard I","Holbein","Harrison","Racine","Locke","Gustavus Adolphus","Clairaut" },
            { "Cyrus","Praxiteles","Thucydides","Hero","Ptolemy Lagus","St. Pulcheria","Joan of Arc","Poussin","Dolland","Voltaire","Vauvernargues","de Witt","Euler" },
            { "Zoroaster","Lysippus","Archytas","Pappus","Philipoemen","St. Genevieve of Paris","Albuquerque","Murillo","Arkwright","Alfieri","Diderot","Ruyter","D'Alembert" },
            { "The Druids","Apelles","Apollonius of Tyrana","Diophantus","Polybus","St. Gregory the Great","Bayard","Teniers","Conté","Schiller","Cabanis","William III","Lagrange" },
            { "Buddha","Phidias","Pythagoras","Apollonius","Alexander (the Great)","Hildebrand","Godfrey","Raphael","Vaucanson","Corneille","Bacon","William the Silent","Newton" },
            { "Fo-Hi","Aesop","Aristippus","Eudoxus","Junius Brutus","St. Benedict","St. Leo the Great","Froissart","Stevin","Alarcón","Grotius","Ximénez","Bergmann" },
            { "Lao-Tzu","Aristophanes","Antisthenes","Pytheas","Camillus","St. Boniface","Gerbert","Camões","Mariotte","Madame de Motteville","Fontenelle","Sully","Priestley" },
            { "Meng-Tzu","Terence","Zeno","Aristarchus","Fabricius","St. Isidore of Seville","Peter the Hermit","The Spanish Romantics","Papin","Madame de Sévigné","Vico","Colbert","Cavendish" },
            { "The Priests of Tibet","Phaedrus","Cicero","Eratosthenes","Hannibal","St. Lanfranc","Suger","Chateaubriand","Black","Lesage","Fréret","Walpole","Guyton Morveau" },
            { "The Priests of Japan","Juvenal","Epictetus","Ptolemy","Paulus Aemilius","St. Heloise","Alexander III","Sir Walter Scott","Jouffroy","Madame de Staal","Montesquieu","D'Aranda","Berthollet" },
            { "Manco Capac","Lucian","Tacitus","Albategnius","Marius","The Architects of the Middle Ages","St. Francis of Assisi","Manzoni","Dalton","Fielding","Buffon","Turgot","Berzelius" },
            { "Confucius","Plautus","Socrates","Hipparchus","Scipio","St. Bernard","Innocent III","Tasse","Watt","Molière","Leibnitz","Richelieu","Lavoisier" },
            { "Abraham","Ennius","Xenocrates","Varro","Augustus","St. Francis Xavier","St. Clothilda","Petrarch","Bernard de Palissy","Pergolesi","Adam Smith","Sidney","Harvey" },
            { "Joseph","Lucretius","Philo of Alexandria","Columella","Vespasian","St. Charles Borromeo","St. Bathilde","Thomas à Kempis","Guglielmini","Sacchini","Kant","Franklin","Boerhaave" },
            { "Samuel","Horace","St. John the Evangelist","Vitruvius","Adrian","St. Theresa","St. Stephen of Hungary","Madame de Lafayette","Duhamel","Gluck","Condorcet","Washington","Linnaeus" },
            { "Solomon","Tibullus","St. Justin","Strabo","Antony","St. Vincent de Paul","St. Elizabeth of Hungary","Fénelon","Saussure","Beethoven","Fichte","Jefferson","Haller" },
            { "Isaac","Ovid","St. Clement of Alexandria","Frontinus","Papinian","Bordalue","Blanche of Castille","Klopstock","Coulomb","Rossini","Joseph de Maistre","Bolívar","Lamarck" },
            { "St. John the Baptist","Lucan","Origen","Plutarch","Alexander Severus","William Penn","St. Ferdinand III","Byron","Carnot","Bellini","Hegel","Francia","Broussais" },
            { "Muhammad","Virgil","Plato","Pliny the Elder","Trajan","Bossuet","St. Louis","Milton","Montgolfier","Mozart","Hume","Cromwell","Gall" }
        };
        public string GetCommemoration(int month, int day) {
            ValidateDateParams(1789, month, day, 0);
            return Commemoration[day - 1, month - 1];
        }

        public override int GetDaysInMonth(int year, int month, int era) {
            ValidateDateParams(year, month, era);
            return month < 13 ? 28 : IsLeapYear(year) ? 30 : 29;
        }

        public override int GetMonthsInYear(int year, int era) {
            ValidateDateParams(year, era);
            return 13;
        }

        public override bool IsIntercalaryDay(int year, int month, int day) {
            ValidateDateParams(year, month, day, 0);
            return day > 28;
        }

        internal protected override string IntercalaryDayName(int year, int month, int day) {
            string n = base.IntercalaryDayName(year, month, day);
            return n == "Leap Day" ? "Festival of Holy Women" : "Festival of All Dead";
        }

        internal protected override string IntercalaryAbbreviatedDayName(int year, int month, int day) {
            return base.IntercalaryAbbreviatedDayName(year, month, day) == "Leap Day" ? "Womn" : "Dead";
        }

        public override bool IsLeapDay(int year, int month, int day, int era) {
            ValidateDateParams(year, month, day, era);
            return IsLeapYear(year) && month == 13 && day == 30;
        }

        protected override int WeekdayNumber(DateTime time) {
            return GetDayOfMonth(time) % 7;
        }

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            dtfi.FirstDayOfWeek = DayOfWeek.Monday;
            SetNames(dtfi, new string[] { "Moses", "Homer", "Aristotle", "Archimedes", "Caesar", "Saint Paul", "Charlemagne", "Dante", "Gutenberg", "Shakespeare", "Descartes", "Frederick", "Bichat" });
        }

        internal override FormatWC GetFormatWC(DateTimeFormatInfo dtfi, DateTime time, string format) {
            FormatWC fx = base.GetFormatWC(dtfi, time, format);
            if (format.FoundUnescaped("c") || format.FoundUnescaped("e")) {
                var ymd = ToLocalDate(time);
                fx.Format = format.ReplaceUnescaped("c", $"\"{GetCommemoration(ymd.Month, ymd.Day)}\"").ReplaceUnescaped("e", $"\"{GetTheme(ymd.Month)}\"");
            }
            return fx;
        }
    }
}