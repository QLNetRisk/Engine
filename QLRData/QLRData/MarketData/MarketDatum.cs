using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class MarketDatum
    {
        public enum InstrumentType
        {
            ZERO,
            DISCOUNT,
            MM,
            MM_FUTURE,
            FRA,
            IMM_FRA,
            IR_SWAP,
            BASIS_SWAP,
            CC_BASIS_SWAP,
            CDS,
            CDS_INDEX,
            FX_SPOT,
            FX_FWD,
            HAZARD_RATE,
            RECOVERY_RATE,
            SWAPTION,
            CAPFLOOR,
            FX_OPTION,
            ZC_INFLATIONSWAP,
            ZC_INFLATIONCAPFLOOR,
            YY_INFLATIONSWAP,
            SEASONALITY,
            EQUITY_SPOT,
            EQUITY_FWD,
            EQUITY_DIVIDEND,
            EQUITY_OPTION,
            BOND,
            INDEX_CDS_OPTION
        };

        //! Supported market quote types
        public enum QuoteType
        {
            BASIS_SPREAD,
            CREDIT_SPREAD,
            YIELD_SPREAD,
            HAZARD_RATE,
            RATE,
            RATIO,
            PRICE,
            RATE_LNVOL,
            RATE_NVOL,
            RATE_SLNVOL,
            BASE_CORRELATION,
            SHIFT
        };

        protected Handle<Quote> _quote;
        protected Date _asofDate;
        protected string _name;
        protected InstrumentType _instrumentType;
        protected QuoteType _quoteType;

        public MarketDatum(double value, Date asofDate, string name, QuoteType quoteType, InstrumentType instrumentType)
        {
            _quote = new Handle<Quote>(new SimpleQuote(value));
            _asofDate = asofDate;
            _name = name;
            _quoteType = quoteType;
            _instrumentType = instrumentType;
        }

        public string Name()
        {
            return _name;
        }
        public Handle<Quote> Quote()
        {
            return _quote;
        }
        public Date AsofDate()
        {
            return _asofDate;
        }

        public InstrumentType GetInstrumentType()
        {
            return _instrumentType;
        }
        public QuoteType GetQuoteType()
        {
            return _quoteType;
        }
    }

    public class ZeroQuote: MarketDatum
    {
        private string _ccy;
        private Date _date;
        private DayCounter _dayCounter;
        private Period _tenor;
        private bool _tenorBased;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="ccy"></param>
        /// <param name="date"></param>
        /// <param name="dayCounter"></param>
        /// <param name="tenor"></param>
        public ZeroQuote(double value, Date asofDate, string name, QuoteType quoteType, string ccy, Date date, DayCounter dayCounter, Period tenor = null)
        : base(value, asofDate, name, quoteType, InstrumentType.ZERO)
        {
            _ccy = ccy;
            _date = date;
            _dayCounter = dayCounter;
            _tenor = tenor;
            // Minimal adjustment in the absence of a calendar
            Utils.QL_REQUIRE(date != new Date() || tenor != null, () => "ZeroQuote: either date or period is required");
            _tenorBased = (_date == new Date());
        }

        public string Ccy()
        {
            return _ccy;
        }
        public Date Date()
        {
            return _date;
        }
        public DayCounter DayCounter()
        {
            return _dayCounter;
        }
        public Period Tenor()
        {
            return _tenor;
        }
        public bool TenorBased()
        {
            return _tenorBased;
        }
    }

    public class DiscountQuote : MarketDatum
    {
        private string _ccy;
        private Date _date;

        //! Constructor
        public DiscountQuote(double value, Date asofDate, string name, QuoteType quoteType, string ccy, Date date)
            : base(value, asofDate, name, quoteType, InstrumentType.DISCOUNT)
        {
            _ccy = ccy;
            _date = date;
        }
        
        public string Ccy()
        {
            return _ccy;
        }
        public Date Date()
        {
            return _date;
        }
    }

    public class FXSpotQuote : MarketDatum
    {
        private string _unitCcy;
        private string _ccy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="unitCcy"></param>
        /// <param name="ccy"></param>
        public FXSpotQuote(double value, Date asofDate, string name, QuoteType quoteType, string unitCcy, string ccy) : base(value, asofDate, name, quoteType, InstrumentType.FX_SPOT)
        {
            _unitCcy = unitCcy;
            _ccy = ccy;
        }
    
        public string UnitCcy()
        {
            return _unitCcy;
        }
        public string Ccy()
        {
            return _ccy;
        } 
    }

    public class FRAQuote : MarketDatum
    {
        private string _ccy;
        private Period _fwdStart;
        private Period _term;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="ccy"></param>
        /// <param name="fwdStart"></param>
        /// <param name="term"></param>
        public FRAQuote(double value, Date asofDate, string name, QuoteType quoteType, string ccy, Period fwdStart, Period term)
            : base(value, asofDate, name, quoteType, InstrumentType.FRA)
        {
            _term = term;
            _ccy = ccy;
            _fwdStart = fwdStart;
        }

        public string Ccy()
        {
            return _ccy;
        }
        public Period FwdStart()
        {
            return _fwdStart;
        }
        public Period Term()
        {
            return _term;
        }   
    }

    public class SwapQuote : MarketDatum
    {
        private string _ccy;
        private Period _fwdStart;
        private Period _term;
        private Period _tenor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="ccy"></param>
        /// <param name="fwdStart"></param>
        /// <param name="term"></param>
        /// <param name="tenor"></param>
        public SwapQuote(double value, Date asofDate, string name, QuoteType quoteType, string ccy, Period fwdStart, Period term, Period tenor)
            : base(value, asofDate, name, quoteType, InstrumentType.IR_SWAP)  
        {
            _ccy = ccy;
            _fwdStart = fwdStart;
            _term = term;
            _tenor = tenor;
        }
        
        public string Ccy()
        {
            return _ccy;
        }
        public Period FwdStart()
        {
            return _fwdStart;
        }
        public Period Term()
        {
            return _term;
        }
        public Period Tenor()
        {
            return _tenor;
        }
    }

    public class CrossCcyBasisSwapQuote : MarketDatum
    {
        private string _flatCcy;
        private Period _flatTerm;
        private string _ccy;
        private Period _term;
        private Period _maturity;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="flatCcy"></param>
        /// <param name="flatTerm"></param>
        /// <param name="ccy"></param>
        /// <param name="term"></param>
        /// <param name="maturity"></param>
        public CrossCcyBasisSwapQuote(double value, Date asofDate, string name, QuoteType quoteType, string flatCcy,
                               Period flatTerm, string ccy, Period term, Period maturity = null)
            : base(value, asofDate, name, quoteType, InstrumentType.CC_BASIS_SWAP)
        {
            _flatCcy = flatCcy;
            _flatTerm = flatTerm;
            _ccy = ccy;
            _term = term;
            _maturity = maturity == null? new Period(3, TimeUnit.Months) : maturity;                
        }
    
        public string FlatCcy()
        {
            return _flatCcy;
        }
        public Period FlatTerm()
        {
            return _flatTerm;
        }
        public string Ccy()
        {
            return _ccy;
        }
        public Period Term()
        {
            return _term;
        }
        public Period Maturity()
        {
            return _maturity;
        }
    }

    public class SwaptionQuote : MarketDatum
    {
        private string _ccy;
        private Period _expiry;
        private Period _term;
        private string _dimension;
        private double _strike;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="ccy"></param>
        /// <param name="expiry"></param>
        /// <param name="term"></param>
        /// <param name="dimension"></param>
        /// <param name="strike"></param>
        public SwaptionQuote(double value, Date asofDate, string name, QuoteType quoteType, string ccy, Period expiry,
                      Period term, string dimension, double strike = 0.0)
            : base(value, asofDate, name, quoteType, InstrumentType.SWAPTION)
        {
            _ccy = ccy;
            _expiry = expiry;
            _term = term;
            _dimension = dimension;
            _strike = strike;
        }

        public string Ccy()
        {
            return _ccy;
        }
        public Period Expiry()
        {
            return _expiry;
        }
        public Period Term()
        {
            return _term;
        }
        public string Dimension()
        {
            return _dimension;
        }
        public double Strike()
        {
            return _strike;
        }
    }

    public class FXForwardQuote : MarketDatum
    {
        private string _unitCcy;
        private string _ccy;
        private Period _term;
        private double _conversionFactor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="unitCcy"></param>
        /// <param name="ccy"></param>
        /// <param name="term"></param>
        /// <param name="conversionFactor"></param>
        public FXForwardQuote(double value, Date asofDate, string name, QuoteType quoteType, string unitCcy, string ccy,
                       Period term, double conversionFactor = 1.0)
            : base(value, asofDate, name, quoteType, InstrumentType.FX_FWD)
        {
            _unitCcy = unitCcy;
            _ccy = ccy;
            _term = term;
            _conversionFactor = conversionFactor;
        }
    
        public string UnitCcy()
        {
            return _unitCcy;
        }
        public string Ccy()
        {
            return _ccy;
        }
        public Period Term()
        {
            return _term;
        }
        public double ConversionFactor()
        {
            return _conversionFactor;
        }   
    }

    public class FXOptionQuote : MarketDatum
    {
        private string _unitCcy;
        private string _ccy;
        private Period _expiry;
        private string _strike; // TODO: either: ATM, 25RR, 25BF. Should be an enum?

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asofDate"></param>
        /// <param name="name"></param>
        /// <param name="quoteType"></param>
        /// <param name="unitCcy"></param>
        /// <param name="ccy"></param>
        /// <param name="expiry"></param>
        /// <param name="strike"></param>
        public FXOptionQuote(double value, Date asofDate, string name, QuoteType quoteType, string unitCcy, string ccy, Period expiry, string strike)
            : base(value, asofDate, name, quoteType, InstrumentType.FX_OPTION)
        {
            _unitCcy = unitCcy;
            _ccy = ccy;
            _expiry = expiry;
            _strike = strike;

            Utils.QL_REQUIRE(strike == "ATM" || strike == "25BF" || strike == "25RR", () => "Invalid FXOptionQuote strike (" + strike + ")");
        }

        public string UnitCcy()
        {
            return _unitCcy;
        }
        public string Ccy()
        {
            return _ccy;
        }
        public Period Expiry()
        {
            return _expiry;
        }
        public string Strike()
        {
            return _strike;
        }    
    }
}
