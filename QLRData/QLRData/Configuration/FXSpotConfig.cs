using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class FXSpotConfig : CurveConfig 
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FXSpotConfig()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="curveID"></param>
        /// <param name="curveDescription"></param>
        public FXSpotConfig(string curveID, string curveDescription) : base(curveID, curveDescription)
        {
            Utils.QL_REQUIRE(curveID.Length == 6, () => "FXSpot curveID must be of the form Ccy1Ccy2");
            Currency ccy1 = Parsers.ParseCurrency(curveID.Substring(0, 3));
            Currency ccy2 = Parsers.ParseCurrency(curveID.Substring(3, 3));
            _quotes.Add("FX/RATE/" + ccy1.code + "/" + ccy2.code);
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "FXSpot");
            _curveID = GetChildValue(node, "CurveId", true);
            Utils.QL_REQUIRE(_curveID.Length == 6, () => "FXSpot curveID must be of the form Ccy1Ccy2");
            Currency ccy1 = Parsers.ParseCurrency(_curveID.Substring(0, 3));
            Currency ccy2 = Parsers.ParseCurrency(_curveID.Substring(3, 3));
            _quotes.Add("FX/RATE/" + ccy1.code + "/" + ccy2.code);

            _curveDescription = GetChildValue(node, "CurveDescription", true);
        }
    }
}
