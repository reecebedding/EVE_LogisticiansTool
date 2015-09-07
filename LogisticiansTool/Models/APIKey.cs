using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LogisticiansTool
{
    [Serializable]
    public class APIKey
    {
        [XmlIgnore]
        public bool IsValid { get; set; }

        [XmlIgnore]
        private string _characterName;
        [XmlAttribute]
        public string CharacterName { get { return _characterName ?? ""; } set { _characterName = value; } }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public int KeyID { get; set; }

        [XmlAttribute]
        public string VCode { get; set; }
        
        [XmlAttribute]
        public int CharacterID { get; set; }



        
    }
}
