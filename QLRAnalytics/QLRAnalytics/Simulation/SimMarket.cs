using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QLRData;

namespace QLRAnalytics
{
    public class SimMarket : MarketImpl
    {
        protected double _numeraire;

        public SimMarket(Conventions conventions) : base(conventions)
        {
            _numeraire = 1.0;
        }

        public virtual void Update(Date d)
        {
            throw new NotImplementedException();
        }

        public virtual void Reset()
        {
            throw new NotImplementedException();
        }

        public double Numeraire()
        {
            return _numeraire;
        }
        
        
    }
}
