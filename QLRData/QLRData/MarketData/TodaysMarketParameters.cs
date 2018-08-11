using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class TodaysMarketParameters
    {
        private Dictionary<string, MarketConfiguration> _configurations;
        private Dictionary<MarketObject, Dictionary<string, Dictionary<string, string>>> _marketObjects;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TodaysMarketParameters()
        {

        }

        public Dictionary<string, MarketConfiguration> Configurations()
        {
            return _configurations;
        }

        public bool HasConfiguration(string configuration)
        {
            return _configurations.ContainsKey(configuration);
        }

        public bool HasMarketObject(MarketObject o)
        {
            return _marketObjects.ContainsKey(o);
        }

        //! EUR => Yield/EUR/EUR6M, USD => Yield/USD/USD3M etc.
        public Dictionary<string, string> Mapping(MarketObject o, string configuration)
        {
            QLNet.Utils.QL_REQUIRE(HasConfiguration(configuration), () => "configuration " + configuration + " not found");
            
            if (_marketObjects.ContainsKey(o))
            {
                if(_marketObjects[o].ContainsKey(MarketObjectId(0, configuration)))
                {
                    return _marketObjects[o][MarketObjectId(0, configuration)];
                }                
            }
            QLNet.Utils.QL_FAIL("market object of type " + o + " with id " + MarketObjectId(o, configuration) + " specified in configuration " + configuration + " not found");
            return null;
        }

        //! Build a vector of all the curve specs (may contain duplicates)
        public List<string> CurveSpecs(string configuration)
        {
            List<string> specs = new List<string>();
            for (int i = 0; i < MarketConfiguration.NumberOfMarketObjects; ++i)
            {
                // swap indices have to be exlcuded here...
                if ((MarketObject)i != MarketObject.SwapIndexCurve && _marketObjects.ContainsKey((MarketObject)i))
                {
                    CurveSpecs(_marketObjects[(MarketObject)i], MarketObjectId((MarketObject)i, configuration), specs);
                }
            }
            return specs;
        }

        //! Individual term structure ids for a given configuration
        public string MarketObjectId(MarketObject o, string configuration)
        {
            QLNet.Utils.QL_REQUIRE(HasConfiguration(configuration), () => "configuration " + configuration + " not found");
            // TODO: Fixme!!
            return _configurations[configuration].ToString(); // (o);
        }

        private void CurveSpecs(Dictionary<string, Dictionary<string, string>> m, string id, List<string> specs)
        {
            // extract all the curve specs            
            if (m.ContainsKey(id))
            {
                foreach (var kv in m[id])
                {
                    specs.Add(kv.Value);
                    //DLOG("Add spec " << kv.second);
                }
            }
        }
    }
}
