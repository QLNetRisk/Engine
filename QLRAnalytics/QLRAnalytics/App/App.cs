using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using QLNet;
using QLRData;

namespace QLRAnalytics
{
    public class App
    {
        protected string _endl;
        protected int _tab;

        protected Date _asof;
        protected StringBuilder _out;
        protected Parameters _parameters;
        protected bool _writeInitialReports;
        protected bool _simulate;
        protected bool _buildSimMarket;
        protected bool _xva;
        protected bool _writeDIMReport;
        protected bool _sensitivity;
        protected bool _stress;
        protected bool _parametricVar;
        protected bool _writeBaseScenario;

        protected Market _market;
        protected Portfolio _portfolio = new Portfolio();
        protected Conventions _conventions = new Conventions();
        protected TodaysMarketParameters _marketParameters;

        protected ScenarioSimMarket _simMarket;
        protected Portfolio _simPortfolio;

        protected DateGrid _grid;
        protected int _samples;

        protected int _cubeDepth;
        //protected NPVCube _cube;
        //protected AggregationScenarioData _scenarioData;
        //protected PostProcess _postProcess;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public App(Parameters parameters)
        {
            _parameters = parameters;            
            _cubeDepth = 0;
            _asof = Parsers.ParseDate(parameters.Get("setup", "asofDate"));
            Settings.setEvaluationDate(_asof);

            _tab = 40;
            _endl = "\n";
            _out = new StringBuilder();
        }

        public int Run()
        {
            Stopwatch timer = new Stopwatch();            

            try
            {
                SetupLog();
                _out.Append("ORE starting" + _endl);
                //LOG("ORE starting");
                ReadSetup();
               
                /*************
                * Load Conventions
                */
                _out.Append(("Conventions... ").PadRight(_tab));
                GetConventions();
                _out.Append("OK" + _endl);

                /*********
                * Build Markets
                */                
                _out.Append(("Market... ").PadRight(_tab));
                GetMarketParameters();
                BuildMarket();
                _out.Append("OK" + _endl);

                /************************
                *Build Pricing Engine Factory
                */
                _out.Append(("Engine factory... ").PadRight(_tab));
                //EngineFactory factory = BuildEngineFactory(_market);
                _out.Append("OK" + _endl);

                /******************************
                * Load and Build the Portfolio
                */
                _out.Append(("Portfolio... ").PadRight(_tab));
                //_portfolio = BuildPortfolio(factory);
                _out.Append("OK" + _endl);

                /******************************
                * Write initial reports
                */
                _out.Append(("Write Reports... ").PadRight(_tab));                
                //WriteInitialReports();
                _out.Append("OK" + _endl);

                /**************************
                * Write base scenario file
                */
                _out.Append(("Write Base Scenario... ").PadRight(_tab));                
                if (_writeBaseScenario)
                {
                    //WriteBaseScenario();
                    _out.Append("OK" + _endl);
                }
                else
                {
                    //LOG("skip base scenario");
                    _out.Append("SKIP" + _endl);
                }

                /**********************
                * Sensitivity analysis
                */
                if (_sensitivity)
                {
                    //RunSensitivityAnalysis();
                }
                else
                {
                    //LOG("skip sensitivity analysis");
                    _out.Append(("Sensitivity... ").PadRight(_tab));
                    _out.Append("SKIP" + _endl);
                }

                /****************
                * Stress testing
                */
                if (_stress)
                {
                    //RunStressTest();
                }
                else
                {
                    //LOG("skip stress test");
                    _out.Append(("Stress testing... ").PadRight(_tab));
                    _out.Append("SKIP" + _endl);
                }

                /****************
                 * Parametric VaR
                 */
                if (_parametricVar)
                {
                    //RunParametricVar();
                }
                else
                {
                    //LOG("skip parametric var");
                    _out.Append(("Parametric VaR... ").PadRight(_tab));
                    _out.Append("SKIP" + _endl);
                }

                /******************************************
                 * Simulation: Scenario and Cube Generation
                 */
                if (_simulate)
                {
                    GenerateNPVCube();
                }
                else
                {
                    //LOG("skip simulation");                    
                    _out.Append(("Simulation... ").PadRight(_tab));
                    _out.Append("SKIP" + _endl);
                }

                /*****************************
                * Aggregation and XVA Reports
                */
                _out.Append(("Aggregation and XVA Reports... ").PadRight(_tab));
                //if (_xva)
                //{

                //    // We reset this here because the date grid building below depends on it.
                //    Settings.setEvaluationDate(_asof);

                //    // Use pre-generated cube
                //    if (!_cube)
                //        LoadCube();

                //    QLNet.Utils.QL_REQUIRE(cube_->numIds() == portfolio_->size(),
                //               "cube x dimension (" << cube_->numIds() << ") does not match portfolio size ("
                //                                    << portfolio_->size() << ")");

                //    // Use pre-generared scenarios
                //    if (!_scenarioData)
                //        LoadScenarioData();

                //    QL_REQUIRE(scenarioData_->dimDates() == cube_->dates().size(),
                //               "scenario dates do not match cube grid size");
                //    QL_REQUIRE(scenarioData_->dimSamples() == cube_->samples(),
                //               "scenario sample size does not match cube sample size");

                //    runPostProcessor();
                //    out_ << "OK" << endl;
                //    out_ << setw(tab_) << left << "Write Reports... " << flush;
                //    writeXVAReports();
                //    if (writeDIMReport_)
                //        writeDIMReport();
                //    out_ << "OK" << endl;
                //}
                //else
                //{
                //    //LOG("skip XVA reports");
                //    _out.Append("SKIP" + _endl);
                //}

                /*****************************
                * Additional reports
                */
                //WriteAdditionalReports();
            }
            catch(Exception ex)
            {
                //ALOG("Error: " << e.what());
                _out.Append(("Error: " + ex.ToString()).PadRight(_tab));                
                return 1;
            }

            _out.Append(("run time: " + timer.ElapsedMilliseconds / 1000.0 + " sec").PadRight(_tab));
            _out.Append("App done." + _endl);            

            //LOG("App done.");
            return 0;
        }

        public virtual void ReadSetup()
        {
            _parameters.Log();

            if (_parameters.Has("setup", "observationModel"))
            {
                string om = _parameters.Get("setup", "observationModel");
                ObservationMode.SetMode(om);
                //LOG("Observation Mode is " << om);
            }

            _writeInitialReports = true;
            _simulate = (_parameters.HasGroup("simulation") && _parameters.Get("simulation", "active") == "Y") ? true : false;
            _buildSimMarket = true;
            _xva = (_parameters.HasGroup("xva") && _parameters.Get("xva", "active") == "Y") ? true : false;
            _writeDIMReport = (_parameters.HasGroup("xva") && _parameters.Has("xva", "dim") && Parsers.ParseBool(_parameters.Get("xva", "dim"))) ? true : false;
            _sensitivity = (_parameters.HasGroup("sensitivity") && _parameters.Get("sensitivity", "active") == "Y") ? true : false;
            _stress = (_parameters.HasGroup("stress") && _parameters.Get("stress", "active") == "Y") ? true : false;
            _parametricVar = (_parameters.HasGroup("parametricVar") && _parameters.Get("parametricVar", "active") == "Y") ? true : false;
            _writeBaseScenario = (_parameters.HasGroup("baseScenario") && _parameters.Get("baseScenario", "active") == "Y") ? true : false;
        }

        /// <summary>
        /// set up logging
        /// </summary>
        public void SetupLog()
        {
            string outputPath = _parameters.Get("setup", "outputPath");
            string logFile = outputPath + "/" + _parameters.Get("setup", "logFile");
            int logMask = 15; // Default level

            // Get log mask if available
            if(_parameters.Has("setup", "logMask"))
            {
                logMask = Parsers.ParseInteger(_parameters.Get("setup", "logMask"));
            }

            if(!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            QLNet.Utils.QL_REQUIRE(File.GetAttributes(outputPath) == FileAttributes.Directory, () => "output path '" + outputPath + "' is not a directory");

            //Log::instance().registerLogger(boost::make_shared<FileLogger>(logFile));
            //Log::instance().setMask(logMask);
            //Log::instance().switchOn();
        }

        /// <summary>
        /// load market conventions
        /// </summary>
        public void GetConventions()
        {
            if (_parameters.Has("setup", "conventionsFile") && _parameters.Get("setup", "conventionsFile") != "")
            {
                string inputPath = _parameters.Get("setup", "inputPath");
                string conventionsFile = inputPath + "/" + _parameters.Get("setup", "conventionsFile");
                _conventions.FromFile(conventionsFile);
            }
            else
            {
                //WLOG("No conventions file loaded");
            }
        }
        /// <summary>
        /// load market parameters
        /// </summary>
        public void GetMarketParameters()
        {
            if(_parameters.Has("setup", "marketConfigFile") && _parameters.Get("setup", "marketConfigFile") != "")
            {
                string inputPath = _parameters.Get("setup", "inputPath");
                string marketConfigFile = inputPath + "/" + _parameters.Get("setup", "marketConfigFile");
                //_marketParameters
            }
        }
        /// <summary>
        /// build today's market
        /// </summary>
        public void BuildMarket()
        {

        }

        /// <summary>
        /// build engine factory for a given market
        /// </summary>
        /// <returns></returns>
        //public virtual EngineFactory BuildEngineFactory(Market market, string groupName = "setup")
        //{

        //}

        /// <summary>
        /// build trade factory
        /// </summary>
        /// <returns></returns>
        //public virtual TradeFactory BuildTradeFactory()
        //{

        //}

        //build portfolio for a given market       
        //public Portfolio BuildPortfolio(EngineFactory factory)
        //{

        //}

        /// <summary>
        /// generate NPV cube
        /// </summary>
        public void GenerateNPVCube()
        {

        }

        /// <summary>
        /// get an instance of an aggregationScenarioData class
        /// </summary>
        public virtual void InitAggregationScenarioData()
        {

        }

        /// <summary>
        /// get an instance of a cube class
        /// </summary>
        public virtual void InitCube()
        {

        }

        /// <summary>
        /// build an NPV cube
        /// </summary>
        public virtual void BuildNPVCube()
        {

        }
        //! load simMarketData
        //public ScenarioSimMarketParameters GetSimMarketData()      
        //! load scenarioGeneratorData
        //public ScenarioGeneratorData GetScenarioGeneratorData();
        //! build scenarioGenerator
        //public virtual ScenarioGenerator BuildScenarioGenerator(Market market, ScenarioSimMarketParameters simMarketData, ScenarioGeneratorData sgd);

        /// <summary>
        /// load in scenarioData
        /// </summary>
        public virtual void LoadScenarioData()
        {

        }

        /// <summary>
        /// load in cube
        /// </summary>
        public virtual void LoadCube()
        {

        }
        //! run postProcessor to generate reports from cube
        //public void RunPostProcessor();
        //! run sensitivity analysis and write out reports
        //public virtual void RunSensitivityAnalysis();
        //! run stress tests and write out report
        //public virtual void RunStressTest();
        //! run parametric var and write out report
        //public void RunParametricVar();
        //! write out initial (pre-cube) reports
        //public void WriteInitialReports();
        //! write out XVA reports
        //public void WriteXVAReports();
        //! write out DIM reports
        //public void WriteDIMReport();
        //! write out cube
        //public void WriteCube();
        //! write out scenarioData
        //public void WriteScenarioData();
        //! write out base scenario
        //public void WriteBaseScenario();
        //! load in nettingSet data
        //public NettingSetManager initNettingSetManager();

        //! write out additional reports
        //public virtual void writeAdditionalReports() { }
    }
}
