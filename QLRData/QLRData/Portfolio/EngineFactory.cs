using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public enum MarketContext { irCalibration, fxCalibration, eqCalibration, pricing };

    public class EngineFactory
    {
        private Market _market;
        private EngineData _engineData;
        private Dictionary<MarketContext, string> _configurations;
        private Dictionary<Tuple<string, string, HashSet<string>>, EngineBuilder> _builders;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engineData"></param>
        /// <param name="market"></param>
        /// <param name="configurations"></param>
        public EngineFactory(EngineData engineData, Market market, Dictionary<MarketContext, string> configurations)
        {
            //LOG("Building EngineFactory");
            _market = market;
            _engineData = engineData;
            _configurations = configurations;

        }

        public void RegisterBuilder(EngineBuilder builder)
        {

        }

        public void AddDefaultBuilders()
        {

        }

        public EngineBuilder Builder(string tradeType)
        {
            // Check that we have a model/engine for tradetype
            QLNet.Utils.QL_REQUIRE(_engineData.HasProduct(tradeType), () => "EngineFactory does not have a model/engine for trade type " + tradeType);

            string model = _engineData.Model(tradeType);
            string engine = _engineData.Engine(tradeType);
            HashSet<string> set = new HashSet<string>() { tradeType };
            Tuple<string, string, HashSet<string>> key = new Tuple<string, string, HashSet<string>>(model, engine, set);

            QLNet.Utils.QL_REQUIRE(_builders.ContainsKey(key), () => "No EngineBuilder for " + model + "/" + engine + "/" + tradeType);            

            EngineBuilder builder = _builders[key];
            builder.Init(_market, _configurations, _engineData.ModelParameters(tradeType), _engineData.EngineParameters(tradeType));

            return builder;
        }
    }
}
