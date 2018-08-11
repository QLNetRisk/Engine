using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class EngineData
    {
        private Dictionary<string, string> _model;
        private Dictionary<string, Dictionary<string, string>> _modelParams;
        private Dictionary<string, string> _engine;
        private Dictionary<string, Dictionary<string, string>> _engineParams;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public EngineData()
        {

        }

        public void Clear()
        {
            _model.Clear();
            _modelParams.Clear();
            _engine.Clear();
            _engineParams.Clear();
        }

        public bool HasProduct(string productName)
        {
            return _model.ContainsKey(productName);            
        }

        public string Model(string productName)
        {
            return _model[productName];
        }

        public Dictionary<string, string> ModelParameters(string productName)
        {
            return _modelParams[productName];
        }

        public string Engine(string productName)
        {
            return _engine[productName];
        }

        public Dictionary<string, string> EngineParameters(string productName)
        {
            return _engineParams[productName];
        }
    }
}