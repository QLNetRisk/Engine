using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class OisConvention : Convention
    {
        private int _spotLag;
        private OvernightIndex _index;
        private DayCounter _fixedDayCounter;
        private int _paymentLag;
        private bool _eom;
        private Frequency _fixedFrequency;
        private BusinessDayConvention _fixedConvention;
        private BusinessDayConvention _fixedPaymentConvention;
        //private DateGeneration::Rule _rule;

        // Strings to store the inputs
        private string _strSpotLag;
        private string _strIndex;
        private string _strFixedDayCounter;
        private string _strPaymentLag;
        private string _strEom;
        private string _strFixedFrequency;
        private string _strFixedConvention;
        private string _strFixedPaymentConvention;
        private string _strRule;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OisConvention()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="spotLag"></param>
        /// <param name="index"></param>
        /// <param name="fixedDayCounter"></param>
        /// <param name="paymentLag"></param>
        /// <param name="eom"></param>
        /// <param name="fixedFrequency"></param>
        /// <param name="fixedConvention"></param>
        /// <param name="fixedPaymentConvention"></param>
        /// <param name="rule"></param>
        public OisConvention(string id, string spotLag, string index, string fixedDayCounter, string paymentLag = "", string eom = "", string fixedFrequency = "", string fixedConvention = "", string fixedPaymentConvention = "", string rule = "")
            : base(id, Type.OIS)
        {
            _strSpotLag = spotLag;
            _strIndex = index;
            _strFixedDayCounter = fixedDayCounter;
            _strPaymentLag = paymentLag;
            _strEom = eom;
            _strFixedFrequency = fixedFrequency;
            _strFixedConvention = fixedConvention;
            _strFixedPaymentConvention = fixedPaymentConvention;
            _strRule = rule;

            Build();
        }       

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "OIS");            
            
        }

        public virtual void Build()
        {
            // First check that we have an overnight index.
            _index = (OvernightIndex) Parsers.ParseIborIndex(_strIndex);
            Utils.QL_REQUIRE(_index != null, () => "The index string, " + _strIndex + ", does not represent an overnight index.");

            _spotLag = Parsers.ParseInteger(_strSpotLag);
            _fixedDayCounter = Parsers.ParseDayCounter(_strFixedDayCounter);
            _paymentLag = _strPaymentLag == string.Empty ? 0 : Parsers.ParseInteger(_strPaymentLag);
            _eom = _strEom == string.Empty ? false : Parsers.ParseBool(_strEom);
            _fixedFrequency = _strFixedFrequency == string.Empty ? Frequency.Annual : Parsers.ParseFrequency(_strFixedFrequency);
            _fixedConvention = _strFixedConvention == string.Empty ? BusinessDayConvention.Following : Parsers.ParseBusinessDayConvention(_strFixedConvention);
            _fixedPaymentConvention = _strFixedPaymentConvention == string.Empty ? BusinessDayConvention.Following : Parsers.ParseBusinessDayConvention(_strFixedPaymentConvention);        
            //_rule = _strRule == string.Empty ? DateGeneration.Rule.Backward : Parsers.ParseDateGenerationRule(_strRule);
        }

        
    }
}
