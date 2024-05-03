using System;
using System.Collections.Generic;
using System.Text;

namespace WeirdCalendars {
    public class TraditionalChristianCalendar : WeirdCalendar {
       
        public override string Author => "Christoph Päper";
        public override Uri Reference => new Uri("https://calendars.fandom.com/wiki/Traditional_Christian_Calendar");

        protected override DateTime SyncDate => throw new NotImplementedException();
        protected override int SyncOffset => throw new NotImplementedException();


    }
}
