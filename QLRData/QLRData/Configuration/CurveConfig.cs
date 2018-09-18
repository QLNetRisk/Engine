using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QLRData
{
    public class CurveConfig : XmlSerializable
    {
        protected string _curveID;
        protected string _curveDescription;
        protected List<string> _quotes;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CurveConfig()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="curveID"></param>
        /// <param name="curveDescription"></param>
        /// <param name="quotes"></param>
        public CurveConfig(string curveID, string curveDescription, List<string> quotes = null)        
        {
            _curveID = curveID;
            _curveDescription = curveDescription;
            _quotes = quotes;
        }

        public string CurveID()
        {
            return _curveID;
        }
        public string CurveDescription()
        {
            return _curveDescription;
        }
        /// <summary>
        /// Return all the market quotes required for this config
        /// </summary>
        /// <returns></returns>
        public virtual List<string> Quotes()
        {
            return _quotes;
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
