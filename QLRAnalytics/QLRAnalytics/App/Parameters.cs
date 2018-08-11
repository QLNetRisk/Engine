using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLRData;

namespace QLRAnalytics
{
    public class Parameters : XmlSerializable
    {
        private Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public Parameters()
        {

        }

        public void Clear()
        {
            _data.Clear();
        }

        public void Log()
        {

        }

        public bool HasGroup(string groupName)
        {
            return _data.ContainsKey(groupName);
        }

        public bool Has(string groupName, string paramName)
        {
            QLNet.Utils.QL_REQUIRE(HasGroup(groupName), () => "param group '" + groupName + "' not found");
            
            return _data[groupName].ContainsKey(paramName);
        }

        public string Get(string groupName, string paramName)
        {
            QLNet.Utils.QL_REQUIRE(Has(groupName, paramName), () => "parameter " + paramName + " not found in param group " + groupName);

            return _data[groupName][paramName];
        }

        public override void FromFile(string fileName)
        {
            //LOG("load ORE configuration from " << fileName);
            Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            FromXML(doc.SelectNodes("ORE").Item(0));
            //LOG("load ORE configuration from " << fileName << " done.")
        }

        public override void FromXML(XmlNode node)
        {
            ParseXMLNode(node, "Setup", "name");
            ParseXMLNode(node, "Markets", "name");
            ParseGroupedXMLNode(node, "Analytics", "type");
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
        }

        protected void ParseXMLNode(XmlNode node, string nodeName, string attribute)
        {
            XmlNode setupNode = node.SelectSingleNode(nodeName);
            QLNet.Utils.QL_REQUIRE(setupNode != null, () => string.Format("node {0} not found in parameter file", nodeName));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (XmlNode child in setupNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Comment) continue;
                var key = child.Attributes[attribute].Value;
                var value = child.HasChildNodes ? child.LastChild.Value : string.Empty;
                dict.Add(key, value);
            }

            _data[nodeName.ToLower()] = dict;
        }

        protected void ParseGroupedXMLNode(XmlNode node, string nodeName, string attribute)
        {
            XmlNode setupNode = node.SelectSingleNode(nodeName);
            QLNet.Utils.QL_REQUIRE(setupNode != null, () => string.Format("node {0} not found in parameter file", nodeName));
            foreach (XmlNode child in setupNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Comment) continue;
                string groupName = child.Attributes[attribute].Value;
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (XmlNode grandChild in child.ChildNodes)
                {
                    if (grandChild.NodeType == XmlNodeType.Comment) continue;
                    var key = grandChild.Attributes["name"].Value;
                    var value = grandChild.HasChildNodes ? grandChild.LastChild.Value : string.Empty;
                    dict.Add(key, value);
                }

                _data[groupName] = dict;
            }
        }
    }
}
