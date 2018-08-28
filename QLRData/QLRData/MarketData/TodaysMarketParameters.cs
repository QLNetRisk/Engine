using System;
using System.Collections.Generic;
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
        //private readonly List<Tuple<string, string>> marketObjectXMLNamesSingle = new List<Tuple<string, string>>()
        //                                            {"DiscountingCurve", "currency"}, {"YieldCurve", "name"}, {"Index", "name"}, {"SwapIndex", "name"},
        //                                            {"FxSpot", "pair"}, {"FxVolatility", "pair"}, {"SwaptionVolatility", "currency"},
        //                                            {"DefaultCurve", "name"}, {"CDSVolatility", "name"}, {"BaseCorrelation", "name"},
        //                                            {"CapFloorVolatility", "currency"}, {"ZeroInflationIndexCurve", "name"},
        //                                            {"YYInflationIndexCurve", "name"}, {"InflationCapFloorPriceSurface", "name"},
        //                                            {"EquityCurve", "name"}, {"EquityVolatility", "name"}, {"Security", "name"}};

        /// <summary>
        /// Default constructor
        /// </summary>
        public TodaysMarketParameters()
        {

        }

        //public override void FromXML(XmlNode node)
        //{
        //    // clear data members
        //    _configurations.Clear();
        //    _marketObjects.Clear();

        //    // add default configuration (may be overwritten below)
        //    MarketConfiguration defaultConfig = new MarketConfiguration();
        //    AddConfiguration(Market.DefaultConfiguration, defaultConfig);

        //    // fill data from XML
        //    CheckNode(node, "TodaysMarket");
        //    XmlNode n = GetChildNode(node);
        //    while (n)
        //    {
        //        if (XMLUtils::getNodeName(n) == "Configuration")
        //        {
        //            MarketConfiguration tmp;
        //            for (Size i = 0; i < numberOfMarketObjects; ++i)
        //            {
        //                tmp.setId(MarketObject(i), XMLUtils::getChildValue(n, marketObjectXMLNames[i] + "Id", false));
        //                addConfiguration(XMLUtils::getAttribute(n, "id"), tmp);
        //            }
        //        }
        //        else
        //        {
        //            Size i = 0;
        //            for (; i < numberOfMarketObjects; ++i)
        //            {
        //                if (XMLUtils::getNodeName(n) == marketObjectXMLNames[i])
        //                {
        //                    string id = XMLUtils::getAttribute(n, "id");
        //                    if (id == "")
        //                        id = Market::defaultConfiguration;
        //                    // The XML schema for swap indices is different ...
        //                    if (MarketObject(i) == MarketObject::SwapIndexCurve)
        //                    {
        //                        vector<XMLNode*> nodes = XMLUtils::getChildrenNodes(n, marketObjectXMLNamesSingle[i].first);
        //                        map<string, string> swapIndices;
        //                        for (XMLNode* xn : nodes)
        //                        {
        //                            string name = XMLUtils::getAttribute(xn, marketObjectXMLNamesSingle[i].second);
        //                            QL_REQUIRE(name != "", "no name given for SwapIndex");
        //                            QL_REQUIRE(swapIndices.find(name) == swapIndices.end(),
        //                                       "Duplicate SwapIndex found for " << name);
        //                            string disc = XMLUtils::getChildValue(xn, "Discounting", true);
        //                            swapIndices[name] = disc; //.emplace(name, { ibor, disc }); won't work?
        //                        }
        //                        addMarketObject(MarketObject::SwapIndexCurve, id, swapIndices);

        //                    }
        //                    else
        //                    {
        //                        auto mp = XMLUtils::getChildrenAttributesAndValues(n, marketObjectXMLNamesSingle[i].first,
        //                                                                           marketObjectXMLNamesSingle[i].second, false);
        //                        Size nc = XMLUtils::getChildrenNodes(n, "").size();
        //                        QL_REQUIRE(mp.size() == nc, "could not recognise " << (nc - mp.size()) << " sub nodes under "
        //                                                                           << marketObjectXMLNames[i]);
        //                        addMarketObject(MarketObject(i), id, mp);
        //                    }
        //                    break;
        //                }
        //            }
        //            QL_REQUIRE(i < numberOfMarketObjects,
        //                       "TodaysMarketParameters::fromXML(): node not recognized: " << XMLUtils::getNodeName(n));
        //        }
        //        n = XMLUtils::getNextSibling(n);
        //    } // while(n)
        //}

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

        public override void FromXML(XmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}
