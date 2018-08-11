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
