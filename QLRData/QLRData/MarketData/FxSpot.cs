using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class FXSpot
    {
        private Handle<Quote> _spot;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asof"></param>
        /// <param name="spec"></param>
        public FXSpot(Date asof, FXSpotSpec spec, Loader loader)
        {
            foreach (var md in loader.LoadQuotes(asof))
            {
                if (md.AsofDate() == asof && md.GetInstrumentType() == MarketDatum.InstrumentType.FX_SPOT)
                {
                    FXSpotQuote q = md as FXSpotQuote;
                    Utils.QL_REQUIRE(q != null, () => "Failed to cast " + md.Name() + " to FXSpotQuote");
                    if (q.UnitCcy() == spec.UnitCcy() && q.Ccy() == spec.Ccy())
                    {
                        _spot = q.Quote();
                        return;
                    }
                }
            }
            Utils.QL_FAIL("Failed to find a quote for " + spec);
        }

        public Handle<Quote> Handle()
        {
            return _spot;
        }
    }
}
