using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogisticiansTool
{
    public class Contract
    {
        public int ContractID { get; set; }
        public int IssuerID { get; set; }
        public int IssuerCorpID { get; set; }
        public int AssigneeID { get; set; }
        public int StartStationID { get; set; }
        public int EndStationID { get; set; }

        public decimal Reward { get; set; }
        public decimal Collateral { get; set; }

        public string ContractType { get; set; }
        public string Status { get; set; }
        public DateTime Expiration { get; set; }
        public float Volume { get; set; }

        public Station StartStation { get; set; }
        public SolarSystem StartSystem { get; set; }

        public Station EndStation { get; set; }
        public SolarSystem EndSystem { get; set; }
    }
}
