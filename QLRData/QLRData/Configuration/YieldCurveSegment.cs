using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class YieldCurveSegment : XmlSerializable
    {
        // Supported segment types
        public enum Type
        {
            Zero,
            ZeroSpread,
            Discount,
            Deposit,
            FRA,
            Future,
            OIS,
            Swap,
            AverageOIS,
            TenorBasis,
            TenorBasisTwo,
            FXForward,
            CrossCcyBasis
        };

        protected List<string> _quotes;

        // TODO: why type and typeID?
        private Type _type;
        private string _typeID;
        private string _conventionsID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public YieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        protected YieldCurveSegment(string typeID, string conventionsID, List<string> quotes)
        {
            _quotes = quotes;
            _type = ParseYieldCurveSegement(typeID);
            _typeID = typeID;
            _conventionsID = conventionsID;
        }  

        public override void FromXML(XmlNode node)
        {
            _typeID = GetChildValue(node, "Type", true);
            _type = ParseYieldCurveSegement(_typeID);
            _conventionsID = GetChildValue(node, "Conventions", false);
        }

        public override void ToXML(XmlDocument doc)
        {
            throw new NotImplementedException();
        }

        public Type ParseYieldCurveSegement(string s)
        {
            if (s == "Zero")
                return YieldCurveSegment.Type.Zero;
            else if (s == "Zero Spread")
                return YieldCurveSegment.Type.ZeroSpread;
            else if (s == "Discount")
                return YieldCurveSegment.Type.Discount;
            else if (s == "Deposit")
                return YieldCurveSegment.Type.Deposit;
            else if (s == "FRA")
                return YieldCurveSegment.Type.FRA;
            else if (s == "Future")
                return YieldCurveSegment.Type.Future;
            else if (s == "OIS")
                return YieldCurveSegment.Type.OIS;
            else if (s == "Swap")
                return YieldCurveSegment.Type.Swap;
            else if (s == "Average OIS")
                return YieldCurveSegment.Type.AverageOIS;
            else if (s == "Tenor Basis Swap")
                return YieldCurveSegment.Type.TenorBasis;
            else if (s == "Tenor Basis Two Swaps")
                return YieldCurveSegment.Type.TenorBasisTwo;
            else if (s == "FX Forward")
                return YieldCurveSegment.Type.FXForward;
            else if (s == "Cross Currency Basis Swap")
                return YieldCurveSegment.Type.CrossCcyBasis;
            else
            {
                Utils.QL_FAIL("Yield curve segment type " + s + " not recognized");
                throw new Exception();
            }            
        }

        public Type CurveSegmentType()
        {
            return _type;
        }
        public string TypeID()
        {
            return _typeID;
        }   
        public string ConventionsID()
        {
            return _conventionsID;
        }        
        public virtual List<string> Quotes()
        {
            return _quotes;
        }

        public virtual void Accept(IAcyclicVisitor v)
        {
            if (v != null) v.visit(this);
            else Utils.QL_FAIL("Not a YieldCurveSegment visitor.");

        }
    }

    public class DirectYieldCurveSegment : YieldCurveSegment
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectYieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        public DirectYieldCurveSegment(string typeID, string conventionsID, List<string> quotes)
        {

        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "Direct");
            base.FromXML(node);
            _quotes = GetChildrenValues(node, "Quotes", "Quote", true);
        }

        //XMLNode* DirectYieldCurveSegment::toXML(XMLDocument& doc)
        //{
        //    XMLNode* node = YieldCurveSegment::toXML(doc);
        //    XMLUtils::setNodeName(doc, node, "Direct");
        //    XMLUtils::addChildren(doc, node, "Quotes", "Quote", quotes_);
        //    return node;
        //}

        public override void Accept(IAcyclicVisitor v)
        {            
            if (v != null) v.visit(this);
            else base.Accept(v);
        }
    }

    public class SimpleYieldCurveSegment : YieldCurveSegment
    {
        private string _projectionCurveID;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public SimpleYieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        /// <param name="projectionCurveID"></param>
        public SimpleYieldCurveSegment(string typeID, string conventionsID, List<string> quotes, string  projectionCurveID = "")
            : base(typeID, conventionsID, quotes)
        {
            _projectionCurveID = projectionCurveID;
        }                    
        
        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "Simple");
            base.FromXML(node);
            _quotes = GetChildrenValues(node, "Quotes", "Quote", true);
            _projectionCurveID = GetChildValue(node, "ProjectionCurve", false);
        }

        //public override XmlNode ToXML(XmlDocument doc)
        //{
        //    XMLNode* node = YieldCurveSegment::toXML(doc);
        //    XMLUtils::setNodeName(doc, node, "Simple");
        //    XMLUtils::addChildren(doc, node, "Quotes", "Quote", quotes_);
        //    if (!projectionCurveID_.empty())
        //        XMLUtils::addChild(doc, node, "ProjectionCurve", projectionCurveID_);
        //    return node;
        //}

        public string ProjectionCurveID()
        {
            return _projectionCurveID;
        }

        public override void Accept(IAcyclicVisitor v)
        {
            //Visitor<SimpleYieldCurveSegment>* v1 = dynamic_cast<Visitor<SimpleYieldCurveSegment>*>(&v);
            if (v != null) v.visit(this);
            else base.Accept(v);
        }
    }

    /// <summary>
    /// The average OIS yield curve segment is used e.g. for USD OIS curve building where
    /// the curve segment is determined by  a set of composite quotes and a projection curve.
    /// The composite quote is  represented here as a pair of quote strings, a tenor basis spread
    /// and an interest rate swap quote.
    /// </summary>
    public class AverageOISYieldCurveSegment : YieldCurveSegment
    {
        private string _projectionCurveID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AverageOISYieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        /// <param name="projectionCurveID"></param>
        public AverageOISYieldCurveSegment(string typeID, string conventionsID, List<string> quotes, string projectionCurveID)
            : base(typeID, conventionsID, quotes)
        {
            _projectionCurveID = projectionCurveID;
        }        

        
        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "AverageOIS");
            base.FromXML(node);
            _projectionCurveID = GetChildValue(node, "ProjectionCurve", false);

            // Read the Quotes node.
            XmlNode quotesNode = GetChildNode(node, "Quotes");
            if (quotesNode != null)
            {
                XmlNode child = GetChildNode(quotesNode, "CompositeQuote");
                while(child != null)            
                {
                    _quotes.Add(GetChildValue(child, "RateQuote", true));
                    _quotes.Add(GetChildValue(child, "SpreadQuote", true));

                    child = GetNextSibling(child, "");
                }
            }
            else
            {
                Utils.QL_FAIL("No Quotes in segment. Remove segment or add quotes.");
            }
        }

        //public override XmlNode ToXML(XmlDocument doc)
        //{

        //}
        
        public string ProjectionCurveID()
        {
            return _projectionCurveID;
        }

        public override void Accept(IAcyclicVisitor v)
        {
            if (v != null) v.visit(this);
            else base.Accept(v);
        }
    }

    /// <summary>
    /// Yield curve building from tenor basis swap quotes requires a set of tenor
    /// basis spread quotes and the projection curve for either the shorter or the longer tenor
    /// which acts as the reference curve.
    /// </summary>
    public class TenorBasisYieldCurveSegment : YieldCurveSegment 
    {
        private string _shortProjectionCurveID;
        private string _longProjectionCurveID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TenorBasisYieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        /// <param name="shortProjectionCurveID"></param>
        /// <param name="longProjectionCurveID"></param>
        public TenorBasisYieldCurveSegment(string typeID, string conventionsID, List<string> quotes, string shortProjectionCurveID, string longProjectionCurveID)
            : base(typeID, conventionsID, quotes)
        {
            _shortProjectionCurveID = shortProjectionCurveID;
            _longProjectionCurveID = longProjectionCurveID;
        }
       
        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "TenorBasis");
            base.FromXML(node);
            _quotes = GetChildrenValues(node, "Quotes", "Quote", true);
            _shortProjectionCurveID = GetChildValue(node, "ProjectionCurveShort", false);
            _longProjectionCurveID = GetChildValue(node, "ProjectionCurveLong", false);
        }

        //public override XmlNode ToXML(XmlDocument doc)
        //{
        //    XMLNode* node = YieldCurveSegment::toXML(doc);
        //    XMLUtils::setNodeName(doc, node, "TenorBasis");
        //    XMLUtils::addChildren(doc, node, "Quotes", "Quote", quotes_);
        //    if (!shortProjectionCurveID_.empty())
        //        XMLUtils::addChild(doc, node, "ProjectionCurveShort", shortProjectionCurveID_);
        //    if (!longProjectionCurveID_.empty())
        //        XMLUtils::addChild(doc, node, "ProjectionCurveLong", longProjectionCurveID_);
        //    return node;
        //}

        public string ShortProjectionCurveID()
        {
            return _shortProjectionCurveID;
        }

        public string LongProjectionCurveID()
        {
            return _longProjectionCurveID;
        }

        public override void Accept(IAcyclicVisitor v)
        {
            if (v != null) v.visit(this);
            else base.Accept(v);
        }
    }

    /// <summary>
    /// Cross currency basis spread adjusted discount curves for 'domestic' currency cash flows
    /// are built using this segment type which requires cross currency basis spreads quotes,
    /// the spot FX quote ID and at least the 'foreign' discount curve ID.
    /// Projection curves for both currencies can be provided as well for consistency with
    /// tenor basis in each currency.
    /// </summary>
    public class CrossCcyYieldCurveSegment : YieldCurveSegment
    {
        private string _spotRateID;
        private string _foreignDiscountCurveID;
        private string _domesticProjectionCurveID;
        private string _foreignProjectionCurveID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrossCcyYieldCurveSegment()
        {

        }

        //! Detailed constructor
        public CrossCcyYieldCurveSegment(string type, string conventionsID, List<string> quotes,
                                   string spotRateID, string foreignDiscountCurveID,
                                   string domesticProjectionCurveID = "",
                                   string foreignProjectionCurveID = "")
            : base(type, conventionsID, quotes)
        {
            _spotRateID = spotRateID;
            _foreignDiscountCurveID = foreignDiscountCurveID;
            _domesticProjectionCurveID = domesticProjectionCurveID;
            _foreignProjectionCurveID = foreignProjectionCurveID;
        }        
        
        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "CrossCurrency");
            base.FromXML(node);
            _quotes = GetChildrenValues(node, "Quotes", "Quote", true);
            _spotRateID = GetChildValue(node, "SpotRate", true);
            _foreignDiscountCurveID = GetChildValue(node, "DiscountCurve", true);
            _domesticProjectionCurveID = GetChildValue(node, "ProjectionCurveDomestic", false);
            _foreignProjectionCurveID = GetChildValue(node, "ProjectionCurveForeign", false);
        }

        //virtual XmlNode ToXML(XmlDocument doc)
        //{
        //    XMLNode* node = YieldCurveSegment::toXML(doc);
        //    XMLUtils::setNodeName(doc, node, "CrossCurrency");
        //    XMLUtils::addChildren(doc, node, "Quotes", "Quote", quotes_);
        //    XMLUtils::addChild(doc, node, "SpotRate", spotRateID_);
        //    XMLUtils::addChild(doc, node, "DiscountCurve", foreignDiscountCurveID_);
        //    if (!domesticProjectionCurveID_.empty())
        //        XMLUtils::addChild(doc, node, "ProjectionCurveDomestic", domesticProjectionCurveID_);
        //    if (!foreignProjectionCurveID_.empty())
        //        XMLUtils::addChild(doc, node, "ProjectionCurveForeign", foreignProjectionCurveID_);
        //    return node;
        //}

        public string SpotRateID()
        {
            return _spotRateID;
        }
        public string ForeignDiscountCurveID()
        {
            return _foreignDiscountCurveID;
        }
        public string DomesticProjectionCurveID()
        {
            return _domesticProjectionCurveID;
        }
        public string ForeignProjectionCurveID()
        {
            return _foreignProjectionCurveID;
        }

        public override void Accept(IAcyclicVisitor v)
        {
            if (v != null) v.visit(this);
            else base.Accept(v);
        }
    }

    /// <summary>
    /// A zero spreaded segment is used to build a yield curve from zero spread quotes and
    /// a reference yield curve.
    /// </summary>
    public class ZeroSpreadedYieldCurveSegment : YieldCurveSegment 
    {
        private string _referenceCurveID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ZeroSpreadedYieldCurveSegment()
        {

        }

        /// <summary>
        /// Detailed constructor
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="conventionsID"></param>
        /// <param name="quotes"></param>
        /// <param name="referenceCurveID"></param>
        public ZeroSpreadedYieldCurveSegment(string typeID, string conventionsID, List<string> quotes, string referenceCurveID)
            : base(typeID, conventionsID, quotes)
        {
            _referenceCurveID = referenceCurveID;
        }
        
        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "ZeroSpread");
            base.FromXML(node);
            _quotes = GetChildrenValues(node, "Quotes", "Quote", true);
            _referenceCurveID = GetChildValue(node, "ReferenceCurve", false);
        }

        //public override XmlNode ToXML(XmlDocument doc)
        //{

        //}

        public string ReferenceCurveID()
        {
            return _referenceCurveID;
        }

        public override void Accept(IAcyclicVisitor v)
        {
            if (v != null) v.visit(this);
            else base.Accept(v);
        }
    }
}

