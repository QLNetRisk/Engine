using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class MarketImpl : Market
    {
        protected Date _asof;
        protected Dictionary<Tuple<string, YieldCurveType, string>, Handle<YieldTermStructure>> _yieldCurves;
        protected Dictionary<Tuple<string, string>, Handle<IborIndex>> _iborIndices;
        protected Dictionary<Tuple<string, string>, Handle<SwapIndex>> _swapIndices;
        protected Dictionary<string, FxTriangulation> _fxSpots = new Dictionary<string, FxTriangulation>();
        protected Conventions _conventions;

        public MarketImpl(Conventions conventions)
        {
            _conventions = conventions;
            _fxSpots[Market.DefaultConfiguration] = new FxTriangulation();
        }        

        public override Handle<YieldTermStructure> YieldCurve(YieldCurveType type, string key, string configuration = "default")
        {
            throw new NotImplementedException();
            //return _yieldCurves[new Tuple { key, type, configuration }, "yield curve"];
        }

        public override Handle<YieldTermStructure> DiscoutCurve(string ccy, string name, string configuration = "default")
        {
            throw new NotImplementedException();
        }

        public override Handle<IborIndex> IborIndex(string indexName, string configuration = "default")
        {
            throw new NotImplementedException();
        }

        public override Handle<IborIndex> SwapIndex(string indexName, string configuration = "default")
        {
            throw new NotImplementedException();
        }

        public override Handle<Quote> FxSpot(string ccypair, string configuration = "default")
        {
            if (_fxSpots.ContainsKey(configuration)) return _fxSpots[configuration].GetQuote(ccypair);
            else return _fxSpots[Market.DefaultConfiguration].GetQuote(ccypair);
        }

        public override Handle<BlackVolTermStructure> FxVol(string ccypair, string configuration = "default")
        {
            throw new NotImplementedException();
        }

        public override Date AsOfDate()
        {
            throw new NotImplementedException();
        }
    }
}
