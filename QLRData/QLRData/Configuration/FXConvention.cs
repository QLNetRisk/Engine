using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class FXConvention : Convention
    {
        private int _spotDays;
        private Currency _sourceCurrency;
        private Currency _targetCurrency;
        private double _pointsFactor;
        private Calendar _advanceCalendar;
        private bool _spotRelative;

        // Strings to store the inputs
        private string _strSpotDays;
        private string _strSourceCurrency;
        private string _strTargetCurrency;
        private string _strPointsFactor;
        private string _strAdvanceCalendar;
        private string _strSpotRelative;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FXConvention()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="spotDays"></param>
        /// <param name="sourceCurrency"></param>
        /// <param name="targetCurrency"></param>
        /// <param name="pointsFactor"></param>
        /// <param name="advanceCalendar"></param>
        /// <param name="spotRelative"></param>
        public FXConvention(string id, string spotDays, string sourceCurrency, string targetCurrency, string pointsFactor, string advanceCalendar = "", string spotRelative = "") 
            : base(id, Type.FX)
        {
            _strSpotDays = spotDays;
            _strSourceCurrency = sourceCurrency;
            _strTargetCurrency = targetCurrency;
            _strPointsFactor = pointsFactor;
            _strAdvanceCalendar = advanceCalendar;
            _strSpotRelative = spotRelative;

            Build();
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "FX");
            _type = Type.FX;
            _id = GetChildValue(node, "Id", true);

            // Get string values from xml
            _strSpotDays = GetChildValue(node, "SpotDays", true);
            _strSourceCurrency = GetChildValue(node, "SourceCurrency", true);
            _strTargetCurrency = GetChildValue(node, "TargetCurrency", true);
            _strPointsFactor = GetChildValue(node, "PointsFactor", true);
            _strAdvanceCalendar = GetChildValue(node, "AdvanceCalendar", false);
            _strSpotRelative = GetChildValue(node, "SpotRelative", false);

            Build();
        }

        public virtual void Build()
        {
            _spotDays = Parsers.ParseInteger(_strSpotDays);
            _sourceCurrency = Parsers.ParseCurrency(_strSourceCurrency);
            _targetCurrency = Parsers.ParseCurrency(_strTargetCurrency);
            _pointsFactor = Parsers.ParseDouble(_strPointsFactor);
            _advanceCalendar = _strAdvanceCalendar == "" ? new NullCalendar() : Parsers.ParseCalendar(_strAdvanceCalendar);
            _spotRelative = _strSpotRelative == "" ? true : Parsers.ParseBool(_strSpotRelative);
        }

        public int SpotDays()
        {
            return _spotDays;
        }
        public Currency SourceCurrency()
        {
            return _sourceCurrency;
        }
        public Currency TargetCurrency()
        {
            return _targetCurrency;
        }
        public double PointsFactor()
        {
            return _pointsFactor;
        }
        public Calendar AdvanceCalendar()
        {
            return _advanceCalendar;
        }
        public bool SpotRelative()
        {
            return _spotRelative;
        }
    }
}
