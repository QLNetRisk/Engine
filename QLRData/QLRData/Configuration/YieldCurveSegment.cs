using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QLRData
{
    public class YieldCurveSegment : XmlSerializable
    {
        // Supported segment types
        public enum Type
        {
            Zero,
            ZeroSpread,
            Discount,
            Deposit,
            FRA,
            Future,
            OIS,
            Swap,
            AverageOIS,
            TenorBasis,
            TenorBasisTwo,
            FXForward,
            CrossCcyBasis
        };

        protected List<string> _quotes;

        // TODO: why type and typeID?
        private Type _type;
        private string _typeID;
        private string _conventionsID;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected YieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        protected YieldCurveSegment(string typeID, string conventionsID, List<string> quotes)
        {

        }  

        public override void FromXML(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
        }

        public Type type()
        {
            return _type;
        }
        public string TypeID()
        {
            return _typeID;
        }   
        public string ConventionsID()
        {
            return _conventionsID;
        }        
        public virtual List<string> Quotes()
        {
            return _quotes;
        }            
    }
}

