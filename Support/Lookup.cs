using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WeirdCalendars.Support {
    internal class Lookup {
        internal Dictionary<Type, (string author, Uri reference)> info = new Dictionary<Type, (string author, Uri reference)>();

        internal Lookup() {
            XmlSerializer xml = new XmlSerializer(typeof(Dictionary<Type, (string author, Uri reference)>));
            using (FileStream stream = new FileStream("info.xml", FileMode.Open)) {
                info = (Dictionary<Type, (string author, Uri reference)>)xml.Deserialize(stream);
                stream.Close();
            }
        }
    }
}
