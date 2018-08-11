using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class FxTriangulation
    {
        private Dictionary<string, Handle<Quote>> _dict;

        /// <summary>
        /// Constructor
        /// </summary>
        public FxTriangulation()
        {

        }

        public void AddQuote(string pair, Handle<Quote> spot)
        {
            _dict[pair] = spot;
        }

        public virtual Handle<Quote> GetQuote(string pair)
        {
            // first, look for the pair in the map
            if (_dict.ContainsKey(pair)) return _dict[pair];
            
            // now we have to break the pair up and search for it.
            QLNet.Utils.QL_REQUIRE(pair.Length == 6, () => "invalid ccypair " + pair);
            string domestic = pair.Substring(0, 3);
            string foreign = pair.Substring(3, 3);

            // check reverse
            string reverse = foreign + domestic;            
            if (_dict.ContainsKey(reverse))
            {
                Handle<Quote> invertedquote = new Handle<Quote>(_dict[reverse]);
                _dict[pair] = invertedquote;
                return invertedquote;
            }

            // check eureur
            if (foreign == domestic)
            {
                Handle<Quote> unity = new Handle<Quote>(new SimpleQuote(1.0));
                return unity;
            }

            // now we search for a pair of quotes that we can combine to construct the quote required.
            // we only search for a pair of quotes a single step apart.
            //
            // suppose we want a usdjpy quote and we have eur based data, there are 4 combinations to
            // consider:
            // eurusd, eurjpy  => we want eurjpy / eurusd [triangulation]
            // eurusd, jpyeur  => we want 1 / (eurusd * jpyeur) [inverseproduct]
            // usdeur, eurjpy  => we want usdeur * eurjpy [product]
            // usdeur, jpyeur  => we want usdeur / jpyeur [triangulation (but in the reverse order)]
            //
            // loop over the map, look for domestic then use the map to find the other side of the pair.
            
            //for (const auto&kv : map_) {
            //    string keydomestic = kv.first.substr(0, 3);
            //    string keyforeign = kv.first.substr(3);
            //    const handle<quote>&q1 = kv.second;

            //    if (domestic == keydomestic)
            //    {
            //        // we have domestic, now look for foreign/keyforeign
            //        // usdeur, jpyeur  => we want usdeur / jpyeur [triangulation (but in the reverse order)]
            //        it = map_.find(foreign + keyforeign);
            //        if (it != map_.end())
            //        {
            //            // here q1 is usdeur and it->second is jpyeur
            //            map_[pair] =
            //                handle<quote>(boost::make_shared<compositequote<triangulation>>(q1, it->second, triangulation()));
            //            return map_[pair];
            //        }
            //        // usdeur, eurjpy  => we want usdeur * eurjpy [product]
            //        it = map_.find(keyforeign + foreign);
            //        if (it != map_.end())
            //        {
            //            map_[pair] = handle<quote>(boost::make_shared<compositequote<product>>(q1, it->second, product()));
            //            return map_[pair];
            //        }
            //    }

            //    if (domestic == keyforeign)
            //    {
            //        // eurusd, jpyeur  => we want 1 / (eurusd * jpyeur) [inverseproduct]
            //        it = map_.find(foreign + keydomestic);
            //        if (it != map_.end())
            //        {
            //            map_[pair] =
            //                handle<quote>(boost::make_shared<compositequote<inverseproduct>>(q1, it->second, inverseproduct()));
            //            return map_[pair];
            //        }
            //        // eurusd, eurjpy  => we want eurjpy / eurusd [triangulation]
            //        it = map_.find(keydomestic + foreign);
            //        if (it != map_.end())
            //        {
            //            // here q1 is eurusd and it->second is eurjpy
            //            map_[pair] =
            //                handle<quote>(boost::make_shared<compositequote<triangulation>>(it->second, q1, triangulation()));
            //            return map_[pair];
            //        }
            //    }
            //}

            QLNet.Utils.QL_FAIL("unable to build fxquote for ccy pair " + pair);
            return null;
        }

        public Dictionary<string, Handle<Quote>> Quotes()
        {
            return _dict;
        }
    }
}
