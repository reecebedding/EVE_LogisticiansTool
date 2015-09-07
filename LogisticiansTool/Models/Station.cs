using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogisticiansTool
{
    public class Station
    {
        public int StationID { get; set; }
        public string StationName { get; set; }
        public int SolarSystemID { get; set; }
        public SolarSystem SolarSystem { get; set; }
        public int RegionID { get; set; }
    }
}
