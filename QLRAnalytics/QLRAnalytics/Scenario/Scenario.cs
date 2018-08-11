using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRAnalytics
{
    public abstract class Scenario
    {
        public virtual Date AsOf()
        {
            throw new NotImplementedException();
        }

        public virtual string Label()
        {
            throw new NotImplementedException();
        }

        public virtual void SetLabel(string label)
        {
            throw new NotImplementedException();
        }

        public virtual double GetNumeraire()
        {
            throw new NotImplementedException();
        }

        public virtual void SetNumeraire(double numeraire)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether this scenario provides the data for the given key
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public virtual bool Has(RiskFactorKey key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Risk factor keys for which this scenario provides data
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public virtual List<RiskFactorKey> Keys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add an element to the scenario
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void Add(RiskFactorKey key, double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an element from the scenario
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual double Get(RiskFactorKey key)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleScenario : Scenario
    {
        private Date _asof;
        private string _label;
        private double _numeraire;
        private List<RiskFactorKey> _keys;
        private Dictionary<RiskFactorKey, double> _data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asof"></param>
        /// <param name="label"></param>
        /// <param name="numeraire"></param>
        public SimpleScenario(Date asof, string label = "", double numeraire = 0.0)
        {
            _asof = asof;
            _label = label;
            _numeraire = numeraire;
        }

        public override Date AsOf()
        {
            return _asof;
        }

        public override string Label()
        {
            return _label;
        }

        public override void SetLabel(string label)
        {
            _label = label;
        }

        public override double GetNumeraire()
        {
            return _numeraire;
        }

        public override void SetNumeraire(double numeraire)
        {
            _numeraire = numeraire;
        }

        public override bool Has(RiskFactorKey key)
        {
            return _data.ContainsKey(key);
        }

        public override List<RiskFactorKey> Keys()
        {
            return _data.Keys.ToList();
        }

        public override void Add(RiskFactorKey key, double value)
        {
            _data[key] = value;
            // TODO: Do we need more logic as below?
            //if(find(keys_.begin(), keys_.end(), key) == keys_.end())
            //keys_.emplace_back(key);
        }

        public override double Get(RiskFactorKey key)
        {
            QLNet.Utils.QL_REQUIRE(_data.ContainsKey(key), () => "Scenario does not provide data for key " + key);            
            return _data[key];
        }
    }

    public class RiskFactorKey
    {
        public enum KeyType
        {
            None,
            DiscountCurve,
            YieldCurve,
            IndexCurve,
            SwaptionVolatility,
            OptionletVolatility,
            FXSpot,
            FXVolatility,
            EquitySpot,
            EquityForecastCurve,
            EquityVolatility,
            DividendYield,
            SurvivalProbability,
            RecoveryRate,
            CDSVolatility,
            BaseCorrelation,
            CPIIndex,
            ZeroInflationCurve,
            YoYInflationCurve
        };

        public string ParseToString(KeyType type)
        {
            switch (type)
            {
                case KeyType.DiscountCurve:
                    return "DiscountCurve";
                case KeyType.YieldCurve:
                    return "YieldCurve";
                case KeyType.FXSpot:
                    return "FXSpot";
                default:
                    return "?";
            }
        }
    }
}
