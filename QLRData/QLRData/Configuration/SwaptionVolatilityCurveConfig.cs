using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class SwaptionVolatilityCurveConfig : CurveConfig
    {
        // supported volatility dimensions
        public enum Dimension { ATM, Smile };
        // supported volatility types
        public enum VolatilityType { Lognormal, Normal, ShiftedLognormal };

        private Dimension _dimension;
        private VolatilityType _volatilityType;
        private bool _extrapolate, _flatExtrapolation;
        private List<Period> _optionTenors, _swapTenors;
        private DayCounter _dayCounter;
        private Calendar _calendar;
        private BusinessDayConvention _businessDayConvention;
        private string _shortSwapIndexBase, _swapIndexBase;
        private List<Period> _smileOptionTenors;
        private List<Period> _smileSwapTenors;
        private List<double> _smileSpreads;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SwaptionVolatilityCurveConfig()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="curveID"></param>
        /// <param name="curveDescription"></param>
        /// <param name="dimension"></param>
        /// <param name="volatilityType"></param>
        /// <param name="extrapolate"></param>
        /// <param name="flatExtrapolation"></param>
        /// <param name="optionTenors"></param>
        /// <param name="swapTenors"></param>
        /// <param name="dayCounter"></param>
        /// <param name="calendar"></param>
        /// <param name="businessDayConvention"></param>
        /// <param name="shortSwapIndexBase"></param>
        /// <param name="swapIndexBase"></param>
        /// <param name="smileOptionTenors"></param>
        /// <param name="smileSwapTenors"></param>
        /// <param name="smileSpreads"></param>
        public SwaptionVolatilityCurveConfig(string curveID, string curveDescription, Dimension dimension,
                                   VolatilityType volatilityType, bool extrapolate,
                                   bool flatExtrapolation, List<Period> optionTenors,
                                   List<Period> swapTenors, DayCounter dayCounter,
                                   Calendar calendar, BusinessDayConvention businessDayConvention,
                                   string shortSwapIndexBase, string swapIndexBase,
                                   List<Period> smileOptionTenors = null,
                                   List<Period> smileSwapTenors = null,
                                   List<double> smileSpreads = null)
            : base(curveID, curveDescription)
        {
            _dimension = dimension;
            _volatilityType = volatilityType;
            _extrapolate = extrapolate;
            _flatExtrapolation = flatExtrapolation;
            _optionTenors = optionTenors;

            _swapTenors = swapTenors;
            _dayCounter = dayCounter;
            _calendar = calendar;
            _businessDayConvention = businessDayConvention;
            _shortSwapIndexBase = shortSwapIndexBase;
            _swapIndexBase = swapIndexBase;
            _smileOptionTenors = smileOptionTenors;
            _smileSwapTenors = smileSwapTenors;
            _smileSpreads = smileSpreads;

            Utils.QL_REQUIRE(dimension == Dimension.ATM || dimension == Dimension.Smile, () => "Invalid dimension");

            if (dimension != Dimension.Smile)
            {
                Utils.QL_REQUIRE(smileOptionTenors.Count == 0 && smileSwapTenors.Count == 0 && smileSpreads.Count == 0,
                           () => "Smile tenors/strikes/spreads should only be set when dim=Smile");
            }
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "SwaptionVolatility");

            _curveID = GetChildValue(node, "CurveId", true);
            _curveDescription = GetChildValue(node, "CurveDescription", true);

            string dim = GetChildValue(node, "Dimension", true);
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
                Utils.QL_FAIL("Dimension " + dim + " not recognized");
            }

            string volType = GetChildValue(node, "VolatilityType", true);
            if (volType == "Normal")
            {
                _volatilityType = VolatilityType.Normal;
            }
            else if (volType == "Lognormal")
            {
                _volatilityType = VolatilityType.Lognormal;
            }
            else if (volType == "ShiftedLognormal")
            {
                _volatilityType = VolatilityType.ShiftedLognormal;
            }
            else
            {
                Utils.QL_FAIL("Volatility type " + volType + " not recognized");
            }

            string extr = GetChildValue(node, "Extrapolation", true);
            _extrapolate = true;
            _flatExtrapolation = true;
            if (extr == "Linear")
            {
                _flatExtrapolation = false;
            }
            else if (extr == "Flat")
            {
                _flatExtrapolation = true;
            }
            else if (extr == "None")
            {
                _extrapolate = false;
            }
            else
            {
                Utils.QL_FAIL("Extrapolation " + extr + " not recognized");
            }

            _optionTenors = GetChildrenValuesAsPeriods(node, "OptionTenors", true);
            _swapTenors = GetChildrenValuesAsPeriods(node, "SwapTenors", true);

            string cal = GetChildValue(node, "Calendar", true);
            _calendar = Parsers.ParseCalendar(cal);

            string dc = GetChildValue(node, "DayCounter", true);
            _dayCounter = Parsers.ParseDayCounter(dc);

            string bdc = GetChildValue(node, "BusinessDayConvention", true);
            _businessDayConvention = Parsers.ParseBusinessDayConvention(bdc);

            _shortSwapIndexBase = GetChildValue(node, "ShortSwapIndexBase", true);
            _swapIndexBase = GetChildValue(node, "SwapIndexBase", true);

            // optional smile stuff
            _smileOptionTenors = GetChildrenValuesAsPeriods(node, "SmileOptionTenors", false);
            _smileSwapTenors = GetChildrenValuesAsPeriods(node, "SmileSwapTenors", false);
            _smileSpreads = GetChildrenValuesAsDoublesCompact(node, "SmileSpreads", false);
        }

        // name Inspectors        
        public Dimension GetDimension()
        {
            return _dimension;
        }
        public VolatilityType GetVolatilityType()
        {
            return _volatilityType;
        }
        public bool Extrapolate()
        {
            return _extrapolate;
        }
        public bool FlatExtrapolation()
        {
            return _flatExtrapolation;
        }
        public List<Period> OptionTenors()
        {
            return _optionTenors;
        }
        public List<Period> SwapTenors()
        {
            return _swapTenors;
        }
        public DayCounter DayCounter()
        {
            return _dayCounter;
        }
        public Calendar Calendar()
        {
            return _calendar;
        }
        public BusinessDayConvention BusinessDayConvention()
        {
            return _businessDayConvention;
        }
        public string ShortSwapIndexBase()
        {
            return _shortSwapIndexBase;
        }
        public string swapIndexBase()
        {
            return _swapIndexBase;
        }
        public List<Period> SmileOptionTenors()
        {
            return _smileOptionTenors;
        }
        public List<Period> SmileSwapTenors()
        {
            return _smileSwapTenors;
        }
        public List<double> SmileSpreads()
        {
            return _smileSpreads;
        }


        public override List<string> Quotes()
        {
            if (_quotes.Count == 0)
            {
                List<string> tokens = _swapIndexBase.Split('-').ToList();                 

                Currency ccy = Parsers.ParseCurrency(tokens[0]);

                string baseStr = "SWAPTION/" + _volatilityType + "/" + ccy.code + "/";
               
             
                if (_dimension == Dimension.ATM)
                {
                    foreach (var o in _optionTenors)
                    {
                        foreach (var s in _swapTenors)
                        {
                            string ss = "";
                            ss += baseStr + o.ToString() + "/" + s + "/ATM";
                            _quotes.Add(ss);
                        }
                    }
                }
                else
                {
                    foreach (var o in _smileOptionTenors)
                    {
                        foreach (var s in _smileSwapTenors)
                        {
                            foreach (var sp in _smileSpreads)
                            {
                                string ss = "";
                                ss += baseStr + o.ToString() + "/" + s + "/Smile/" + sp;
                                _quotes.Add(ss);
                            }
                        }
                    }
                }
            }

            return _quotes;
        }
    }
}
