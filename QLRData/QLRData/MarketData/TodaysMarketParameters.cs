using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class TodaysMarketParameters : XmlSerializable
    {


        private Dictionary<string, MarketConfiguration> _configurations = new Dictionary<string, MarketConfiguration>();
        private Dictionary<MarketObject, Dictionary<string, Dictionary<string, string>>> _marketObjects = new Dictionary<MarketObject, Dictionary<string, Dictionary<string, string>>>();

        private const int numberOfMarketObjects = 17;

        // clang-format off
        private readonly List<string> marketObjectStrings = new List<string>(){"DiscountCurve", "YieldCurve", "IndexCurve", "SwapIndexCurve",
                                                   "FXSpot", "FXVol", "SwaptionVol", "DefaultCurve", "CDSVol",
                                                   "BaseCorrelation", "CapFloorVol", "ZeroInflationCurve",
                                                   "YoYInflationCurve", "InflationCapFloorPriceSurface",
                                                   "EquityCurves", "EquityVols", "Securities"};
        private readonly List<string> marketObjectXMLNames = new List<string>(){"DiscountingCurves", "YieldCurves", "IndexForwardingCurves",
                                                    "SwapIndexCurves",
                                                    "FxSpots", "FxVolatilities", "SwaptionVolatilities",
                                                    "DefaultCurves", "CDSVolatilities", "BaseCorrelations",
                                                    "CapFloorVolatilities",
                                                    "ZeroInflationIndexCurves", "YYInflationIndexCurves",
                                                    "InflationCapFloorPriceSurfaces",
                                                    "EquityCurves", "EquityVolatilities",
                                                    "Securities"};
        private readonly OrderedDictionary marketObjectXMLNamesSingle = new OrderedDictionary(){
                                                    { "DiscountingCurve", "currency" },
                                                    {"YieldCurve", "name"},
                                                    {"Index", "name"}, {"SwapIndex", "name"},
                                                    {"FxSpot", "pair"}, {"FxVolatility", "pair"}, {"SwaptionVolatility", "currency"},
                                                    {"DefaultCurve", "name"}, {"CDSVolatility", "name"}, {"BaseCorrelation", "name"},
                                                    {"CapFloorVolatility", "currency"}, {"ZeroInflationIndexCurve", "name"},
                                                    {"YYInflationIndexCurve", "name"}, {"InflationCapFloorPriceSurface", "name"},
                                                    {"EquityCurve", "name"}, {"EquityVolatility", "name"}, {"Security", "name"}};

        /// <summary>
        /// Default constructor
        /// </summary>
        public TodaysMarketParameters()
        {

        }

        public override void FromXML(XmlNode node)
        {            
            // TODO: Implement differently?
            String[] keys = new String[marketObjectXMLNamesSingle.Keys.Count];
            String[] values = new String[marketObjectXMLNamesSingle.Values.Count];
            marketObjectXMLNamesSingle.Keys.CopyTo(keys, 0);
            marketObjectXMLNamesSingle.Values.CopyTo(values, 0);

            // clear data members
            _configurations.Clear();
            _marketObjects.Clear();

            // add default configuration (may be overwritten below)
            MarketConfiguration defaultConfig = new MarketConfiguration();
            AddConfiguration(Market.DefaultConfiguration, defaultConfig);

            // fill data from XML
            CheckNode(node, "TodaysMarket");
            XmlNode n = GetChildNode(node, "");
            while (n != null)
            {
                if (GetNodeName(n) == "Configuration")
                {
                    MarketConfiguration tmp = new MarketConfiguration();
                    for (int i = 0; i < numberOfMarketObjects; ++i)
                    {
                        tmp.SetId((MarketObject)i, GetChildValue(n, marketObjectXMLNames[i] + "Id", false));
                        AddConfiguration(GetAttribute(n, "id"), tmp);
                    }
                }
                else
                {
                    int i = 0;
                    for (; i < numberOfMarketObjects; ++i)
                    {
                        if (GetNodeName(n) == marketObjectXMLNames[i])
                        {
                            string id = GetAttribute(n, "id");
                            if (id == "")
                                id = Market.DefaultConfiguration;
                            // The XML schema for swap indices is different ...
                            if ((MarketObject)i == MarketObject.SwapIndexCurve)
                            {
                                List<XmlNode> nodes = GetChildrenNodes(n, keys[i]);
                                Dictionary<string, string> swapIndices = new Dictionary<string, string>();
                                foreach (XmlNode xn in nodes)
                                {
                                    string name = GetAttribute(xn, values[i]);
                                    Utils.QL_REQUIRE(name != "", () => "no name given for SwapIndex");
                                    Utils.QL_REQUIRE(!swapIndices.ContainsKey(name), () => "Duplicate SwapIndex found for " + name);

                                    string disc = GetChildValue(xn, "Discounting", true);
                                    swapIndices[name] = disc; //.emplace(name, { ibor, disc }); won't work?
                                }
                                AddMarketObject(MarketObject.SwapIndexCurve, id, swapIndices);

                            }
                            else
                            {
                               
                                try
                                {
                                    var mp = GetChildrenAttributesAndValues(n, keys[i], values[i], false);

                                    int nc = GetChildrenNodes(n, "").Count;
                                    Utils.QL_REQUIRE(mp.Count == nc, () => "could not recognise " + (nc - mp.Count) + " sub nodes under " + marketObjectXMLNames[i]);

                                    AddMarketObject((MarketObject)i, id, mp);
                                }
                                catch(Exception ex)
                                {
                                    var debug = "";
                                }                                                                
                            }
                            break;
                        }
                    }
                    Utils.QL_REQUIRE(i < numberOfMarketObjects, () => "TodaysMarketParameters.FromXML(): node not recognized: " + GetNodeName(n));

                }
                try
                {
                    n = GetNextSibling(n, "");
                }
                catch(Exception ex)
                {
                    var debug = "";
                }
            }
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
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

        public void AddConfiguration(string id, MarketConfiguration configuration)
        {
            _configurations[id] = configuration;
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

        private void AddMarketObject(MarketObject o, string id, Dictionary<string, string> assignments) 
        {            
            if(!_marketObjects.ContainsKey(o))
            {
                Dictionary<string, Dictionary<string, string>> temp = new Dictionary<string, Dictionary<string, string>>();
                temp.Add(id, assignments);
                _marketObjects.Add(o, temp);
            }
            else
            {
                if(!_marketObjects[o].ContainsKey(id))
                {
                    _marketObjects[o].Add(id, assignments);
                }
            }
            //_marketObjects[o][id] = assignments;
            foreach (KeyValuePair<string, string> s in assignments)
            {
                //DLOG("TodaysMarketParameters, add market objects of type " + o + ": " + id + " " + s.Key + " " + s.Value);
            }
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
