using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Configuration;

namespace LogisticiansTool.Models
{
    public static class Algorithms
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["CurrentDB"].ToString();

        public static IEnumerable<SolarSystem> GetShortestJumpPath(SolarSystem startSystem, SolarSystem endSystem, float shipJumpRange)
        {
            //List of systems to the nearest low sec (Only used if end system is in highsec)
            List<SolarSystem> lowsecToDestoRoute = new List<SolarSystem>();

            //Solar system object for the nearest lowsec system
            SolarSystem lowsecSystem = null;
            //Complete route from start to end
            List<SolarSystem> completeRoute = new List<SolarSystem>();

            //If the end system is in high sec
            if (endSystem.Security >= 0.5m)
            {
                IEnumerable<SolarSystem> neighbourSystems = GetSystemNeighbours(GetSystems(), GetAllJumpNeighbours().ToList(), endSystem.SolarSystemID);
                IEnumerable<SolarSystem> reverseJumpRoute = DijkstrasGateAlgorithm(endSystem, startSystem, shipJumpRange);
                foreach (SolarSystem system in reverseJumpRoute)
                {   
                    if (system.Security < 0.5m)
                    {
                        lowsecSystem = system;
                        break;
                    }
                    system.DistanceWeight = 0;
                    lowsecToDestoRoute.Add(system);
                }
                if (lowsecSystem != null && (lowsecSystem.SolarSystemID != startSystem.SolarSystemID))
                {
                    completeRoute.AddRange(DijkstrasJumpAlgorithm(startSystem, lowsecSystem, shipJumpRange));
                }
                else if (lowsecSystem != null)
                    completeRoute.Add(startSystem);

            }            
            //This is true when the end system is in lowsec or null
            else
            {
                //Calculate the jump route from the start system, to the end system as we know its in lowsec or null
                completeRoute.AddRange(DijkstrasJumpAlgorithm(startSystem, endSystem, shipJumpRange));
            }
            //Reverse the complete route
            lowsecToDestoRoute.Reverse();
            completeRoute.AddRange(lowsecToDestoRoute);
            
            return completeRoute;
        }

        private static List<SolarSystem> GetSystemNeighbours(List<SolarSystem> systems, List<JumpNeighbour> jumpNeighbours, int systemID)
        {
            //Get the system object for the parent node
            SolarSystem fromNode = systems.Where(x => x.SolarSystemID == systemID).FirstOrDefault();

            List<SolarSystem> neighbours = new List<SolarSystem>();
            
            //Get a list of all the system id's of neighbouring systems
            IEnumerable<long> systemIDs = jumpNeighbours.Where(x => x.fromSystem == systemID).Select(x => x.toSystem);
            //For every ID, build and collect the system objects for them
            foreach (long id in systemIDs)
            {
                SolarSystem system = systems.Where(x => x.SolarSystemID == id).FirstOrDefault();
                //Store the distance between the parent node, and the neighbour for distance
                system.DistanceWeight = GetEuclideanDistance(fromNode, system);
                neighbours.Add(system);
            }
            return neighbours;
        }

        public static IEnumerable<SolarSystem> DijkstrasJumpAlgorithm(SolarSystem startSystem, SolarSystem endSystem, float shipJumpRange)
        {

            //Get all system in the universe life and everything
            List<SolarSystem> _fullSystems = GetSystems();

            //The complete path
            List<long> shortestPath = new List<long>();
            List<SolarSystem> shortestPathSystems = new List<SolarSystem>();

            //Adds the start system to array, not needed but nice to know where your coming from
            shortestPath.Add(startSystem.SolarSystemID);

            //Set the "active" node to the start 
            SolarSystem currentNode = startSystem;
            while (true)
            {
                //Get all systems within jump range, can be swapped with GetSystemNeighbours function to get directly connected nodes
                List<SolarSystem> currentNodeNeighbours = GetSystemsInJumpRange(_fullSystems, currentNode, shipJumpRange);
                //If the system in the neighbours has been added to the complete route, ignore it. Were not backtracking
                IEnumerable<SolarSystem> unresolvedNeighbours = from system in currentNodeNeighbours
                                                                where !shortestPath.Contains(system.SolarSystemID)
                                                                select system;

                //No  neighbours left? .. Welp we have hit a dead end.
                if (unresolvedNeighbours.Count() == 0)
                    break;
                //If the neighbour system list has the end desto in it, we have made it!
                if (unresolvedNeighbours.Select(x => x.SolarSystemID).Contains(endSystem.SolarSystemID))
                {
                    shortestPath.Add(endSystem.SolarSystemID);
                    break;
                }

                //If we get this far, the destination has not been met, so get the next nearest node to end, and repeat
                SolarSystem nextAdjacentNode = GetNearestSystem(unresolvedNeighbours.ToList(), endSystem);

                //Add the next node to the complete path as it is where we are going next on our stepping stone journey.
                shortestPath.Add(nextAdjacentNode.SolarSystemID);
                currentNode = nextAdjacentNode;
            }
            foreach (long systemID in shortestPath)
                shortestPathSystems.Add(_fullSystems.Where(x => x.SolarSystemID == systemID).FirstOrDefault());
            return shortestPathSystems;
        }

        public static IEnumerable<SolarSystem> DijkstrasGateAlgorithm(SolarSystem startSystem, SolarSystem endSystem, float shipJumpRange)
        {

            //Get all system in the universe life and everything
            List<SolarSystem> _fullSystems = GetSystems();

            //The complete path
            List<long> shortestPath = new List<long>();
            List<SolarSystem> shortestPathSystems = new List<SolarSystem>();

            //Adds the start system to array, not needed but nice to know where your coming from
            shortestPath.Add(startSystem.SolarSystemID);

            //Set the "active" node to the start 
            SolarSystem currentNode = startSystem;
            while (true)
            {
                //Get all systems within jump range, can be swapped with GetSystemNeighbours function to get directly connected nodes
                List<SolarSystem> currentNodeNeighbours = GetSystemNeighbours(_fullSystems, GetAllJumpNeighbours().ToList(), currentNode.SolarSystemID);  //GetSystemsInJumpRange(_fullSystems, currentNode, shipJumpRange);
                //If the system in the neighbours has been added to the complete route, ignore it. Were not backtracking
                IEnumerable<SolarSystem> unresolvedNeighbours = from system in currentNodeNeighbours
                                                                where !shortestPath.Contains(system.SolarSystemID)
                                                                select system;

                //No  neighbours left? .. Welp we have hit a dead end.
                if (unresolvedNeighbours.Count() == 0)
                    break;
                //If the neighbour system list has the end desto in it, we have made it!
                if (unresolvedNeighbours.Select(x => x.SolarSystemID).Contains(endSystem.SolarSystemID))
                {
                    shortestPath.Add(endSystem.SolarSystemID);
                    break;
                }

                //If we get this far, the destination has not been met, so get the next nearest node to end, and repeat
                SolarSystem nextAdjacentNode = GetNearestSystem(unresolvedNeighbours.ToList(), endSystem);

                //Add the next node to the complete path as it is where we are going next on our stepping stone journey.
                shortestPath.Add(nextAdjacentNode.SolarSystemID);
                currentNode = nextAdjacentNode;
            }
            foreach (long systemID in shortestPath)
                shortestPathSystems.Add(_fullSystems.Where(x => x.SolarSystemID == systemID).FirstOrDefault());
            return shortestPathSystems;
        }

        public static float GetEuclideanDistance(SolarSystem A, SolarSystem B)
        {
            //Calculate the Delta values for the XYZ positions
            double deltaX = ((double)(A.X - B.X));
            double deltaY = ((double)(A.Y - B.Y));
            double deltaZ = ((double)(A.Z - B.Z));
            //Run Euclidean Algorithm
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            return distance;
        }

        private static SolarSystem GetNearestSystem(List<SolarSystem> neighbourSystems, SolarSystem parentSystem)
        {
            //Assume no systems are greater than the max of a float.
            float shortestDistance = float.MaxValue;
            //Representation of the nearest system to the system node
            SolarSystem shortestSystem = new SolarSystem();

            //Check each of the systems in the neighbour / jump range list
            foreach (SolarSystem system in neighbourSystems)
            {
                //Calculate the distance of the neighbour node from the system using the Euclidean Algorithm
                float currentDistance = GetEuclideanDistance(parentSystem, system);
                //If the system is closer to the node than any other, it is the closest within the jump range
                if (currentDistance < shortestDistance)
                {
                    //Set the shortest system and distance for reference
                    shortestDistance = currentDistance;
                    shortestSystem = system;
                }
            }
            //Set the vector weight property to distance for future reference
            shortestSystem.DistanceWeight = shortestDistance;
            return shortestSystem;
        }

        private static List<SolarSystem> GetSystemsInJumpRange(List<SolarSystem> systems, SolarSystem parentSystem, float jumpRange)
        {
            List<SolarSystem> nearest = new List<SolarSystem>();

            //Check every system in eve
            foreach (SolarSystem system in systems)
            {
                //If the solar system is not the current system we are in, check it.
                if (system.SolarSystemID != parentSystem.SolarSystemID)
                {
                    if (system.Security < 0.5m)
                    {
                        //Get the distance between the system and the current system using the Euclidean Algorithm, if the distance is less than the Max jump range, include it.
                        if (GetEuclideanDistance(parentSystem, system) <= jumpRange)
                            nearest.Add(system);
                    }
                }
            }
            //List of systems in jump range
            return nearest;
        }


        #region DBRelated

        private static List<SolarSystem> GetSystems()
        {
            SQLiteConnection con = new SQLiteConnection(_connectionString);
            SQLiteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM mapSolarSystems";
            con.Open();

            SQLiteDataReader reader = cmd.ExecuteReader();
            List<SolarSystem> systems = new List<SolarSystem>();
            while (reader.Read())
            {
                systems.Add(BuildSolarSystem(ref reader));
            }
            return systems;
        }

        private static IEnumerable<JumpNeighbour> GetAllJumpNeighbours()
        {
            SQLiteConnection con = new SQLiteConnection(_connectionString);
            SQLiteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM mapSolarSystemJumps";
            con.Open();

            SQLiteDataReader reader = cmd.ExecuteReader();
            List<JumpNeighbour> jumps = new List<JumpNeighbour>();
            while (reader.Read())
            {
                jumps.Add(new JumpNeighbour() { fromSystem = Convert.ToInt64(reader["fromSolarSystemID"]), toSystem = Convert.ToInt64(reader["toSolarSystemID"]) });
            }
            return jumps;
        }

        private static IEnumerable<SolarSystem> GetJumpNeighboursForSystem(SolarSystem parentSystem)
        {
            List<SolarSystem> systems = new List<SolarSystem>();

            SQLiteConnection con = new SQLiteConnection(_connectionString);
            SQLiteCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM mapSolarSystemJumps WHERE fromSolarSystemID = " + parentSystem.SolarSystemID;
            con.Open();

            SQLiteDataReader reader = cmd.ExecuteReader();
            //List<JumpNeighbour> jumps = new List<JumpNeighbour>();
            while (reader.Read())
            {
                //jumps.Add(new JumpNeighbour() { fromSystem = Convert.ToInt64(reader["fromSolarSystemID"]), toSystem = Convert.ToInt64(reader["toSolarSystemID"]) });
                systems.Add(GetSolarSystemByID(Convert.ToInt32(reader["toSolarSystemID"])));
            }
            return systems;
        }

        private static SolarSystem BuildSolarSystem(ref SQLiteDataReader reader)
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
                Security = Math.Round(Convert.ToDecimal(reader["security"]), 1)
            };
            return solarSystem;
        }

        private static SolarSystem GetSolarSystemByID(int Id)
        {
            SQLiteConnection _connectionObject = new SQLiteConnection(_connectionString);
            SQLiteCommand _cmd = _connectionObject.CreateCommand();
            _cmd.CommandText = "SELECT * FROM mapSolarSystems WHERE solarSystemID = '" + Id + "'";
            _connectionObject.Open();
            SQLiteDataReader reader = _cmd.ExecuteReader();
            SolarSystem system = new SolarSystem();
            reader.Read();
            system = DataAccess.BuildObjects.BuildSolarSystem(ref reader);
            _connectionObject.Close();
            return system;
        }


        #endregion
    }

    class JumpNeighbour
    {
        public long fromSystem { get; set; }
        public long toSystem { get; set; }
    }
}
