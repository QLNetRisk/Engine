using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class TodaysMarket : MarketImpl
    {
        public TodaysMarket(Date asof, TodaysMarketParameters todaysParams, Loader loader, CurveConfigurations curveConfigs, Conventions conventions) : base(conventions)
        {
            // Fixings
            // Apply them now in case a curve builder needs them
            //LOG("Todays Market Loading Fixings");
            //applyFixings(loader.loadFixings(), conventions);
            //LOG("Todays Market Loading Fixing done.");

            // store all curves built, since they might appear in several configurations
            // and might therefore be reused
            Dictionary<string, YieldCurve> requiredYieldCurves = new Dictionary<string, YieldCurve>();
            Dictionary<string, SwapIndex> requiredSwapIndices = new Dictionary<string, SwapIndex>();
            Dictionary<string, FXSpot> requiredFxSpots = new Dictionary<string, FXSpot>();
            Dictionary<string, FXVolCurve> requiredFxVolCurves = new Dictionary<string, FXVolCurve>();
            //Dictionary<string, SwaptionVolCurve> requiredSwaptionVolCurves = new Dictionary<string, SwaptionVolCurve>();           

            foreach (var configuration in todaysParams.Configurations())
            {
                //LOG("Build objects in TodaysMarket configuration " << configuration.first);

                _asof = asof;

                // Build the curve specs                
                List<CurveSpec> specs = new List<CurveSpec>();
                foreach (var it in todaysParams.CurveSpecs(configuration.Key))
                {
                    specs.Add(CurveSpecParser.ParseCurveSpec(it));
                    //DLOG("CurveSpec: " << specs.back()->name());
                }

                // order them
                //Order(specs, curveConfigs);
                bool swapIndicesBuilt = false;

                // Loop over each spec, build the curve and add it to the MarketImpl container.
                for (int count = 0; count < specs.Count; ++count)
                {
                    var spec = specs[count];
                    //LOG("Loading spec " << *spec);

                    switch (spec.BaseType())
                    {
                        case CurveSpec.CurveType.Yield:
                            //YieldCurveSpec ycspec = new YieldCurveSpec(spec);
                            //Utils.QL_REQUIRE(ycspec != null, () => "Failed to convert spec " + spec + " to yield curve spec");
                            //// have we built the curve already ?
                            //auto itr = requiredYieldCurves.find(ycspec->name());
                            //if (itr == requiredYieldCurves.end())
                            //{
                            //    // build
                            //    //LOG("Building YieldCurve for asof " << asof);
                            //    YieldCurve yieldCurve = new YieldCurve(asof, *ycspec, curveConfigs, loader, conventions, requiredYieldCurves);

                            //    itr = requiredYieldCurves.insert(make_pair(ycspec->name(), yieldCurve)).first;
                            //}

                            ////DLOG("Added YieldCurve \"" << ycspec->name() << "\" to requiredYieldCurves map");

                            //if (itr->second->currency().code() != ycspec->ccy())
                            //{
                            //    //WLOG("Warning: YieldCurve has ccy " << itr->second->currency() << " but spec has ccy " << ycspec->ccy());
                            
                            //}

                            //// We may have to add this spec multiple times (for discounting, yield and forwarding curves)
                            //List<YieldCurveType> yieldCurveTypes = new List<YieldCurveType>{ YieldCurveType.Discount, YieldCurveType.Yield };
                            //foreach (var y in yieldCurveTypes)
                            //{
                            //    MarketObject o = y as MarketObject;
                            //    foreach (auto & it : params.mapping(o, configuration.first))
                            //    {
                            //        if (it.second == spec->name())
                            //        {
                            //            LOG("Adding YieldCurve(" << it.first << ") with spec " << *ycspec << " to configuration " << configuration.first);

                            //        yieldCurves_[make_tuple(configuration.first, y, it.first)] = itr->second->handle();
                            //        }
                            //    }
                            //}

                            //for (const auto&it : params.mapping(MarketObject::IndexCurve, configuration.first)) 
                            //{
                            //    if (it.second == spec->name())
                            //    {
                            //        LOG("Adding Index(" << it.first << ") with spec " << *ycspec << " to configuration "
                            //                            << configuration.first);
                            //        iborIndices_[make_pair(configuration.first, it.first)] =
                            //            Handle<IborIndex>(parseIborIndex(it.first, itr->second->handle()));
                            //    }
                            //}
                        break;

                        case CurveSpec.CurveType.FX:
                            FXSpotSpec fxspec = spec as FXSpotSpec;
                            QLNet.Utils.QL_REQUIRE(fxspec != null, () => "Failed to convert spec " + spec + " to fx spot spec");

                            //// have we built the curve already ?
                            //var itr = requiredFxSpots.find(fxspec->name());
                            //if (itr == requiredFxSpots.end())
                            //{
                            //    // build the curve
                            //    LOG("Building FXSpot for asof " << asof);
                            //    boost::shared_ptr<FXSpot> fxSpot = boost::make_shared<FXSpot>(asof, *fxspec, loader);
                            //    itr = requiredFxSpots.insert(make_pair(fxspec->name(), fxSpot)).first;
                            //}

                            //// add the handle to the Market Map (possible lots of times for proxies)
                            //for (const auto&it : params.mapping(MarketObject::FXSpot, configuration.first)) 
                            //{
                            //    if (it.second == spec->name())
                            //    {
                            //        LOG("Adding FXSpot (" << it.first << ") with spec " << *fxspec << " to configuration "
                            //                              << configuration.first);
                            //        fxSpots_[configuration.first].addQuote(it.first, itr->second->handle());
                            //    }
                            //}
                            break;
                        default:
                            throw new NotImplementedException();
                    }          
                }
            }
        }
    }
}
