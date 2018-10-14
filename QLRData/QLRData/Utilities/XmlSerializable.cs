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

        public virtual string GetNodeValue(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Text) return node.Value;
            else return node.Name;
        }

        public virtual string GetChildValue(XmlNode node, string name, bool mandatory = false)
        {
            try
            {
                foreach(XmlNode child in node.ChildNodes)
                {
                    if(child.NodeType == XmlNodeType.Text)
                    {
                        if (node.Name == name) return child.Value;                        
                    }
                    else
                    {
                        if (child.Name == name && child.FirstChild != null) return child.FirstChild.Value ?? "";
                    }
                    
                    //if (child.Name == name && child.NodeType == XmlNodeType.Element && child.FirstChild != null) return child.FirstChild.Value ?? "";
                    //if (child.Name == name && child.NodeType == XmlNodeType.Text) return child.Value ?? "";
                }
                //string value = node.ChildNodes SelectSingleNode(name).FirstChild.Value;
                //return value;
                return "";
            }
            catch(Exception ex)
            {
                if (mandatory) throw ex;
                else return string.Empty;
            }            
        }

        public virtual List<string> GetChildrenValues(XmlNode parent, string names, string name, bool mandatory = false)
        {
            List<string> values = new List<string>();
            XmlNode node = GetChildNode(parent, names);

            if (mandatory)
            {
                Utils.QL_REQUIRE(node != null, () => "Error: No XML Node " + names + " found.");
            }
            if (node != null)
            {
                XmlNode child = GetChildNode(node, name);
                //values.Add(GetChildValue(node, name));
                while (child != null)
                {
                    values.Add(GetChildValue(child, name, true));
                    child = GetNextSibling(child, name);
                }               
            }            

            return values;
        }

        public List<Period> GetChildrenValuesAsPeriods(XmlNode node, string name, bool mandatory)
        {
            string s = GetChildValue(node, name, mandatory);
            return Parsers.ParseListOfValues<Period>(s, Parsers.ParsePeriod);
        }

        public List<double> GetChildrenValuesAsDoublesCompact(XmlNode node, string name, bool mandatory)
        {
            string s = GetChildValue(node, name, mandatory);
            return Parsers.ParseListOfValues<double>(s, Parsers.ParseDouble);
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

        public virtual XmlNode GetNextSibling(XmlNode node, string name)
        {
            if (node.NextSibling == null) return null;

            if(node.NextSibling.NodeType == XmlNodeType.Comment || node.NextSibling.NodeType == XmlNodeType.XmlDeclaration)
            {
                return GetNextSibling(node.NextSibling, name);
            }
            else if(name.Length > 0)
            {
                if (name == node.NextSibling.Name) return node.NextSibling;
                else return GetNextSibling(node.NextSibling, name);
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

        //public virtual List<Period> GetChildrenValuesAsPeriods(XmlNode node, string name, bool mandatory)
        //{
        //    string s = GetChildValue(node, name, mandatory);
        //    return Parsers.ParseListOfValues(s, Parsers.ParsePeriod);
        //}

        public void CheckNode(XmlNode node, string expectedName)
        {
            QLNet.Utils.QL_REQUIRE(node != null, () => "XML node is NULL (expected " + expectedName + ")");
            QLNet.Utils.QL_REQUIRE(node.Name == expectedName, () => "XML node name " + node.Name + " does not match exptected name " + expectedName);
        }
    }
}
