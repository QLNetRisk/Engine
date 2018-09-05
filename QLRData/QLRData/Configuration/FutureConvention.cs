using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class FutureConvention : Convention
    {
        private string _strIndex;
        private IborIndex _index;                

        /// <summary>
        /// Default constructor
        /// </summary>
        public FutureConvention()
        {

        }

        /// <summary>
        /// Index based constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        public FutureConvention(string id, string index) : base(id, Type.Future)
        {
            _strIndex = index;
            _index = Parsers.ParseIborIndex(index);
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "Future");
            _type = Type.Future;
            _id = GetChildValue(node, "Id", true);
            _strIndex = GetChildValue(node, "Index", true);
            _index = Parsers.ParseIborIndex(_strIndex);
        }

        public virtual void Build()
        {
            
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IborIndex Index()
        {
            return _index;
        }
        /// <summary>
        /// Index name
        /// </summary>
        /// <returns></returns>
        public string IndexName()
        {
            return _strIndex;
        }
    }
}
