using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class Portfolio
    {
        private List<Trade> _trades = new List<Trade>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public Portfolio()
        {

        }

        public void Add(Trade trade)
        {
            _trades.Add(trade);
        }

        public bool Has(string tradeId)
        {
            foreach (var trade in _trades)
            {
                if (trade.Id() == tradeId) return true;                
            }

            return false;
        }

        public void Clear()
        {
            _trades.Clear();
        }

        public void Reset()
        {
            //LOG("Reset portfolio of size " << trades_.size());
            foreach(var trade in _trades)
            {
                trade.Reset();
            }
        }

        public bool Remove(string tradeId)
        {
            foreach (var trade in _trades)
            {
                if (trade.Id() == tradeId) _trades.Remove(trade);
                return true;
            }

            return false;
        }

        public List<Trade> Trades()
        {
            return _trades;
        }

        public int Size()
        {
            return _trades.Count;
        }

        public Date Maturity()
        {
            throw new NotImplementedException();
        }

        //private void Load(XmlDocument doc, TradeFactory)
        //{

        //}

        //private void Load(string fileName, TradeFactory)
        //{

        //}

        //private void LoadFromXMLString(string xmlString, TradeFactory)
        //{

        //}

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }        

        //public void Build(EngineFactory )
        //{

        //}
    }
}
