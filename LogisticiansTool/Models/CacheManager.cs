using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace LogisticiansTool
{
    public class OverrideCacheManager
    {
        public ICacheManager Cache = CacheFactory.GetCacheManager();
        private DataRepository _repository;
        private NLog.Logger _logger;

        public OverrideCacheManager()
        {
            _repository = new DataRepository();
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void RefreshItemPriceCache()
        {
            IEnumerable<int> fuels = _repository.GetAllShips().Select(x => x.FuelID);
            foreach (int fuelId in fuels)
            {
                double currentVal = 0;
                if (Cache.Contains(fuelId.ToString()))
                    currentVal = Convert.ToDouble(Cache.GetData(fuelId.ToString()));

                try
                {
                    string url = string.Format("{0}?typeid={1}&regionLimit={2}", "http://api.eve-central.com/api/marketstat", fuelId, 10000002);
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent", "LogisticalTool: Contact- Natalie Cruella");
                    Stream data = client.OpenRead(url);

                    XPathDocument doc = new XPathDocument(data);
                    XPathNavigator nav = doc.CreateNavigator();
                    XPathExpression expr;
                    expr = nav.Compile("/evec_api/marketstat/type/buy/percentile");
                    XPathNodeIterator iterator = nav.Select(expr);
                    iterator.MoveNext();

                    double newPrice = iterator.Current.ValueAsDouble;
                    Cache.Add(fuelId.ToString(), newPrice, CacheItemPriority.Normal, null, null);
                    _logger.Info("Cache was updated. Key: " + fuelId.ToString() + " Value: " + newPrice);
                }
                catch (Exception exn)
                {
                    Cache.Add(fuelId.ToString(), currentVal);
                    _logger.Error(string.Format("Unable to retrieve item cache value for : {0}. Exception: {1}", fuelId, exn.Message));
                }
            }

        }
    }
}
