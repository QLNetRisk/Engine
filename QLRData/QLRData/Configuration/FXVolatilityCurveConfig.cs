using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class FXVolatilityCurveConfig : CurveConfig
    {        
        /// <summary>
        /// supported volatility structure types
        /// For ATM we will only load ATM quotes, for Smile we load ATM, 25RR, 25BF
        /// TODO: Add more options (e.g. Delta)
        /// </summary>
        public enum Dimension { ATM, Smile };

        private Dimension _dimension;
        private List<Period> _expiries;
        private DayCounter _dayCounter;
        private Calendar _calendar;
        private string _fxSpotID;
        private string _fxForeignYieldCurveID;
        private string _fxDomesticYieldCurveID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FXVolatilityCurveConfig()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="curveID"></param>
        /// <param name="curveDescription"></param>
        /// <param name="dimension"></param>
        /// <param name="expiries"></param>
        /// <param name="fxSpotID"></param>
        /// <param name="fxForeignCurveID"></param>
        /// <param name="fxDomesticCurveID"></param>
        /// <param name="dayCounter"></param>
        /// <param name="calendar"></param>
        public FXVolatilityCurveConfig(string curveID, string curveDescription, Dimension dimension, List<Period> expiries, string fxSpotID = "",
                             string fxForeignCurveID = "",  string fxDomesticCurveID = "", DayCounter dayCounter = null, Calendar calendar = null)
            : base(curveID, curveDescription)
        {
            _dimension = dimension;
            _expiries = expiries;
            _dayCounter = dayCounter;
            _calendar = calendar;
            _fxSpotID = fxSpotID;
            _fxForeignYieldCurveID = fxForeignCurveID;
            _fxDomesticYieldCurveID = fxDomesticCurveID;
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "FXVolatility");

            _curveID = GetChildValue(node, "CurveId", true);
            _curveDescription = GetChildValue(node, "CurveDescription", true);
            string dim = GetChildValue(node, "Dimension", true);
            string cal = GetChildValue(node, "Calendar");
            if (cal == "") cal = "TARGET";

            _calendar = Parsers.ParseCalendar(cal);

            string dc = GetChildValue(node, "DayCounter");
            if (dc == "") dc = "A365";

            _dayCounter = Parsers.ParseDayCounter(dc);

            if (dim == "ATM")
            {
                _dimension = Dimension.ATM;
            }
            else if (dim == "Smile")
            {
                _dimension = Dimension.Smile;
            }
            else
            {
                Utils.QL_FAIL("Dimension " + dim + " not supported yet");
            }
            _expiries = GetChildrenValuesAsPeriods(node, "Expiries", true);

            if (_dimension == Dimension.Smile)
            {
                _fxSpotID = GetChildValue(node, "FXSpotID", true);
                _fxForeignYieldCurveID = GetChildValue(node, "FXForeignCurveID", true);
                _fxDomesticYieldCurveID = GetChildValue(node, "FXDomesticCurveID", true);
            }
        }

        //public override XMLNode* toXML(XMLDocument& doc) override;

        public Dimension GetDimension()
        {
            return _dimension;
        }
        public List<Period> Expiries()
        {
            return _expiries;
        }
        public DayCounter DayCounter()
        {
            return _dayCounter;
        }
        public Calendar Calendar()
        {
            return _calendar;
        }
        // only required for Smile
        public string FxSpotID()
        {
            return _fxSpotID;
        }
        public string FxForeignYieldCurveID()
        {
            return _fxForeignYieldCurveID;
        }
        public string FxDomesticYieldCurveID()
        {
            return _fxDomesticYieldCurveID;
        }
        public override List<string> Quotes()
        {
            if (_quotes.Count == 0)
            {
                List<string> tokens = FxSpotID().Split('/').ToList();                
                string b = "FX_OPTION/RATE_LNVOL/" + tokens[1] + "/" + tokens[2] + "/";
                foreach(var e in _expiries)
                {
                    _quotes.Add(b + e.ToString() + "/ATM");
                    if (_dimension == Dimension.Smile)
                    {
                        _quotes.Add(b + e.ToString() + "/25RR");
                        _quotes.Add(b + e.ToString() + "/25BF");
                    }
                }
            }
            return _quotes;
        }        
    }
}
