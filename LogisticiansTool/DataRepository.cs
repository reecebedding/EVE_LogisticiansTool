using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace LogisticiansTool
{
    public class DataRepository
    {
        private string ApplicationAPIKeyXMLPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Natalie Cruella\" + System.AppDomain.CurrentDomain.FriendlyName + @"\APIKeys.xml";
        private string ApplicationSavedRoutesXMLPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Natalie Cruella\" + System.AppDomain.CurrentDomain.FriendlyName + @"\SavedRoutes.xml";

        private string _connectionString = ConfigurationManager.ConnectionStrings["CurrentDB"].ToString();
        private SQLiteConnection _connectionObject;
        private const string _baseEveApi = "https://api.eveonline.com";

        public DataRepository()
        {
            _connectionObject = new SQLiteConnection(_connectionString);
        }

        public ConfigValue GetConfigValue(string key)
        {
            SQLiteCommand _cmd = _connectionObject.CreateCommand();
            _cmd.CommandText = "SELECT * FROM Config WHERE Key = @Key";
            _cmd.Parameters.Add("@Key", System.Data.DbType.String);
            _connectionObject.Open();
            SQLiteDataReader reader = _cmd.ExecuteReader();
            reader.Read();
            ConfigValue value = DataAccess.BuildObjects.BuildConfigValue(ref reader) ?? null;
            _connectionObject.Close();
            return value;
        }

        public IEnumerable<Ship> GetAllShips()
        {
            SQLiteCommand _cmd = _connectionObject.CreateCommand();
            _cmd.CommandText = "SELECT * FROM Ships";
            _connectionObject.Open();
            SQLiteDataReader reader = _cmd.ExecuteReader();
            List<Ship> ships = new List<Ship>();
            while (reader.Read())
            {
                Ship newShip = DataAccess.BuildObjects.BuildShip(ref reader);
                ships.Add(newShip);
            }
            _connectionObject.Close();
            return ships;
        }

        public IEnumerable<SolarSystem> GetSolarSystemsLikeName(string name)
        {
            SQLiteCommand _cmd = _connectionObject.CreateCommand();
            _cmd.CommandText = "SELECT * FROM mapSolarSystems WHERE solarSystemName LIKE '" + name + "%'";
            _connectionObject.Open();
            SQLiteDataReader reader = _cmd.ExecuteReader();
            List<SolarSystem> systems = new List<SolarSystem>();
            while (reader.Read())
            {
                systems.Add(DataAccess.BuildObjects.BuildSolarSystem(ref reader));
            }
            _connectionObject.Close();
            return systems;
        }

        public SolarSystem GetSolarSystemByID(int Id)
        {
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

        public IEnumerable<Contract> GetAvailableContractsByStatus(APIKey key, string status)
        {
            IEnumerable<Contract> contracts = GetAvailableContracts(key);
            return contracts.ToList().Where(x => x.Status == status);
        }

        public IEnumerable<Contract> GetAvailableContracts(APIKey key)
        {
            List<Contract> contracts = new List<Contract>();

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("user-agent", "LogisticiansTools: - Contact Natalie Cruella");
            string URL = string.Format("{0}/{1}/Contracts.xml.aspx?keyID={2}&vCode={3}", _baseEveApi, key.Type, key.KeyID, key.VCode);
            if (key.Type.ToUpper() == "CHAR")
            {
                URL += "&characterID=" + key.CharacterID;
            }
            System.IO.Stream dataStream = webClient.OpenRead(URL);
            string XMLData = new System.IO.StreamReader(dataStream).ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLData);
            XmlNodeList nodes = doc.GetElementsByTagName("row");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["type"].Value.ToString().ToLower() == "courier" && (node.Attributes["status"].Value.ToString().ToLower() == "outstanding" || node.Attributes["status"].Value.ToString().ToLower() == "inprogress"))
                {
                    Contract newContract = new Contract()
                    {
                        AssigneeID = Convert.ToInt32(node.Attributes["assigneeID"].Value),
                        ContractID = Convert.ToInt32(node.Attributes["contractID"].Value),
                        ContractType = node.Attributes["type"].Value.ToString(),
                        EndStationID = Convert.ToInt32(node.Attributes["endStationID"].Value),

                        Reward = Convert.ToDecimal(node.Attributes["reward"].Value),
                        Collateral = Convert.ToDecimal(node.Attributes["collateral"].Value),

                        Expiration = Convert.ToDateTime(node.Attributes["dateExpired"].Value),
                        IssuerCorpID = Convert.ToInt32(node.Attributes["issuerCorpID"].Value),
                        IssuerID = Convert.ToInt32(node.Attributes["issuerID"].Value),
                        StartStationID = Convert.ToInt32(node.Attributes["startStationID"].Value),
                        Status = node.Attributes["status"].Value.ToString(),
                        Volume = Convert.ToSingle(node.Attributes["volume"].Value)
                    };
                    newContract.StartStation = GetStationByID(newContract.StartStationID);
                    newContract.StartSystem = newContract.StartStation.SolarSystem;

                    newContract.EndStation = GetStationByID(newContract.EndStationID);
                    newContract.EndSystem = newContract.EndStation.SolarSystem;

                    contracts.Add(newContract);
                }
            }
            return contracts;
        }

        public Station GetStationByID(int stationID)
        {
            SQLiteCommand _cmd = _connectionObject.CreateCommand();
            _cmd.CommandText = "SELECT * FROM staStations WHERE stationID = '" + stationID + "'";
            _connectionObject.Open();
            SQLiteDataReader reader = _cmd.ExecuteReader();
            Station station = new Station();
            if (reader.Read())
            {
                station = DataAccess.BuildObjects.BuildStation(ref reader);
                _connectionObject.Close();
                station.SolarSystem = GetSolarSystemByID(station.SolarSystemID);
            }
            else
                station = GetConquerableStationByID(stationID);

            _connectionObject.Close();
            return station;
        }

        private Station GetConquerableStationByID(int stationID)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("user-agent", "LogisticiansTools: - Contact Natalie Cruella");
            string URL = string.Format("{0}/eve/ConquerableStationList.xml.aspx", _baseEveApi);
            System.IO.Stream dataStream = webClient.OpenRead(URL);
            string XMLData = new System.IO.StreamReader(dataStream).ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLData);
            XmlNodeList nodes = doc.GetElementsByTagName("row");
            Station station = new Station();
            foreach (XmlNode node in nodes)
            {
                if (Convert.ToInt32(node.Attributes["stationID"].Value) == stationID)
                {
                    station = new Station()
                    {
                        StationID = stationID,
                        SolarSystemID = Convert.ToInt32(node.Attributes["solarSystemID"].Value),
                        StationName = node.Attributes["stationName"].Value.ToString()
                    };
                    _connectionObject.Close();
                    station.SolarSystem = GetSolarSystemByID(station.SolarSystemID);
                    return station;
                }
            }
            return station;
        }

        public bool IsAPIValid(APIKey key)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("user-agent", "LogisticiansTools: - Contact Natalie Cruella");
            string URL = string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseEveApi, key.KeyID, key.VCode);

            System.IO.Stream dataStream = webClient.OpenRead(URL);
            string XMLData = new System.IO.StreamReader(dataStream).ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLData);
            XmlNode nod = doc.GetElementsByTagName("key")[0];
            int accessMask = Convert.ToInt32(nod.Attributes["accessMask"].Value);

            string apiType = GetAPIType(key);

            if (apiType.ToUpper() == "CHAR")
                return (accessMask & 67108864) == 67108864;
            else if (apiType.ToUpper() == "CORP")
                return (accessMask & 8388608) == 8388608;
            else
                return false;
        }

        public string GetAPIType(APIKey key)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("user-agent", "LogisticiansTools: - Contact Natalie Cruella");
            string URL = string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseEveApi, key.KeyID, key.VCode);

            System.IO.Stream dataStream = webClient.OpenRead(URL);
            string XMLData = new System.IO.StreamReader(dataStream).ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLData);
            string apiType = doc.GetElementsByTagName("key")[0].Attributes["type"].Value.ToString();
            apiType = (apiType == "Character" || apiType == "Account") ? "Char" : "Corp";
            return apiType;
        }

        public Dictionary<string, string> GetCharsOrCorpOnAPI(APIKey key)
        {
            Dictionary<string, string> chars = new Dictionary<string, string>();

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("user-agent", "LogisticiansTools: - Contact Natalie Cruella");
            string URL = string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseEveApi, key.KeyID, key.VCode);

            System.IO.Stream dataStream = webClient.OpenRead(URL);
            string XMLData = new System.IO.StreamReader(dataStream).ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLData);
            XmlNodeList nodes = doc.GetElementsByTagName("row");
            if (key.Type.ToLower() == "corp")
            {
                //chars.Add(nodes[0].Attributes["corporationName"].Value.ToString() + " (Corp)");
                chars.Add(nodes[0].Attributes["corporationName"].Value.ToString() + " (Corp)", nodes[0].Attributes["corporationID"].Value.ToString());
            }
            else if (key.Type.ToLower() == "char")
            {
                foreach (XmlNode nod in nodes)
                {
                    //chars.Add(nod.Attributes["characterName"].Value.ToString() + " (Pilot)");
                    chars.Add(nod.Attributes["characterName"].Value.ToString() + " (Pilot)", nod.Attributes["characterID"].Value.ToString());
                }
            }
            return chars;
        }

        public void DeleteAPIKey(APIKey key)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(APIKey));

            XmlDocument doc = new XmlDocument();
            doc.Load(ApplicationAPIKeyXMLPath);
            XmlNode docNode = doc.DocumentElement;
            XmlNodeList nodes = doc.GetElementsByTagName("APIKey");
            foreach (XmlNode nod in nodes)
            {
                APIKey apiKey = (APIKey)ser.Deserialize(new StringReader(nod.OuterXml));
                if (apiKey.VCode == key.VCode && apiKey.KeyID == key.KeyID && apiKey.CharacterName == key.CharacterName)
                {
                    docNode.RemoveChild(nod);
                    break;
                }
            }
            doc.Save(ApplicationAPIKeyXMLPath);
        }

        public IEnumerable<APIKey> GetAllAPIKeys()
        {
            List<APIKey> keys = new List<APIKey>();
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(APIKey));

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Natalie Cruella\" + System.AppDomain.CurrentDomain.FriendlyName + @"\APIKeys.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ApplicationAPIKeyXMLPath);
                XmlNodeList nodes = doc.GetElementsByTagName("APIKey");
                foreach (XmlNode nod in nodes)
                {
                    APIKey apiKey = (APIKey)ser.Deserialize(new StringReader(nod.OuterXml));
                    try
                    {
                        apiKey.IsValid = IsAPIValid(apiKey);
                    }
                    catch (Exception exn)
                    {                        
                        throw new System.Web.HttpException("Unable to contact the EVE API Server.");
                    }                    
                    keys.Add(apiKey);
                }
            }
            return keys;
        }

        public IEnumerable<Route> GetAllRoutes()
        {
            List<Route> routes = new List<Route>();
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Route));
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Natalie Cruella\" + System.AppDomain.CurrentDomain.FriendlyName + @"\SavedRoutes.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ApplicationSavedRoutesXMLPath);
                XmlNodeList nodes = doc.GetElementsByTagName("SavedRoute");
                foreach (XmlNode nod in nodes)
                {
                    Route savedRoute = (Route)ser.Deserialize(new StringReader(nod.OuterXml));
                    for (int i = 0; i < savedRoute.SystemRoute.Count(); i++)
                    {
                        savedRoute.SystemRoute[i] = GetSolarSystemByID(savedRoute.SystemRoute[i].SolarSystemID);
                    }
                    routes.Add(savedRoute);
                }
            }
            return routes;
        }


        public void AddSavedRoute(string name, IEnumerable<SolarSystem> route)
        {
            Route addRoute = new Route() { SystemRoute = route.ToArray(), RouteName = name };

            if (!File.Exists(ApplicationSavedRoutesXMLPath))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Natalie Cruella\" + System.AppDomain.CurrentDomain.FriendlyName);
                CreateBaseSavedRouteXML();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(ApplicationSavedRoutesXMLPath);

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Route));
            StringWriter wr = new StringWriter();
            ser.Serialize(wr, addRoute);
            XmlDocument newNode = new XmlDocument();
            newNode.LoadXml(wr.ToString());
            XmlNode newAPI = newNode.DocumentElement;


            XmlNode nod = doc.DocumentElement;
            XmlNode importNode = nod.OwnerDocument.ImportNode(newAPI, true);
            nod.AppendChild(importNode);
            doc.Save(ApplicationSavedRoutesXMLPath);
        }

        public void AddAPIKey(APIKey ApiKey)
        {
            if (!File.Exists(ApplicationAPIKeyXMLPath))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Natalie Cruella\" + System.AppDomain.CurrentDomain.FriendlyName);
                CreateBaseAPIXML();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(ApplicationAPIKeyXMLPath);

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(APIKey));
            StringWriter wr = new StringWriter();
            ser.Serialize(wr, ApiKey);
            XmlDocument newNode = new XmlDocument();
            newNode.LoadXml(wr.ToString());
            XmlNode newAPI = newNode.DocumentElement;


            XmlNode nod = doc.DocumentElement;
            XmlNode importNode = nod.OwnerDocument.ImportNode(newAPI, true);
            nod.AppendChild(importNode);
            doc.Save(ApplicationAPIKeyXMLPath);
        }

        public void CreateBaseAPIXML()
        {
            File.Create(ApplicationAPIKeyXMLPath).Close();
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode keysNode = doc.CreateElement("APIKeys");
            doc.AppendChild(keysNode);

            doc.Save(ApplicationAPIKeyXMLPath);
        }

        public void CreateBaseSavedRouteXML()
        {
            File.Create(ApplicationSavedRoutesXMLPath).Close();
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode keysNode = doc.CreateElement("SavedRoutes");
            doc.AppendChild(keysNode);

            doc.Save(ApplicationSavedRoutesXMLPath);
        }

        public void DeleteSavedRoute(string routeName)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Route));

            XmlDocument doc = new XmlDocument();
            doc.Load(ApplicationSavedRoutesXMLPath);
            XmlNode docNode = doc.DocumentElement;
            XmlNodeList nodes = doc.GetElementsByTagName("SavedRoute");
            foreach (XmlElement nod in nodes)
            {
                XmlNode nodRouteName = nod.GetElementsByTagName("RouteName")[0];
                if (nodRouteName.InnerText.ToUpper() == routeName.ToUpper())
                {
                    docNode.RemoveChild(nod);
                    break;
                }

            }
            doc.Save(ApplicationSavedRoutesXMLPath);
        }

        public void DeleteSavedRoutesFile()
        {
            if (File.Exists(ApplicationSavedRoutesXMLPath))
            {
                File.Delete(ApplicationSavedRoutesXMLPath);
            }
        }

        public void DeleteSavedAPIsFile()
        {
            if (File.Exists(ApplicationAPIKeyXMLPath))
            {
                File.Delete(ApplicationAPIKeyXMLPath);
            }
        }
    }
}