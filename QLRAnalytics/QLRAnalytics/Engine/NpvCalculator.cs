using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;
using QLRData;

namespace QLRAnalytics
{
    public class NpvCalculator : ValuationCalculator
    {
        private string _baseCcyCode;
        private int _index;

        public NpvCalculator(string baseCcyCode, int index = 0)
        {
            _baseCcyCode = baseCcyCode;
            _index = index;
        }

        public override void Calculate(Trade trade, int tradeIndex, SimMarket simMarket, Date date, int dateIndex)
        {
            NPV(trade, simMarket);
        }

        public override void CalculateT0(Trade trade, int tradeIndex, SimMarket simMarket)
        {
            NPV(trade, simMarket);
        }

        public double NPV(Trade trade, SimMarket simMarket)
        {
            double npv = 0.0;

            try
            {
                double fx = simMarket.FxSpot(trade.NpvCurrency() + _baseCcyCode).link.value();
                double numeraire = simMarket.Numeraire();
                npv = trade.Instrument().NPV() * fx / numeraire;
            }
            catch(Exception ex)
            {
                //ALOG("Failed to price trade " << trade->id() << " : " << e.what());
                npv = 0;
            }

            return npv;
        }
    }
}
