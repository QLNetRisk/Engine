using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRAnalytics
{
    public class ScenarioPathGenerator : ScenarioGenerator
    {
        protected Date _today;
        protected List<Date> _dates;
        protected TimeGrid _timeGrid;
        protected int _pathStep;
        protected List<Scenario> _path;

        public ScenarioPathGenerator(Date today, List<Date> dates, TimeGrid timeGrid)
        {
            _today = today;
            _dates = dates;
            _timeGrid = timeGrid;
        }

        public override Scenario Next(Date d)
        {
            if(d == _dates.First())
            {
                _path = NextPath();
                _pathStep = 0;
            }
            QLNet.Utils.QL_REQUIRE(_pathStep < _dates.Count && d == _dates[_pathStep], () => "Step mismatch");
            return _path[_pathStep++];
        }

        protected virtual List<Scenario> NextPath()
        {
            return new List<Scenario>(); // TODO!
        }
    }
}
