using System;
using System.Globalization;

namespace WeirdCalendars {
    public class NineteenBy19By19Minus19Calendar : ThreeSixtyBy19Calendar {

        public NineteenBy19By19Minus19Calendar() => Title = "19 × 19 × 19 − 19 Lunisolar Calendar";

        protected override int CycleLength => 361;

        internal override void CustomizeDTFI(DateTimeFormatInfo dtfi) {
            SetNames(dtfi, new string[] { "Dios", "Apellaios", "Audnaios", "Peritios", "Dystros", "Xandikos", "Artemisios", "Daisios", "Panemos", "Loios", "Gorpiaios", "Hypeberetaios", "Xandikos Embolimos" }, new string[] { "Dio", "Ape", "Aud", "Per", "Dys", "Xan", "Art", "Dai", "Pan", "Loi", "Gor", "Hyp", "XEm" });
        }
    }
}
