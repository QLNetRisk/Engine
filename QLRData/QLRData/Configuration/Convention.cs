using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class Convention : XmlSerializable
    {
        public enum Type
        {
            Zero,
            Deposit,
            Future,
            FRA,
            OIS,
            Swap,
            AverageOIS,
            TenorBasisSwap,
            TenorBasisTwoSwap,
            FX,
            CrossCcyBasis,
            CDS,
            SwapIndex,
            InflationSwap,
            SecuritySpread
        };

        protected Type _type;
        protected string _id;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Convention()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public Convention(string id, Type type)
        {
            _id = id;
            _type = type;
        }

        public string Id()
        {
            return _id;
        }
        public Type ConventionType()
        {
            return _type;
        }

        public override void FromFile(string fileName)
        {
            base.FromFile(fileName);
        }

        public override void FromXML(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public override void ToFile(string fileName)
        {
            base.ToFile(fileName);
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}
