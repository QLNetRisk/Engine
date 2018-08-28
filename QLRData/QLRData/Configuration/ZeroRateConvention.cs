using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
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
            CheckNode(node, "Zero");
            _type = Type.Zero;
            _id = GetChildValue(node, "Id", true);
            _tenorBased = GetChildValueAsBool(node, "TenorBased", true);

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
            _compounding = _strCompounding == string.Empty ? QLNet.Compounding.Continuous : Parsers.ParseCompounding(_strCompounding);
            _compoundingFrequency = _strCompoundingFrequency == string.Empty ? Frequency.Annual : Parsers.ParseFrequency(_strCompoundingFrequency);

            if (_tenorBased)
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
