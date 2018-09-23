using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class FXVolCurve
    {        
        private FXVolatilityCurveSpec _spec;
        private BlackVolTermStructure _vol;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FXVolCurve()
        {

        }

        //! Detailed constructor
        public FXVolCurve(Date asof, FXVolatilityCurveSpec spec, Loader loader, CurveConfigurations curveConfigs,
               Dictionary<string, FXSpot> fxSpots,
               Dictionary<string, YieldCurve> yieldCurves)
        {
            try
            {
                FXVolatilityCurveConfig config = curveConfigs.FxVolCurveConfig(spec.CurveConfigID());

                Utils.QL_REQUIRE(config.GetDimension() == FXVolatilityCurveConfig.Dimension.ATM ||
                               config.GetDimension() == FXVolatilityCurveConfig.Dimension.Smile,
                           () => "Unkown FX curve building dimension");

                bool isATM = config.GetDimension() == FXVolatilityCurveConfig.Dimension.ATM;

                // We loop over all market data, looking for quotes that match the configuration
                // every time we find a matching expiry we remove it from the list
                // we replicate this for all 3 types of quotes were applicable.
                int n = isATM ? 1 : 3; // [0] = ATM, [1] = RR, [2] = BF
                List<List<FXOptionQuote>> quotes = new List<List<FXOptionQuote>>(n);
                List<List<Period>> expiries = new InitializedList<List<Period>>(n, config.Expiries());
                foreach(var md in loader.LoadQuotes(asof))
                {
                    // skip irrelevant data
                    if (md.AsofDate() == asof && md.GetInstrumentType() == MarketDatum.InstrumentType.FX_OPTION)
                    {
                        FXOptionQuote q = md as FXOptionQuote;

                        if (q.UnitCcy() == spec.UnitCcy() && q.Ccy() == spec.Ccy())
                        {
                            int idx = 999999;
                            if (q.Strike() == "ATM")
                                idx = 0;
                            else if (q.Strike() == "25RR")
                                idx = 1;
                            else if (q.Strike() == "25BF")
                                idx = 2;

                            // silently skip unknown strike strings
                            if ((isATM && idx == 0) || (!isATM && idx <= 2))
                            {
                                if(expiries[idx].Contains(q.Expiry()))
                                //var it = std::find(expiries[idx].begin(), expiries[idx].end(), q->expiry());
                                //if (it != expiries[idx].end())
                                {
                                    // we have a hit
                                    quotes[idx].Add(q);
                                    // remove it from the list
                                    expiries[idx].Remove(q.Expiry());
                                }

                                // check if we are done
                                // for ATM we just check expiries[0], otherwise we check all 3
                                if (expiries[0].empty() && (isATM || (expiries[1].empty() && expiries[2].empty())))
                                    break;
                            }
                        }
                    }
                }

                // Check ATM first
                // Check that we have all the expiries we need
                //LOG("FXVolCurve: read " + quotes[0].Count + " ATM vols");
                Utils.QL_REQUIRE(expiries[0].Count == 0, () => "No ATM quote found for spec " + spec + " with expiry " + expiries[0].First());
                Utils.QL_REQUIRE(quotes[0].Count > 0, () => "No ATM quotes found for spec " + spec);
                // No check the rest
                if (!isATM)
                {
                    //LOG("FXVolCurve: read " + quotes[1].Count + " RR and " + quotes[2].Count + " BF quotes");
                    Utils.QL_REQUIRE(expiries[1].Count == 0, () => "No RR quote found for spec " + spec + " with expiry " + expiries[1].First());
                    Utils.QL_REQUIRE(expiries[2].Count == 0, () => "No BF quote found for spec " + spec + " with expiry " + expiries[2].First());
                }

                // daycounter used for interpolation in time.
                // TODO: push into conventions or config
                DayCounter dc = config.DayCounter();
                Calendar cal = config.Calendar();

                // sort all quotes
                for (int i = 0; i < n; i++)
                {
                    // TODO!!
                    //IComparer<FXOptionQuote> compare = 
                    //quotes[i] = quotes[i].Sort((a,b) => a.Expiry().CompareTo(b.Expiry()))                                   
                }

                // build vol curve
                if (isATM && quotes[0].Count == 1)
                {
                    _vol = new BlackConstantVol(asof, new Calendar(), quotes[0].First().Quote().link.value(), dc);
                }
                else
                {

                    int numExpiries = quotes[0].Count;
                    List<Date> dates = new List<Date>(numExpiries);
                    List<List<double>> vols = new InitializedList<List<double>>(n, new List<double>(numExpiries)); // same as above: [0] = ATM, etc.

                    for (int i = 0; i < numExpiries; i++)
                    {
                        dates[i] = asof + quotes[0][i].Expiry();
                        for (int idx = 0; idx < n; idx++)
                        {
                            vols[idx][i] = quotes[idx][i].Quote().link.value();
                        }                            
                    }

                    if (isATM)
                    {
                        // ATM
                        _vol = new BlackVarianceCurve(asof, dates, vols[0], dc, false);
                    }
                    else
                    {
                        // Smile
                        if(fxSpots.ContainsKey(config.FxSpotID()) && yieldCurves.ContainsKey(config.FxDomesticYieldCurveID()) && yieldCurves.ContainsKey(config.FxForeignYieldCurveID()))
                        {
                            var fxSpot = fxSpots[config.FxSpotID()]; // new Handle<Quote>(config.FxSpotID(), fxSpots);
                            var domYTS = yieldCurves[config.FxDomesticYieldCurveID()];
                            var forYTS = yieldCurves[config.FxForeignYieldCurveID()];

                            // TODO!!
                            //_vol = new QuantExt.FxBlackVannaVolgaVolatilitySurface(asof, dates, vols[0], vols[1], vols[2], dc, cal, fxSpot, domYTS, forYTS));
                        }       
                        else
                        {
                            Utils.QL_FAIL("FXVolCurve: Can't find spec " + spec);
                        }
                    }
                }
                _vol.enableExtrapolation();

            }
            catch (Exception e) 
            {
                Utils.QL_FAIL("fx vol curve building failed :" + e.Message);
            }             
        }
    
        public FXVolatilityCurveSpec Spec()
        {
            return _spec;
        }

        public BlackVolTermStructure VolTermStructure()
        {
            return _vol;
        }
    }
}
