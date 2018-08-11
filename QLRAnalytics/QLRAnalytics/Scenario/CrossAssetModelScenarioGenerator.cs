using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QLRData;
using QuantExt;

namespace QLRAnalytics
{
    public class CrossAssetModelScenarioGenerator<GSG> : ScenarioPathGenerator where GSG : IRNG
    {
        private List<CalibratedModel> _models;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrossAssetModelScenarioGenerator(List<CalibratedModel> models, GSG multiPathGenerator, Date today, DateGrid grid, Market initMarket, string configuration = Market.DefaultConfiguration) : base(today, grid.Dates(), null)
        {
            _models = models;

        }

        public override Scenario Next(Date d)
        {
            return base.Next(d);
        }

        public override void Reset()
        {
            base.Reset();
        }

        protected override List<Scenario> NextPath()
        {
            return base.NextPath();
        }
    }
}
