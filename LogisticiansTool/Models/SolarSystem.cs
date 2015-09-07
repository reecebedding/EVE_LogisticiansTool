using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LogisticiansTool
{
    [Serializable]
    public class SolarSystem
    {
        [XmlIgnore]
        public int RegionID { get; set; }
        [XmlIgnore]
        public int ConstellationID { get; set; }
        [XmlAttribute]
        public int SolarSystemID { get; set; }
        [XmlAttribute]
        public string SolarSystemName { get; set; }
        [XmlIgnore]
        public decimal X { get; set; }
        [XmlIgnore]
        public decimal Y { get; set; }
        [XmlIgnore]
        public decimal Z { get; set; }
        [XmlIgnore]
        public decimal xMin { get; set; }
        [XmlIgnore]
        public decimal xMax { get; set; }
        [XmlIgnore]
        public decimal yMin { get; set; }
        [XmlIgnore]
        public decimal yMax { get; set; }
        [XmlIgnore]
        public decimal zMin { get; set; }
        [XmlIgnore]
        public decimal zMax { get; set; }
        [XmlIgnore]
        public decimal Security { get; set; }
        [XmlIgnore]
        public float DistanceWeight { get; set; }

        //Gets the display name for a system, by name and sec status
        [XmlIgnore]
        public string SolarSystemDisplayName 
        {
            get 
            {
                decimal secStatus = (Security < 0) ? 0.0m : Security;
                return SolarSystemName + " (" + secStatus + " Sec)";
            } 
        }
    }
}
