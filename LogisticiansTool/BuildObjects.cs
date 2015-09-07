using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace LogisticiansTool.DataAccess
{
    public static class BuildObjects
    {
        public static ConfigValue BuildConfigValue(ref SQLiteDataReader reader)
        {
            ConfigValue value = new ConfigValue() { Key = reader["Key"].ToString(), Value = reader["Value"].ToString() };
            return value;
        }

        public static Ship BuildShip(ref SQLiteDataReader reader)
        {
            Ship ship = new Ship() 
            { 
                ItemID = Convert.ToInt32(reader["ItemID"]), 
                ShipName = reader["ShipName"].ToString(), 
                FuelID = Convert.ToInt32(reader["FuelID"]), 
                FuelConsumption = Convert.ToInt32(reader["FuelConsumption"]), 
                CargoCapacity = Convert.ToDecimal(reader["CargoCapacity"]),
                JumpRange = Convert.ToInt64(reader["JumpRange"])
            };
            return ship;
        }

        public static SolarSystem BuildSolarSystem(ref SQLiteDataReader reader)
        {
            SolarSystem solarSystem = new SolarSystem()
            {
                RegionID = Convert.ToInt32(reader["RegionID"]),
                ConstellationID = Convert.ToInt32(reader["ConstellationID"]),
                SolarSystemID = Convert.ToInt32(reader["SolarSystemID"]),
                SolarSystemName = reader["SolarSystemName"].ToString(),
                X = Convert.ToDecimal(reader["x"]),
                Y = Convert.ToDecimal(reader["y"]),
                Z = Convert.ToDecimal(reader["z"]),
                xMax = Convert.ToDecimal(reader["xMax"]),
                xMin = Convert.ToDecimal(reader["xMin"]),
                yMax = Convert.ToDecimal(reader["yMax"]),
                yMin = Convert.ToDecimal(reader["yMin"]),
                zMax = Convert.ToDecimal(reader["zMax"]),
                zMin = Convert.ToDecimal(reader["zMin"]),
                Security = Convert.ToDecimal(reader["security"])
            };
            return solarSystem;
        }

        public static Station BuildStation(ref SQLiteDataReader reader)
        {
            Station station = new Station()
            {
               RegionID = Convert.ToInt32(reader["regionID"]),
               SolarSystemID = Convert.ToInt32(reader["solarSystemID"]),
               StationID = Convert.ToInt32(reader["stationID"]),
               StationName = reader["stationName"].ToString()
            };
            return station;
        }

        public static APIKey BuildAPIKey(ref SQLiteDataReader reader)
        {
            DateTime expires;
            DateTime.TryParse(reader["DateExpires"].ToString(), out expires);

            APIKey key = new APIKey()
            {
                KeyID = Convert.ToInt32(reader["KeyID"]),
                VCode = reader["VCode"].ToString(),
                Type = reader["Type"].ToString(),
                CharacterName = reader["CharacterName"].ToString()
            };
            return key;
        }
    }
}
