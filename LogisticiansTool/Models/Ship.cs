using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogisticiansTool
{
    [Serializable]
    public class Ship
    {
        public int ItemID { get; set; }
        public string ShipName { get; set; }
        public int FuelID { get; set; }
        public int FuelConsumption { get; set; }
        public decimal CargoCapacity { get; set; }
        public double JumpRange { get; set; }
    }
}
