using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QLRData;
using QLRAnalytics;

namespace QLR
{
    class Qlr
    {
        private const string QLNET_RISK_VERSION = "1.0";

        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "-v" || args[0] == "--version"))
            {
                Console.WriteLine("QLR version " + QLNET_RISK_VERSION);
                return;
            }

            if (args.Length != 1)
            {
                Console.WriteLine("usage: QLR path/to/qlr.xml");
                return;
            }

            try
            {
                string inputFile = args[0];

                Parameters parameters = new Parameters();
                //parameters.FromFile("C:\\developer\\src\\cpp\\Engine\\Examples\\Example_FXFWD\\Input\\ore.xml");
                parameters.FromFile("C:\\developer\\git\\Engine\\Examples\\Example_FXFWD\\Input\\ore.xml");
                Directory.SetCurrentDirectory("C:\\developer\\git\\Engine\\Examples\\Example_FXFWD");

                App app = new App(parameters);
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
