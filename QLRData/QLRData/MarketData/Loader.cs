using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public abstract class Loader
    {
        public abstract List<MarketDatum> LoadQuotes(Date d);
        public abstract MarketDatum Get(string name, Date d);
        //public abstract List<Fixing>
    }


}
