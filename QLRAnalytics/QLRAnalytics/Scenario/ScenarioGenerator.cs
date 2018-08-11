using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRAnalytics
{
    public class ScenarioGenerator
    {
        public ScenarioGenerator()
        {

        }

        public virtual Scenario Next(Date d)
        {
            throw new NotImplementedException();
        }

        public virtual void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
