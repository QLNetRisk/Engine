using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QLRData
{
    public class CurveConfigurations : XmlSerializable
    {
        private Dictionary<string, YieldCurveConfig> _yieldCurveConfigs = new Dictionary<string, YieldCurveConfig>();
        private Dictionary<string, FXVolatilityCurveConfig> _fxVolCurveConfigs = new Dictionary<string, FXVolatilityCurveConfig>();
        //private Dictionary<string, SwaptionVolatilityCurveConfig>> swaptionVolCurveConfigs_;
        //private Dictionary<string, CapFloorVolatilityCurveConfig>> capFloorVolCurveConfigs_;
        //private Dictionary<string, DefaultCurveConfig>> defaultCurveConfigs_;
        //private Dictionary<string, CDSVolatilityCurveConfig>> cdsVolCurveConfigs_;
        //private Dictionary<string, BaseCorrelationCurveConfig>> baseCorrelationCurveConfigs_;
        //private Dictionary<string, InflationCurveConfig>> inflationCurveConfigs_;
        //private Dictionary<string, InflationCapFloorPriceSurfaceConfig>> inflationCapFloorPriceSurfaceConfigs_;
        //private Dictionary<string, EquityCurveConfig>> equityCurveConfigs_;
        //private Dictionary<string, EquityVolatilityCurveConfig>> equityVolCurveConfigs_;
        //private Dictionary<string, SecurityConfig>> securityConfigs_;
        //private Dictionary<string, FXSpotConfig>> fxSpotConfigs_;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CurveConfigurations()
        {

        }

        public YieldCurveConfig YieldCurveConfig(string curveID)
        {
            return _yieldCurveConfigs[curveID];
        }

        public FXVolatilityCurveConfig FxVolCurveConfig(string curveID)
        {
            return _fxVolCurveConfigs[curveID];
        }

        public HashSet<string> Quotes()
        {
            List<string> quotes = new List<string>();
            foreach(KeyValuePair<string, YieldCurveConfig> kvp in _yieldCurveConfigs)
                quotes.AddRange(kvp.Value.Quotes());
            foreach (KeyValuePair<string, FXVolatilityCurveConfig> kvp in _fxVolCurveConfigs)
                quotes.AddRange(kvp.Value.Quotes());            
            //for (auto m : swaptionVolCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : capFloorVolCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : defaultCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : cdsVolCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : baseCorrelationCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : inflationCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : inflationCapFloorPriceSurfaceConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : equityCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : equityVolCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : securityConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
            //for (auto m : fxSpotConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());

            return new HashSet<string>(quotes);
        }               

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "CurveConfiguration");

            // Load YieldCurves, FXVols, etc, etc
            ParseNode(node, "YieldCurves", "YieldCurve", _yieldCurveConfigs);
            ParseNode(node, "FXVolatilities", "FXVolatility", _fxVolCurveConfigs);
            //parseNode(node, "SwaptionVolatilities", "SwaptionVolatility", swaptionVolCurveConfigs_);
            //parseNode(node, "CapFloorVolatilities", "CapFloorVolatility", capFloorVolCurveConfigs_);
            //parseNode(node, "DefaultCurves", "DefaultCurve", defaultCurveConfigs_);
            //parseNode(node, "CDSVolatilities", "CDSVolatility", cdsVolCurveConfigs_);
            //parseNode(node, "BaseCorrelations", "BaseCorrelation", baseCorrelationCurveConfigs_);
            //parseNode(node, "EquityCurves", "EquityCurve", equityCurveConfigs_);
            //parseNode(node, "EquityVolatilities", "EquityVolatility", equityVolCurveConfigs_);
            //parseNode(node, "InflationCurves", "InflationCurve", inflationCurveConfigs_);
            //parseNode(node, "InflationCapFloorPriceSurfaces", "InflationCapFloorPriceSurface",
            //          inflationCapFloorPriceSurfaceConfigs_);
            //parseNode(node, "Securities", "Security", securityConfigs_);
            //parseNode(node, "FXSpots", "FXSpot", fxSpotConfigs_);
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
        }

        private void ParseNode<T>(XmlNode node, string parentName, string childName, Dictionary<string, T> m) where T : CurveConfig
        {
            XmlNode parentNode = GetChildNode(node, parentName);
            if (parentNode != null)
            {
                XmlNode child = GetChildNode(parentNode, childName);

                //foreach (XmlNode sibling in GetNextSibling(child, childName))
                while(child != null)
                {
                    T curveConfig = (T)Activator.CreateInstance(typeof(T));
                    try
                    {
                        curveConfig.FromXML(child);
                        string id = curveConfig.CurveID();
                        m.Add(id, curveConfig);
                        //DLOG("Added curve config with ID = " << id);

                        child = GetNextSibling(child, childName);
                    }
                    catch (Exception ex)
                    {
                        var debug = "";
                        //ALOG("Exception parsing curve config: " << ex.what());
                    }
                }
            }
        }
    }
}
