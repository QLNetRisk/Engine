using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

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
            FromXML(GetFirstNode(doc, ""));
            //FromXML(doc); // doc or doc.GetFirstNode() ?
        }

        public virtual void ToFile(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            ToXML(doc);            
            doc.Save(fileName);            
        }

        public XmlNode GetFirstNode(XmlDocument doc, string name)
        {
            List<XmlNode> children;

            if (name.Length > 0) children = GetChildrenNodes(doc, name).Where(c => c.NodeType != XmlNodeType.Comment && c.NodeType != XmlNodeType.XmlDeclaration).ToList();            
            else children = GetChildrenNodes(doc, "").Where(c => c.NodeType != XmlNodeType.Comment && c.NodeType != XmlNodeType.XmlDeclaration).ToList();

            return children.FirstOrDefault();                        
        }

        public XmlNode GetChildNode(XmlNode node, string name)
        {
            // TODO!
            //foreach (XmlNode child in node.ChildNodes)
            //{
            //    if (child.NodeType == XmlNodeType.Comment) continue;
            //    if (child.Name == name) return child;
            //}
            XmlNode child;
            if (name.Length > 0) child = GetChildrenNodes(node, name).Where(c => c.NodeType != XmlNodeType.Comment && c.NodeType != XmlNodeType.XmlDeclaration).FirstOrDefault();
            else child = GetChildrenNodes(node, "").Where(c => c.NodeType != XmlNodeType.Comment && c.NodeType != XmlNodeType.XmlDeclaration).FirstOrDefault();

            return child;
        }

        public string GetNodeName(XmlNode node)
        {
            return node.Name;            
        }

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

        public virtual List<XmlNode> GetChildrenNodes(XmlNode node, string name)
        {
            Utils.QL_REQUIRE(node != null, () => "XMLUtils.GetAttribute() node is NULL");
            List<XmlNode> res = new List<XmlNode>();

            //string p = name.Length == 0 ? null : name;                        

            if (name.Length == 0)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if(child.NodeType != XmlNodeType.Comment && child.NodeType != XmlNodeType.XmlDeclaration) res.Add(child);
                }                
            }
            else
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if(child.NodeType != XmlNodeType.Comment && child.NodeType != XmlNodeType.XmlDeclaration && child.Name == name) res.Add(child);
                }
            }                        

            return res;          
        }

        public virtual XmlNode GetNextSibling(XmlNode node)
        {
            if (node.NextSibling == null) return null;

            if(node.NextSibling.NodeType == XmlNodeType.Comment || node.NextSibling.NodeType == XmlNodeType.XmlDeclaration)
            {
                return GetNextSibling(node.NextSibling);
            }
            else
            {
                return node.NextSibling;
            }
            // TODO: name redundant?
        }

        public virtual string GetAttribute(XmlNode node, string attrName)
        {
            Utils.QL_REQUIRE(node != null, () => "GetAttribute() node is NULL");
            XmlNode attr = node.Attributes.GetNamedItem(attrName);

            if (attr != null && attr.Value != null) return attr.Value;
            else return "";              
        }

        public virtual Dictionary<string, string> GetChildrenAttributesAndValues(XmlNode parent, string names, string attributeName, bool mandatory)                                                             
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach(XmlNode child in GetChildrenNodes(parent, names))                 
            {
                string first = GetAttribute(child, attributeName);
                string second = GetChildValue(parent, child.Name, true); // child.Value;
                if (mandatory)
                {
                    Utils.QL_REQUIRE(first != "", () => "empty attribute for " + names);
                }
                res.Add(first, second);
            }
            if (mandatory)
            {
                Utils.QL_REQUIRE(res.Count > 0, () => "Error: No XML Node " + names + " found.");
            }
            return res;
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
