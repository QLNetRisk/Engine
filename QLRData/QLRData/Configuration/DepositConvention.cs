using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class DepositConvention : Convention
    {
        private string _index;
        private Calendar _calendar;
        private BusinessDayConvention _convention;
        private bool _eom;
        private DayCounter _dayCounter;
        private int _settlementDays;
        private bool _indexBased;

        // Strings to store the inputs
        private string _strCalendar;
        private string _strConvention;
        private string _strEom;
        private string _strDayCounter;
        private string _strSettlementDays;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DepositConvention()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        public DepositConvention(string id, string index) : base(id, Type.Deposit)
        {
            _index = index;
            _indexBased = true;
        }

        public DepositConvention(string id, string calendar, string convention, string eom, string dayCounter, string settlementDays) : base(id, Type.Deposit)
        {            
            _indexBased = false;
            _strCalendar = calendar;
            _strConvention = convention;
            _strEom = eom;
            _strDayCounter = dayCounter;
            _strSettlementDays = settlementDays;

            Build();
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "Deposit");            
            _type = Type.Deposit;
            _id = GetChildValue(node, "Id", true);
            _indexBased = GetChildValueAsBool(node, "IndexBased", true);

            // Get string values from xml
            if (_indexBased)
            {
                _index = GetChildValue(node, "Index", true);
            }
            else
            {                
                _strCalendar = GetChildValue(node, "Calendar", true);
                _strConvention = GetChildValue(node, "Convention", true);
                _strEom = GetChildValue(node, "EOM", true);
                _strDayCounter = GetChildValue(node, "DayCounter", true);
                _strSettlementDays = GetChildValue(node, "SettlementDays", true);
                Build();
            }            
        }

        public virtual void Build()
        {
            _calendar = Parsers.ParseCalendar(_strCalendar);
            _convention = Parsers.ParseBusinessDayConvention(_strConvention);
            _eom = Parsers.ParseBool(_strEom);
            _dayCounter = Parsers.ParseDayCounter(_strDayCounter);
            _settlementDays = Parsers.ParseInteger(_strSettlementDays);
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public string Index()
        {
            return _index;
        }
        /// <summary>
        /// Calendar 
        /// </summary>
        /// <returns></returns>
        public Calendar Calendar()
        {
            return _calendar;
        }
        /// <summary>
        /// Business-day convention
        /// </summary>
        /// <returns></returns>
        public BusinessDayConvention Convention()
        {
            return _convention;
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
        /// Deposit rate day counter
        /// </summary>
        /// <returns></returns>
        public DayCounter DayCounter()
        {
            return _dayCounter;
        }
        /// <summary>
        /// Settlement days
        /// </summary>
        /// <returns></returns>
        public int SettlementDays()
        {
            return _settlementDays;
        }
        /// <summary>
        /// Index-based
        /// </summary>
        /// <returns></returns>
        public bool IndexBased()
        {
            return _indexBased;
        }
    }
}
