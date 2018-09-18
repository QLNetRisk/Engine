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
        private Dictionary<string, YieldCurveConfig> _yieldCurveConfigs;
        //private Dictionary<string, FXVolatilityCurveConfig>> fxVolCurveConfigs_;
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

        public HashSet<string> Quotes()
        {
            List<string> quotes = new List<string>();
            foreach(KeyValuePair<string, YieldCurveConfig> kvp in _yieldCurveConfigs)
                quotes.AddRange(kvp.Value.Quotes());
            //for (auto m : fxVolCurveConfigs_)
            //    quotes.insert(quotes.end(), m.second->quotes().begin(), m.second->quotes().end());
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
            throw new NotImplementedException();
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}
