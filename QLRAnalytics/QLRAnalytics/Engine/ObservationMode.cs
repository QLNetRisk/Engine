using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLRAnalytics
{
    public static class ObservationMode
    {        
        public enum Mode
        {
            None = 1,
            Disable = 2,
            Defer = 3,
            Unregister
        }

        private static Mode _mode = Mode.None;        

        public static Mode GetMode()
        {
            return _mode;
        }

        public static void SetMode(Mode s)
        {
            _mode = s;
        }

        public static void SetMode(string s)
        {
            if (s == "None") _mode = Mode.None;
            else if (s == "Disable") _mode = Mode.Disable;
            else if (s == "Defer") _mode = Mode.Defer;
            else if (s == "Unregister") _mode = Mode.Unregister;
            else
            {
                QLNet.Utils.QL_FAIL("Invalid ObserverMode string " + s);
            }
        }
    }
}
