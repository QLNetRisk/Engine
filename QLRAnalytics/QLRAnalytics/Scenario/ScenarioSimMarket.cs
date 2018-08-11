using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QLRData;


namespace QLRAnalytics
{
    public class ScenarioSimMarket : SimMarket
    {
        private Scenario _baseScenario;
        private Dictionary<RiskFactorKey, SimpleQuote> _simData;
        private ScenarioGenerator _scenarioGenerator;        

        public ScenarioSimMarket(Market initMarket, Conventions conventions, string configuration = Market.DefaultConfiguration) : base(conventions)
        {
            _numeraire = 1.0;
        }

        public override void Update(Date d)
        {
            QLNet.Utils.QL_REQUIRE(_scenarioGenerator != null, () => "ScenarioSimMarket::update: no scenario generator set");

            ObservationMode.Mode om = ObservationMode.Mode.Disable;
            //if (om == ObservationMode.Disable)
            //    ObservableSettings::instance().disableUpdates(false);
            //else if (om == ObservationMode.Defer)                
            //    ObservableSettings::instance().disableUpdates(true);

            Scenario scenario = _scenarioGenerator.Next(d);
            QLNet.Utils.QL_REQUIRE(scenario.AsOf() == d, () => "Invalid Scenario date " + scenario.AsOf() + ", expected " + d);

            _numeraire = scenario.GetNumeraire();

            if(d != Settings.evaluationDate())
            {
                Settings.setEvaluationDate(d);
            }
            else if (om == ObservationMode.Mode.Unregister)
            {
                // Due to some of the notification chains having been unregistered,
                // it is possible that some lazy objects might be missed in the case
                // that the evaluation date has not been updated. Therefore, we
                // manually kick off an observer notification from this level.
                // We have unit regression tests in OREAnalyticsTestSuite to ensure
                // the various ObservationMode settings return the anticipated results.
                
                //QLNet.IObservable obs = Settings.evaluationDate();
                //obs.notifyObservers();
            }

            ApplyScenario(scenario);

            // Observation Mode - key to update these before fixings are set
            if (om == ObservationMode.Mode.Disable)
            {
                Refresh();
                //ObservableSettings::instance().enableUpdates();
            }
            else if (om == ObservationMode.Mode.Defer)
            {
                //ObservableSettings::instance().enableUpdates();
            }

            // Apply fixings as historical fixings. Must do this before we populate ASD
            //_fixingManager.update(d);

            //if (asd_)
            //{
            //    // add additional scenario data to the given container, if required
            //    for (auto i : parameters_->additionalScenarioDataIndices())
            //    {
            //        boost::shared_ptr<QuantLib::Index> index;
            //        try
            //        {
            //            index = *iborIndex(i);
            //        }
            //        catch (...) {
            //    }
            //    try
            //    {
            //        index = *swapIndex(i);
            //    }
            //    catch (...) {
            //    }
            //    QL_REQUIRE(index != nullptr, "ScenarioSimMarket::update() index " << i << " not found in sim market");
            //    asd_->set(index->fixing(d), AggregationScenarioDataType::IndexFixing, i);
            //    }

            //    for (auto c : parameters_->additionalScenarioDataCcys())
            //    {
            //        if (c != parameters_->baseCcy())
            //            asd_->set(fxSpot(c + parameters_->baseCcy())->value(), AggregationScenarioDataType::FXSpot, c);
            //    }

            //    asd_->set(numeraire_, AggregationScenarioDataType::Numeraire);

            //    asd_->next();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        public ScenarioGenerator ScenarioGenerator()
        {
            return _scenarioGenerator;
        }

        public void SetScenarioGenerator(ScenarioGenerator scenarioGenerator)
        {
            _scenarioGenerator = scenarioGenerator;
        }

        public Scenario BaseScenario()
        {
            return _baseScenario;
        }
        
        private void ApplyScenario(Scenario scenario)
        {
            List<RiskFactorKey> keys = scenario.Keys();

            int count = 0;
            bool missingPoint = false;
            foreach(var key in keys)
            {
                if(!_simData.ContainsKey(key))
                {
                    //ALOG("simulation data point missing for key " << key);
                    missingPoint = true;
                }
                else
                {
                    // TODO: Include below?
                    //if (filter_->allow(key)) 
                    _simData[key].setValue(scenario.Get(key));
                    count++;
                }                
            }

            QLNet.Utils.QL_REQUIRE(!missingPoint, () => "simulation data points missing from scenario, exit.");

            if (count != _simData.Count)
            {
                //ALOG("mismatch between scenario and sim data size, " << count << " vs " << simData_.size());
                foreach (var key in _simData.Keys)
                {
                    if (!scenario.Has(key))
                    {
                        //ALOG("Key " << it.first << " missing in scenario");
                    }                        
                }
                QLNet.Utils.QL_FAIL("mismatch between scenario and sim data size, exit.");
            }

            // update market asof date
            _asof = scenario.AsOf();
        }
    }
}
