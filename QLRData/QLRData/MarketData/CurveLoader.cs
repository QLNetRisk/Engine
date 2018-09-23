//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace QLRData
//{
//    public static class CurveLoader
//    {
//        public static void Order(List<CurveSpec> curveSpecs, CurveConfigurations curveConfigs)
//        {
//            /* Order the curve specs and remove duplicates (i.e. those with same name).
//             * The sort() call relies on CurveSpec::operator< which ensures a few properties:
//             * - FX loaded before FXVol
//             * - Eq loaded before EqVol
//             * - Inf loaded before InfVol
//             */
//            curveSpecs.Sort( begin(), curveSpecs.end());
//                auto itSpec = unique(curveSpecs.begin(), curveSpecs.end());
//                curveSpecs.resize(distance(curveSpecs.begin(), itSpec));

//            /* remove the YieldCurveSpecs from curveSpecs
//             */
//            vector<boost::shared_ptr<YieldCurveSpec>> yieldCurveSpecs;
//                itSpec = curveSpecs.begin();
//            while (itSpec != curveSpecs.end()) {
//                if ((* itSpec)->baseType() == CurveSpec::CurveType::Yield) {
//                    boost::shared_ptr<YieldCurveSpec> spec = boost::dynamic_pointer_cast<YieldCurveSpec>(*itSpec);
//                yieldCurveSpecs.push_back(spec);
//                    itSpec = curveSpecs.erase(itSpec);
//                } else
//                    ++itSpec;
//            }

//        /* Now sort the yieldCurveSpecs, store them in sortedYieldCurveSpecs  */
//        vector<boost::shared_ptr<YieldCurveSpec>> sortedYieldCurveSpecs;

//            /* Loop over yieldCurveSpec, remove all curvespecs that we can build by checking sortedYieldCurveSpecs
//             * Repeat until yieldCurveSpec is empty
//             */
//            while (yieldCurveSpecs.size() > 0) {
//                Size n = yieldCurveSpecs.size();

//        auto it = yieldCurveSpecs.begin();
//                while (it != yieldCurveSpecs.end()) {
//                    if (canBuild(* it, sortedYieldCurveSpecs, curveConfigs)) {
//                        DLOG("can build " << (* it)->curveConfigID());
//        sortedYieldCurveSpecs.push_back(* it);
//                        it = yieldCurveSpecs.erase(it);
//                    } else {
//                        DLOG("can not (yet) build " << (* it)->curveConfigID());
//                        ++it;
//                    }
//                }
//                QL_REQUIRE(n > yieldCurveSpecs.size(), "missing curve or cycle in yield curve spec");
//            }

//            /* Now put them into the front of curveSpecs */
//            curveSpecs.insert(curveSpecs.begin(), sortedYieldCurveSpecs.begin(), sortedYieldCurveSpecs.end());

//            DLOG("Ordered Curves (" << curveSpecs.size() << ")")
//            for (Size i = 0; i<curveSpecs.size(); ++i)
//                DLOG(std::setw(2) << i << " " << curveSpecs[i]->name());
//        }
//    }
//}
