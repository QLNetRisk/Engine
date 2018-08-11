using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class FxSpot
    {
        private Handle<Quote> _spot;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asof"></param>
        /// <param name="spec"></param>
        public FxSpot(Date asof, FXSpotSpec spec)
        {

        }

        public Handle<Quote> Handle()
        {
            return _spot;
        }
    }
}
