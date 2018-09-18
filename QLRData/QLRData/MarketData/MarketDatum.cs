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
}
