using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LogisticiansTool
{
    [Serializable]
    [XmlRoot(ElementName="SavedRoute")]
    public class Route
    {
        public string RouteName { get; set; }

        private SolarSystem[] _systemRoute;
        [XmlArray(ElementName="Route")]
        public SolarSystem[] SystemRoute
        {
            get
            {
                return _systemRoute;
            }
            set
            {
                _systemRoute = value;
            }
        }
    }
}
