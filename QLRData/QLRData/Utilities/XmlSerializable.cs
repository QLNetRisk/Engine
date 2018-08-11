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
    }
}
