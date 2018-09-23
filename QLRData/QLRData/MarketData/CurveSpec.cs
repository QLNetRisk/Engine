using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public abstract class CurveSpec
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

        public abstract CurveType BaseType();
        public abstract string SubName();

        public virtual string Name()
        {
            return BaseName() + "/" + SubName();
        }

        public string BaseName()
        {
            switch (BaseType())
            {
                case CurveType.Yield:
                    return "Yield";
                case CurveType.CapFloorVolatility:
                    return "CapFloorVolatility";
                case CurveType.SwaptionVolatility:
                    return "SwaptionVolatility";
                case CurveType.FX:
                    return "FX";
                case CurveType.FXVolatility:
                    return "FXVolatility";
                case CurveType.Security:
                    return "Security";
                case CurveType.Default:
                    return "Default";
                case CurveType.CDSVolatility:
                    return "CDSVolatility";
                case CurveType.Inflation:
                    return "Inflation";
                case CurveType.InflationCapFloorPrice:
                    return "InflationCapFloorPrice";
                case CurveType.Equity:
                    return "Equity";
                case CurveType.EquityVolatility:
                    return "EquityVolatility";
                case CurveType.BaseCorrelation:
                    return "BaseCorrelation";
                default:
                    return "N/A";
            }
        }
    }

    public class YieldCurveSpec : CurveSpec
    {
        protected string _ccy;
        protected string _curveConfigID;

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="ccy"></param>
        /// <param name="curveConfigID"></param>
        public YieldCurveSpec(string ccy, string curveConfigID)           
        {
            _ccy = ccy;
            _curveConfigID = curveConfigID;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public YieldCurveSpec()
        {

        }
        
        public override CurveType BaseType()
        {
            return CurveType.Yield;
        }
        public string Ccy()
        {
            return _ccy;
        }
        public string CurveConfigID()
        {
            return _curveConfigID;
        }
        public override string SubName()
        {
            return _ccy + "/" + _curveConfigID;
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
        public override string SubName()
        {
            return UnitCcy() + "/" + Ccy();                
        }
    }

    public class FXVolatilityCurveSpec : CurveSpec
    {
        private string _unitCcy;
        private string _ccy;
        private string _curveConfigID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FXVolatilityCurveSpec()
        {

        }
        
        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="unitCcy"></param>
        /// <param name="ccy"></param>
        /// <param name="curveConfigID"></param>
        public FXVolatilityCurveSpec(string unitCcy, string ccy, string curveConfigID)
        {
            _unitCcy = unitCcy;
            _ccy = ccy;
            _curveConfigID = curveConfigID;
        }

        public override CurveType BaseType()
        {
            return CurveType.FXVolatility;
        }
        public string UnitCcy()
        {
            return _unitCcy;
        }
        public string Ccy()
        {
            return _ccy;
        }
        public string CurveConfigID()
        {
            return _curveConfigID;
        }
        public override string SubName()
        {
            return UnitCcy() + "/" + Ccy() + "/" + CurveConfigID();
        }
    }       
}

