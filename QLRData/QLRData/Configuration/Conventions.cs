using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class Conventions : XmlSerializable
    {
        private Dictionary<string, Convention> _data = new Dictionary<string, Convention>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Conventions()
        {

        }

        public void Clear()
        {
            _data.Clear();
        }

        public Convention Get(string id)
        {
            QLNet.Utils.QL_REQUIRE(_data.ContainsKey(id), () => "Cannot find conventions for id " + id);

            if (_data.ContainsKey(id)) return _data[id];
            else return null;            
        }

        public void Add(Convention convention)
        {
            string id = convention.Id();
            QLNet.Utils.QL_REQUIRE(!_data.ContainsKey(id), () => "Convention already exists for id " + id);
            _data[id] = convention;
        }        

        public override void FromXML(XmlNode node)
        {            
            // TODO: Implement correctly
            XmlNode conventionsNode = node.SelectSingleNode("Conventions");
            CheckNode(conventionsNode, "Conventions");

            foreach (XmlNode child in conventionsNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Comment) continue;

                Convention convention = new Convention();
                string childName = child.Name;

                if (childName == "Zero")
                {
                    convention = new ZeroRateConvention();
                }
                else if (childName == "Deposit")
                {
                    convention = new DepositConvention();
                }
                //else if (childName == "Future")
                //{
                //    convention.reset(new FutureConvention());
                //}
                //else if (childName == "FRA")
                //{
                //    convention.reset(new FraConvention());
                //}
                //else if (childName == "OIS")
                //{
                //    convention.reset(new OisConvention());
                //}
                else if (childName == "Swap")
                {
                    convention = new IRSwapConvention();
                }
                //else if (childName == "AverageOIS")
                //{
                //    convention.reset(new AverageOisConvention());
                //}
                //else if (childName == "TenorBasisSwap")
                //{
                //    convention.reset(new TenorBasisSwapConvention());
                //}
                //else if (childName == "TenorBasisTwoSwap")
                //{
                //    convention.reset(new TenorBasisTwoSwapConvention());
                //}
                else if (childName == "FX")
                {
                    convention = new FXConvention();
                }
                //else if (childName == "CrossCurrencyBasis")
                //{
                //    convention.reset(new CrossCcyBasisSwapConvention());
                //}
                //else if (childName == "CDS")
                //{
                //    convention.reset(new CdsConvention());
                //}
                //else if (childName == "SwapIndex")
                //{
                //    convention.reset(new SwapIndexConvention());
                //}
                //else if (childName == "InflationSwap")
                //{
                //    convention.reset(new InflationSwapConvention());
                //}
                else
                {
                    // Temporary before having implemented all conventions
                    continue;
                    //QLNet.Utils.QL_FAIL("Convention name, " + childName + ", not recognized.");
                }
                
                string id = child.SelectSingleNode("Id").FirstChild.Value;

                try
                {
                    //DLOG("Loading Convention " << id);
                    convention.FromXML(child);
                    Add(convention);
                }
                catch (Exception ex)
                {
                    QLNet.Utils.QL_FAIL("Exception parsing convention XML Node (id = " + id + ") : " + ex.ToString());
                    //WLOG("Exception parsing convention " "XML Node (id = " << id << ") : " << e.what());
                }
            }
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
