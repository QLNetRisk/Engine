using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public abstract class IborIndexParser
    {
        public abstract IborIndex Build(Period p, Handle<YieldTermStructure> h);        
    }

    public class IborIndexParserWithPeriod<T> : IborIndexParser where T : IborIndex
    {
        public override IborIndex Build(Period p, Handle<YieldTermStructure> h)
        {
            QLNet.Utils.QL_REQUIRE(p != new Period(1, TimeUnit.Days), () => "must have a period longer than 1D");
            return (T)Activator.CreateInstance(typeof(T), p, h);
        }
    }

    public class IborIndexParserOIS<T> : IborIndexParser where T : IborIndex
    {
        public override IborIndex Build(Period p, Handle<YieldTermStructure> h)
        {
            QLNet.Utils.QL_REQUIRE(p == new Period(1, TimeUnit.Days), () => "must have period 1D");
            return (T)Activator.CreateInstance(typeof(T), p, h);
        }
    }

}
