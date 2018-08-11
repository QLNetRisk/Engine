using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    //! elements must be numbered 0...n, so we can iterate over them
    public enum MarketObject
    {
        DiscountCurve = 0,
        YieldCurve = 1,
        IndexCurve = 2,
        SwapIndexCurve = 3,
        FXSpot = 4,
        FXVol = 5,
        SwaptionVol = 6,
        DefaultCurve = 7,
        CDSVol = 8,
        BaseCorrelation = 9,
        CapFloorVol = 10,
        ZeroInflationCurve = 11,
        YoYInflationCurve = 12,
        InflationCapFloorPriceSurface = 13,
        EquityCurve = 14,
        EquityVol = 15,
        Security = 16
    };    

    public class MarketConfiguration
    {
        public const int NumberOfMarketObjects = 17;
        private Dictionary<MarketObject, string> _marketObjectIds;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketConfiguration()
        {
            for (int i = 0; i < NumberOfMarketObjects; ++i) {
                _marketObjectIds[(MarketObject)i] = Market.DefaultConfiguration;
            }
        }

        public void SetId(MarketObject o, string id) 
        {
            if (id != "") _marketObjectIds[o] = id;
        }
    }
}
