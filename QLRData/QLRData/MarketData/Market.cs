using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public enum YieldCurveType
    {
        Discount = 0, // Chosen to match MarketObject::DiscountCurve
        Yield = 1,    // Chosen to match MarketObject::YieldCurve
        EquityDividend = 2,
        EquityForecast = 3
    };

    public abstract class Market
    {
        public const string DefaultConfiguration = "default";

        public abstract Date AsOfDate();

        // Yield Curves
        public abstract Handle<YieldTermStructure> YieldCurve(YieldCurveType type, string name, string configuration = DefaultConfiguration);
        public abstract Handle<YieldTermStructure> DiscoutCurve(string ccy, string name, string configuration = DefaultConfiguration);
        public abstract Handle<IborIndex> IborIndex(string indexName, string configuration = DefaultConfiguration);
        public abstract Handle<IborIndex> SwapIndex(string indexName, string configuration = DefaultConfiguration);

        // Foreign Exchange
        public abstract Handle<Quote> FxSpot(string ccypair, string configuration = DefaultConfiguration);
        public abstract Handle<BlackVolTermStructure> FxVol(string ccypair, string configuration = DefaultConfiguration);

        public virtual void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
