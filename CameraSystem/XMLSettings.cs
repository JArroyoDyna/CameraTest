using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraSystem
{
    using System.IO;
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Settings
    {
        [XmlElement("SettingsItem")]
        public List<CodesSettingsItem> SettingsItem { get; set; }

        public Settings()
        { 
        }

    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class CodesSettingsItem
    { 
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public string Min { get; set; }

        public string Max { get; set; }

        public string Value { get; set; }
    }
 
}
