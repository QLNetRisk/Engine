using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using QLNet;
using QLRData;

namespace QLRAnalytics
{    
    public class ValuationEngine
    {
        private Date _today;
        private DateGrid _dg;
        private SimMarket _simMarket;

        public ValuationEngine(Date today, DateGrid dg, SimMarket simMarket)
        {
            _today = today;
            _dg = dg;
            _simMarket = simMarket;
        }

        public void BuildCube(Portfolio portfolio, List<ValuationCalculator> calculators)
        {
            QLNet.Utils.QL_REQUIRE(portfolio.Size() > 0, () => "ValuationEngine: Error portfolio is empty");

            ObservationMode.Mode om = ObservationMode.Mode.Disable;
            double updateTime = 0.0;
            double pricingTime = 0.0;
            double fixingTime = 0.0;
            int samples = 1;

            List<Date> dates = _dg.Dates();
            List<Trade> trades = portfolio.Trades();

            int numFRC = 0;

            // initialise state objects for each trade (required for path-dependent derivatives in particular)
            for (int i = 0; i < trades.Count; i++)
            {
                QLNet.Utils.QL_REQUIRE(trades[i].NpvCurrency() != "", () => "NPV currency not set for trade " + trades[i].Id());

                //DLOG("Initialise wrapper for trade " << trades[i]->id());
                trades[i].Instrument();// ->initialise(dates);

                // T0 values
                foreach (var calc in calculators)
                {
                    calc.CalculateT0(trades[i], i, _simMarket); //, outputCube);
                }                
                // TODO: Fix me!
                if (om == ObservationMode.Mode.Unregister)
                {
                    foreach (var leg in trades[i].Legs())
                    {
                        for (int n = 0; n < leg.Count; n++)
                        {
                            FloatingRateCoupon frc = leg[n] as FloatingRateCoupon;
                            if (frc != null)
                            {
                                //frc.unregisterWith(frc.index());
                                //trades[i]->instrument()->qlInstrument()->unregisterWith(frc);
                                //// Unregister with eval dates
                                //frc->unregisterWith(Settings::instance().evaluationDate());
                                //frc->index()->unregisterWith(Settings::instance().evaluationDate());
                                //trades[i]->instrument()->qlInstrument()->unregisterWith(Settings::instance().evaluationDate());
                            }
                        }
                    }
                }
            }

            //    LOG("Total number of swaps = " << portfolio->size());
            //    LOG("Total number of FRC = " << numFRC);

            //    simMarket_->fixingManager()->initialise(portfolio);

            Stopwatch timer = new Stopwatch();
            Stopwatch loopTimer = new Stopwatch();

            // We call Cube::samples() each time her to allow for dynamic stopping times
            // e.g. MC convergence tests
            for (int sample = 0; sample < samples; ++sample)
            {
                //updateProgress(sample, outputCube->samples());

                foreach (var trade in trades)
                {
                    //trade.Instrument().Reset();
                }                    

                // loop over Dates
                for (int i = 0; i < dates.Count; ++i)
                {
                    Date d = dates[i];

                    timer.Start();

                    //simMarket_->update(d);

                    // recalibrate models
                    //foreach (var b in modelBuilders_) 
                    //{
                    //    if (om == ObservationMode::Mode::Disable)
                    //    {
                    //        b.second->recalculate();
                    //    }

                    //    b.second->recalibrate();
                    //}

                    updateTime += timer.ElapsedMilliseconds / 1000.0;

                    // loop over trades
                    timer.Restart();
                    for (int j = 0; j < trades.Count; ++j)
                    {
                        var trade = trades[j];

                        // We can avoid checking mode here and always call updateQlInstruments()
                        if (om == ObservationMode.Mode.Disable)
                        {
                            trade.Instrument().update(); // ->updateQlInstruments();
                        }

                        foreach (var calc in calculators)
                        {
                            calc.Calculate(trade, j, _simMarket, d, i); //, outputCube, d, i, sample);
                        }
                    }
                    pricingTime += timer.ElapsedMilliseconds / 1000.0;
                }

                timer.Restart();
                //simMarket_->fixingManager()->reset();
                fixingTime += timer.ElapsedMilliseconds / 1000.0;
            }

            //simMarket_->reset();
            //updateProgress(outputCube->samples(), outputCube->samples());
            //LOG("ValuationEngine completed: loop " << setprecision(2) << loopTimer.elapsed() << " sec, "
            //                                               << "pricing " << pricingTime << " sec, "
            //                                               << "update " << updateTime << " sec "
            //                                               << "fixing " << fixingTime);
               
        }
    }
}
  

