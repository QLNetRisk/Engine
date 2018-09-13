using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class IRSwapConvention : Convention
    {
        private Calendar _fixedCalendar;
        private Frequency _fixedFrequency;
        private BusinessDayConvention _fixedConvention;
        private DayCounter _fixedDayCounter;
        private IborIndex _index;
        private bool _hasSubPeriod;
        private Frequency _floatFrequency;
        //private QuantExt.SubPeriodsCoupon::Type _subPeriodsCouponType;

        // Strings to store the inputs
        private string _strFixedCalendar;
        private string _strFixedFrequency;
        private string _strFixedConvention;
        private string _strFixedDayCounter;
        private string _strIndex;
        private string _strFloatFrequency;
        private string _strSubPeriodsCouponType;

        /// <summary>
        /// Default constructor
        /// </summary>
        public IRSwapConvention()
        {

        }
        
        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fixedCalendar"></param>
        /// <param name="fixedFrequency"></param>
        /// <param name="fixedConvention"></param>
        /// <param name="fixedDayCounter"></param>
        /// <param name="index"></param>
        /// <param name="hasSubPeriod"></param>
        /// <param name="floatFrequency"></param>
        /// <param name="subPeriodCouponType"></param>
        public IRSwapConvention(string id, string fixedCalendar, string fixedFrequency, 
            string fixedConvention, string fixedDayCounter, string index, 
            bool hasSubPeriod = false, string floatFrequency = "",
            string subPeriodCouponType = "") : base(id, Type.Swap)
        {
            _strFixedCalendar = fixedCalendar;
            _strFixedFrequency = fixedFrequency;
            _strFixedConvention = fixedConvention;
            _strFixedDayCounter = fixedDayCounter;
            _strIndex = index;
            _hasSubPeriod = hasSubPeriod;
            _strFloatFrequency = floatFrequency;
            _strSubPeriodsCouponType = subPeriodCouponType;

            Build();
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "Swap");

            _type = Type.Swap;
            _id = GetChildValue(node, "Id", true);

            // Get string values from xml
            _strFixedCalendar = GetChildValue(node, "FixedCalendar", true);
            _strFixedFrequency = GetChildValue(node, "FixedFrequency", true);
            _strFixedConvention = GetChildValue(node, "FixedConvention", true);
            _strFixedDayCounter = GetChildValue(node, "FixedDayCounter", true);
            _strIndex = GetChildValue(node, "Index", true);

            // optional
            _strFloatFrequency = GetChildValue(node, "FloatFrequency", false);
            _strSubPeriodsCouponType = GetChildValue(node, "SubPeriodsCouponType", false);
            _hasSubPeriod = (_strFloatFrequency != "");

            Build();
        }

        public virtual void Build()
        {
            _fixedCalendar = Parsers.ParseCalendar(_strFixedCalendar);
            _fixedFrequency = Parsers.ParseFrequency(_strFixedFrequency);
            _fixedConvention = Parsers.ParseBusinessDayConvention(_strFixedConvention);
            _fixedDayCounter = Parsers.ParseDayCounter(_strFixedDayCounter);
            _index = Parsers.ParseIborIndex(_strIndex);

            if (_hasSubPeriod)
            {
                _floatFrequency = Parsers.ParseFrequency(_strFloatFrequency);
                //_subPeriodsCouponType = Parsers.ParseSubPeriodsCouponType(_strSubPeriodsCouponType);
            }
            else
            {
                _floatFrequency = Frequency.NoFrequency;
                //_subPeriodsCouponType = QuantExt::SubPeriodsCoupon::Compounding;
            }
        }

        /// <summary>
        /// Calendar 
        /// </summary>
        /// <returns></returns>
        public Calendar FixedCalendar()
        {
            return _fixedCalendar;
        }
        /// <summary>
        /// Fixed frequency
        /// </summary>
        /// <returns></returns>
        public Frequency FixedFrequency()
        {
            return _fixedFrequency;
        }
        /// <summary>
        /// Fixed businessday convention
        /// </summary>
        /// <returns></returns>
        public BusinessDayConvention FixedConvention()
        {
            return _fixedConvention;
        }
        /// <summary>
        /// Fixed day counter
        /// </summary>
        /// <returns></returns>
        public DayCounter FixedDayCounter()
        {
            return _fixedDayCounter;
        }
        /// <summary>
        /// Index name
        /// </summary>
        /// <returns></returns>
        public string IndexName()
        {
            return _strIndex;
        }        
        /// <summary>
        /// Ibor index
        /// </summary>
        /// <returns></returns>
        public IborIndex Index()
        {
            return _index;
        }
        /// <summary>
        /// For sub period
        /// </summary>
        /// <returns></returns>
        public bool HasSubPeriod()
        {
            return _hasSubPeriod;
        }
        /// <summary>
        /// Floating frequncy. Return NoFrequency for normal swaps
        /// </summary>
        /// <returns></returns>
        public Frequency FloatFrequency()
        {
            return _floatFrequency;
        }
        //public QuantExt.SubPeriodsCoupon SubPeriodsCouponType()
        //{
        //    return _subPeriodsCouponType;
        //}
    }
}
