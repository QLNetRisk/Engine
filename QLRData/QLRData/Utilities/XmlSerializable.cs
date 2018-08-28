using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QLRData
{
    public abstract class XmlSerializable
    {
        public abstract void FromXML(XmlNode node);
        public abstract void ToXML(XmlDocument doc);

        public virtual void FromFile(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            FromXML(doc);
        }

        public virtual void ToFile(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            ToXML(doc);            
            doc.Save(fileName);            
        }

        //public XmlNode GetChildNode(XmlNode node, string name)
        //{
        //    XmlNode conventionsNode = node.SelectSingleNode("Conventions");

        //    foreach (XmlNode child in conventionsNode.ChildNodes)
        //    {
        //        if (child.NodeType == XmlNodeType.Comment) continue;
        //    }
        //}

        public virtual string GetChildValue(XmlNode node, string name, bool mandatory = false)
        {
            try
            {
                string value = node.SelectSingleNode(name).FirstChild.Value;
                return value;
            }
            catch(Exception ex)
            {
                if (mandatory) throw ex;
                else return string.Empty;
            }            
        }

        public virtual double GetChildValueAsDouble(XmlNode node, string name, bool mandatory = false)
        {
            string s = GetChildValue(node, name, mandatory);
            return s == "" ? 0.0 : Parsers.ParseDouble(s);
        }

        public virtual int GetChildValueAsInt(XmlNode node, string name, bool mandatory = false)
        {
            string s = GetChildValue(node, name, mandatory);
            return s == "" ? 0 : Parsers.ParseInteger(s);
        }

        public virtual bool GetChildValueAsBool(XmlNode node, string name, bool mandatory = false)
        {
            string s = GetChildValue(node, name, mandatory);
            return s == "" ? true : Parsers.ParseBool(s); 
        }

        public void CheckNode(XmlNode node, string expectedName)
        {
            QLNet.Utils.QL_REQUIRE(node != null, () => "XML node is NULL (expected " + expectedName + ")");
            QLNet.Utils.QL_REQUIRE(node.Name == expectedName, () => "XML node name " + node.Name + " does not match exptected name " + expectedName);
        }
    }
}
