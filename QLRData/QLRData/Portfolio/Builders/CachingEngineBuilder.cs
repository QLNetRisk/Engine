using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    // TODO: Can we have unkown number of generic types (in C++ => Args...)
    public abstract class CachingEngineBuilder<T, U, Arg1, Arg2> : EngineBuilder
    {
        protected Dictionary<T, U> _engines;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="engine"></param>
        /// <param name="tradeTypes"></param>
        public CachingEngineBuilder(string model, string engine, HashSet<string> tradeTypes) : base(model, engine, tradeTypes)
        {

        }

        public U Engine(Arg1 arg1, Arg2 arg2)
        {
            T key = KeyImpl(arg1, arg2);

            if (_engines.ContainsKey(key))
            {
                // build first (in case it throws)
                U engine = EngineImpl(arg1, arg2);
                // then add to dictionary
                _engines[key] = engine;
            }

            return _engines[key];
        }

        protected virtual T KeyImpl(Arg1 arg1, Arg2 arg2)
        {
            throw new NotImplementedException();
        }

        protected virtual U EngineImpl(Arg1 arg1, Arg2 arg2)
        {
            throw new NotImplementedException();
        }
    }
}
