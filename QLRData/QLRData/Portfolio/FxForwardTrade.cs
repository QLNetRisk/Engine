using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QuantExt;
using Leg = System.Collections.Generic.List<QLNet.CashFlow>;

namespace QLRData
{
    public class FxForwardTrade : Trade
    {
        private string _maturityDate;
        private string _boughtCurrency;
        private double _boughtAmount;
        private string _soldCurrency;
        private double _soldAmount;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FxForwardTrade() : base("FxForward")
        {
            _boughtAmount = 0.0;
            _soldAmount = 0.0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maturityDate"></param>
        /// <param name="boughtCurrency"></param>
        /// <param name="boughtAmount"></param>
        /// <param name="soldCurrency"></param>
        /// <param name="soldAmount"></param>
        public FxForwardTrade(string maturityDate, string boughtCurrency, double boughtAmount, string soldCurrency, double soldAmount) : base("FxForward")
        {
            _maturityDate = maturityDate;
            _boughtCurrency = boughtCurrency;
            _boughtAmount = boughtAmount;
            _soldCurrency = soldCurrency;
            _soldAmount = soldAmount;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maturityDate"></param>
        /// <param name="boughtCurrency"></param>
        /// <param name="boughtAmount"></param>
        /// <param name="soldCurrency"></param>
        /// <param name="soldAmount"></param>
        public FxForwardTrade(Date maturityDate, string boughtCurrency, double boughtAmount, string soldCurrency, double soldAmount) : this(maturityDate.ToString(), boughtCurrency, boughtAmount, soldCurrency, soldAmount)
        {
            
        }

        public override void Build(EngineFactory engineFactory)
        {            
            Currency boughtCcy = Parsers.ParseCurrency(_boughtCurrency);
            Currency soldCcy = Parsers.ParseCurrency(_soldCurrency);
            Date maturityDate = Parsers.ParseDate(_maturityDate);

            //QL_REQUIRE(tradeActions().empty(), "TradeActions not supported for FxForward");

            try
            {
                //DLOG("Build FxForward with maturity date " << QuantLib::io::iso_date(maturityDate));

                QLNet.Instrument instrument = new FxForward(_boughtAmount, boughtCcy, _soldAmount, soldCcy, maturityDate, false);

                //instrument_.reset(new VanillaInstrument(instrument));

                _npvCurrency = _soldCurrency;
                _notional = _soldAmount;
                _maturity = maturityDate;

            }
            catch (Exception ex)
            {

                //_instrument.reset();
                throw;
            }
            
            SimpleCashFlow cf1 = new SimpleCashFlow(_boughtAmount, maturityDate);
            SimpleCashFlow cf2 = new SimpleCashFlow(_soldAmount, maturityDate);
            
            _legs = new List<List<CashFlow>>() { new List<CashFlow> { cf1, cf2 } };

            _legCurrencies = new List<string>{ _boughtCurrency, _soldCurrency};
            _legPayers = new List<bool> { false, true };

            // set Pricing engine
            EngineBuilder builder = engineFactory.Builder(_tradeType);
            QLNet.Utils.QL_REQUIRE(builder != null, () => "No builder found for " + _tradeType);
            FxForwardEngineBuilder fxBuilder = builder as FxForwardEngineBuilder;            

            _instrument.setPricingEngine(fxBuilder.Engine(boughtCcy, soldCcy));

            //DLOG("FxForward leg 0: " << legs_[0][0]->date() << " " << legs_[0][0]->amount());
            //DLOG("FxForward leg 1: " << legs_[1][0]->date() << " " << legs_[1][0]->amount());
        }        

        public string MaturityDate()
        {
            return _maturityDate;
        }

        public string BoughtCurrency()
        {
            return _boughtCurrency;
        }

        public double BoughtAmount()
        {
            return _boughtAmount;
        }

        public string SoldCurrency()
        {
            return _soldCurrency;
        }

        public double SoldAmount()
        {
            return _soldAmount;
        }        
    }
}
