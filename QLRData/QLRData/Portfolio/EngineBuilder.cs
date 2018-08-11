using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class EngineBuilder
    {
        protected string _model;
        protected string _engine;
        protected HashSet<string> _tradeTypes;
        protected Market _market;
        protected Dictionary<MarketContext, string> _configurations;
        protected Dictionary<string, string> _modelParameters;
        protected Dictionary<string, string> _engineParameters;
        //protected HashSet<Pair<string, ModelBuilder>>> _modelBuilders;

        public EngineBuilder(string model, string engine, HashSet<string> tradeTypes)
        {
            _model = model;
            _engine = engine;
            _tradeTypes = tradeTypes;
        }

        public string Model()
        {
            return _model;
        }

        public string Engine()
        {
            return _engine;
        }

        public HashSet<string> TradeTypes()
        {
            return _tradeTypes;
        }

        public void Init(Market market, Dictionary<MarketContext, string> configuration, Dictionary<string, string> modelParameters, Dictionary<string, string> engineParameters)
        {
            _market = market;
            _configurations = configuration;
            _modelParameters = modelParameters;
            _engineParameters = engineParameters;
        }
        
        public string Configuration(MarketContext key)
        {
            if (_configurations.ContainsKey(key))
            {
                return _configurations[key];
            }
            else
            {
                return Market.DefaultConfiguration;
            }
        }

        //public HashSet<Pair<string, ModelBuilder>> ModelBuilder()
        //{
        //    return _modelBuilders;
        //}
    }
}