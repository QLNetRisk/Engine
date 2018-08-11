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
            XmlNode conventionsNode = node.SelectSingleNode("Conventions");
            QLNet.Utils.QL_REQUIRE(conventionsNode != null, () => string.Format("node {0} not found in conventions file", "Conventions"));
            
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
                    //convention = new DepositConvention();
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
                //else if (childName == "Swap")
                //{
                //    convention.reset(new IRSwapConvention());
                //}
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
                //else if (childName == "FX")
                //{
                //    convention.reset(new FXConvention());
                //}
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
                    QLNet.Utils.QL_FAIL("Convention name, " + childName + ", not recognized.");
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

    public class ZeroRateConvention : Convention
    {
        private DayCounter _dayCounter;
        private Calendar _tenorCalendar;
        private Compounding _compounding;
        private Frequency _compoundingFrequency;
        private int _spotLag;
        private Calendar _spotCalendar;
        private BusinessDayConvention _rollConvention;
        private bool _eom;
        private bool _tenorBased;

        private string _strTenorCalendar;
        private string _strDayCounter;
        private string _strCompounding;
        private string _strCompoundingFrequency;
        private string _strSpotLag;
        private string _strSpotCalendar;
        private string _strRollConvention;
        private string _strEom;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ZeroRateConvention()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dayCounter"></param>
        /// <param name="compounding"></param>
        /// <param name="compoundingFrequency"></param>
        public ZeroRateConvention(string id, string dayCounter, string compounding, string compoundingFrequency)                       
        {
            _id = id;
            _strDayCounter = dayCounter;
            _strCompounding = compounding;
            _strCompoundingFrequency = compoundingFrequency;

            Build();
        }

        public ZeroRateConvention(string id, string dayCounter, string tenorCalendar, string compounding = "Continuous", string compoundingFrequency = "Annual",
            string spotLag = "", string spotCalendar = "", string rollConvention = "", string eom = "")
        {
            _id = id;
            _strTenorCalendar = tenorCalendar;
            _strDayCounter = dayCounter;
            _strCompounding = compounding;
            _strCompoundingFrequency = compoundingFrequency;
            _strSpotLag = spotLag;
            _strSpotCalendar = spotCalendar;
            _strRollConvention = rollConvention;
            _strEom = eom;

            Build();
        }

        public override void FromXML(XmlNode node)
        {
            //XmlNode conventionsNode = node.SelectSingleNode("Zero");
            QLNet.Utils.QL_REQUIRE(node != null, () => string.Format("node {0} not found in conventions file", "Conventions"));
            
            _type = Type.Zero;

            _id = GetChildValue(node, "Id", true); 
            _tenorBased = Parsers.ParseBool(GetChildValue(node, "TenorBased", true)); 

            // Get string values from xml
            _strDayCounter = GetChildValue(node, "DayCounter", true); 
            _strCompoundingFrequency = GetChildValue(node, "CompoundingFrequency", false); 
            _strCompounding = GetChildValue(node, "Compounding", false); 
            if (_tenorBased)
            {
                _strTenorCalendar = GetChildValue(node, "TenorCalendar", true); 
                _strSpotLag = GetChildValue(node, "SpotLag", false); 
                _strSpotCalendar = GetChildValue(node, "SpotCalendar", false); 
                _strRollConvention = GetChildValue(node, "RollConvention", false); 
                _strEom = GetChildValue(node, "EOM", false); 
            }

            Build();
        }

        public virtual void Build()
        {
            _dayCounter = Parsers.ParseDayCounter(_strDayCounter);            
            _compounding = Parsers.ParseCompounding(_strCompounding);
            _compoundingFrequency = Parsers.ParseFrequency(_strCompoundingFrequency);

            if(_tenorBased)
            {
                _tenorCalendar = Parsers.ParseCalendar(_strTenorCalendar);
                _spotLag = Convert.ToInt32(_strSpotLag);
                _spotCalendar = Parsers.ParseCalendar(_strSpotCalendar);
                _rollConvention = Parsers.ParseBusinessDayConvention(_strRollConvention);
                _eom = Parsers.ParseBool(_strEom);
            }
        }

        /// <summary>
        /// Zero rate day counter
        /// </summary>
        /// <returns></returns>
        public DayCounter DayCounter()
        {
            return _dayCounter;
        }
        /// <summary>
        /// Return the calendar used for converting tenor points into dates
        /// </summary>
        /// <returns></returns>
        public Calendar TenorCalendar()
        {
            return _tenorCalendar;
        }
        /// <summary>
        /// Zero rate compounding
        /// </summary>
        /// <returns></returns>
        public Compounding Compounding()
        {
            return _compounding;
        }
        /// <summary>
        /// Zero rate compounding frequency
        /// </summary>
        /// <returns></returns>
        public Frequency CompoundingFrequency()
        {
            return _compoundingFrequency;
        }
        /// <summary>
        /// Zero rate spot lag
        /// </summary>
        /// <returns></returns>
        public int SpotLag()
        {
            return _spotLag;
        }
        /// <summary>
        /// Calendar used for spot date adjustment
        /// </summary>
        /// <returns></returns>
        public Calendar SpotCalendar()
        {
            return _spotCalendar;
        }
        /// <summary>
        /// Business day convention used in converting tenor points into dates
        /// </summary>
        /// <returns></returns>
        public BusinessDayConvention RollConvention()
        {
            return _rollConvention;
        }
        /// <summary>
        /// End of month adjustment
        /// </summary>
        /// <returns></returns>
        public bool Eom()
        { 
            return _eom; 
        }
        /// <summary>
        /// Flag to indicate whether the Zero Rate convention is based on a tenor input
        /// </summary>
        /// <returns></returns>
        public bool TenorBased()
        {
            return _tenorBased;
        }
    }
}
