using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QuantExt;

namespace QLRData
{    
    public class FxForwardEngineBuilder : CachingEngineBuilder<string, QLNet.IPricingEngine, Currency, Currency>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FxForwardEngineBuilder() : base("DiscountedCashflows", "DiscountingFxForwardEngine", new HashSet<string>{ "FxForward"})
        {

        }

        protected override QLNet.IPricingEngine EngineImpl(Currency forCcy, Currency domCcy)
        {
            string pair = KeyImpl(forCcy, domCcy);

            return new DiscountingFxForwardEngine(domCcy, _market.DiscoutCurve(domCcy.code, Configuration(MarketContext.pricing)), 
                                                  forCcy, _market.DiscoutCurve(forCcy.code, Configuration(MarketContext.pricing)), 
                                                  _market.FxSpot(pair, Configuration(MarketContext.pricing)), true, new Date(), new Date());
        }

        protected override string KeyImpl(Currency forCcy, Currency domCcy)
        {
            return forCcy.code + domCcy.code;
        }
    }
}