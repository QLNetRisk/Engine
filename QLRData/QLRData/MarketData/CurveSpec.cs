using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class CurveSpec
    {
        //! Supported curve types
        public enum CurveType
        {
            Yield,
            CapFloorVolatility,
            SwaptionVolatility,
            FX,
            FXVolatility,
            Default,
            CDSVolatility,
            Inflation,
            InflationCapFloorPrice,
            Equity,
            EquityVolatility,
            Security,
            BaseCorrelation
        };

        /// <summary>
        /// Default constructor
        /// </summary>
        public CurveSpec()
        {

        }

        public virtual CurveType BaseType()
        {
            throw new NotImplementedException();
        }
    }

    public class FXSpotSpec : CurveSpec
    {
        private string _unitCcy;
        private string _ccy;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FXSpotSpec()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitCcy"></param>
        /// <param name="ccy"></param>
        public FXSpotSpec(string unitCcy, string ccy)
        {
            _unitCcy = unitCcy;
            _ccy = ccy;
        }

        public override CurveType BaseType()
        {
            return CurveType.FX;
        }
        public string UnitCcy()
        {
            return _unitCcy;
        }
        public string Ccy()
        {
            return _ccy;
        }

        public string SubName()
        {
            return UnitCcy() + "/" + Ccy();                
        }
    }
}

