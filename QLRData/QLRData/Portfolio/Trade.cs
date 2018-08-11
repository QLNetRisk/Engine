using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using Leg = System.Collections.Generic.List<QLNet.CashFlow>;

namespace QLRData
{
    public class Trade
    {
        protected string _tradeType;
        protected string _id;
        protected string _npvCurrency;
        protected QLNet.Instrument _instrument;
        protected List<Leg> _legs;
        protected List<string> _legCurrencies;
        protected List<bool> _legPayers;
        protected double _notional;
        protected Date _maturity;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tradeType"></param>
        public Trade(string tradeType)
        {
            _tradeType = tradeType;
        }

        public virtual void Build(EngineFactory engineFactory) 
        {
            throw new NotImplementedException();
        }

        public void AddPayment()
        {

        }

        public string Id()
        {
            return _id;
        }

        public string NpvCurrency()
        {
            return _npvCurrency;
        }

        public QLNet.Instrument Instrument()
        {
            return _instrument;
        }

        public List<Leg> Legs()
        {
            return _legs;
        }

        public void Reset()
        {
            _legs.Clear();
            _legCurrencies.Clear();
            _legPayers.Clear();
            _npvCurrency = "";
            _notional = 0.0;
            _maturity = new Date();
            //_tradeActions            
        }
    }
}
