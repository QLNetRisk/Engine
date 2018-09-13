using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class Fixing
    {
        // Fixing date
        public Date date { get; set; }
        // Index name
        public string name { get; set; }
        // Fixing amount
        public double fixing { get; set; }

//        void applyFixings(const vector<Fixing>& fixings, const data::Conventions& conventions) {
//    Size count = 0;
//        map<string, boost::shared_ptr<Index>> cache;
//        boost::timer timer;
//        boost::shared_ptr<Index> index;
//    for (auto& f : fixings) {
//        try {
//            auto it = cache.find(f.name);
//            if (it == cache.end()) {
//                index = parseIndex(f.name, conventions);
//        cache[f.name] = index;
//            } else {
//                index = it->second;
//            }
//index->addFixing(f.date, f.fixing, true);
//TLOG("Added fixing for " << f.name << " (" << io::iso_date(f.date) << ") value:" << f.fixing);
//count++;
//        } catch (const std::exception& e) {
//            WLOG("Error during adding fixing for " << f.name << ": " << e.what());
//        }
//    }
//    DLOG("Added " << count << " of " << fixings.size() << " fixings in " << timer.elapsed() << " seconds");
//}
    }
}
