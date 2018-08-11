using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QLRData;

namespace QLRAnalytics
{
    public class ValuationCalculator
    {
        public virtual void Calculate(Trade trade, int tradeIndex, SimMarket simMarket, Date date, int dateIndex)
        {
            throw new NotImplementedException();
        }

        public virtual void CalculateT0(Trade trade, int tradeIndex, SimMarket simMarket)
        {
            throw new NotImplementedException();
        }
    }
}
