using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QLRAnalytics;
using QLRData;
using ExcelDna.Integration;

namespace QLRExcel
{
    public class QlrExcel
    {
        private enum ExcelTaskStatus
        {
            IN_PROGRESS,
            COMPLETED
        }
        
        [ExcelFunction(Category = "QLNetRisk Excel", Description = "Run app")]
        public static object App()
        {
            string message = ExcelTaskStatus.IN_PROGRESS.ToString();

            try
            {                
                Parameters parameters = new Parameters();
                //parameters.FromFile("C:\\developer\\src\\cpp\\Engine\\Examples\\Example_FXFWD\\Input\\ore.xml");
                parameters.FromFile("C:\\developer\\git\\Engine\\Examples\\Example_FXFWD\\Input\\ore.xml");
                Directory.SetCurrentDirectory("C:\\developer\\git\\Engine\\Examples\\Example_FXFWD");

                App app = new App(parameters);
                app.Run();

                message = ExcelTaskStatus.COMPLETED.ToString();
            }
            catch (Exception ex)
            {
                message = ExcelError.ExcelErrorNA.ToString();
            }

            return message;
        }
    }
}
