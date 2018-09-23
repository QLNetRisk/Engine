using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using QLNet;

namespace QLRData
{
    public class YieldCurveConfig : CurveConfig
    {
        // Mandatory members
        private string _currency;
        private string _discountCurveID;
        private List<YieldCurveSegment> _curveSegments = new List<YieldCurveSegment>();
        private HashSet<string> _requiredYieldCurveIDs= new HashSet<string>();

        // Optional members
        private string _interpolationVariable;
        private string _interpolationMethod;
        private string _zeroDayCounter;
        private bool _extrapolation;
        private double _tolerance;

        /// <summary>
        /// Default constructor
        /// </summary>
        public YieldCurveConfig()
        {

        }

        //Detailed constructor
        public YieldCurveConfig(string curveID, string curveDescription, string currency,
                         string discountCurveID, List<YieldCurveSegment> curveSegments,
                         string interpolationVariable = "Discount", string interpolationMethod = "LogLinear",
                         string zeroDayCounter = "A365", bool extrapolation = true, double tolerance = 1.0e-12)
            : base(curveID, curveDescription)
        {
            _currency = currency;
            _discountCurveID = discountCurveID;
            _curveSegments = curveSegments;
            _interpolationVariable = interpolationVariable;
            _interpolationMethod = interpolationMethod;
            _zeroDayCounter = zeroDayCounter;
            _extrapolation = extrapolation;
            _tolerance = tolerance;

            PopulateRequiredYieldCurveIDs();
        }

        public override void FromXML(XmlNode node)
        {
            CheckNode(node, "YieldCurve");

            // Read in the mandatory nodes.
            _curveID = GetChildValue(node, "CurveId", true);
            _curveDescription = GetChildValue(node, "CurveDescription", true);
            _currency = GetChildValue(node, "Currency", true);
            _discountCurveID = GetChildValue(node, "DiscountCurve", true);

            // Read in the segments.
            XmlNode segmentsNode = GetChildNode(node, "Segments");
            if (segmentsNode != null)
            {
                XmlNode child = GetChildNode(segmentsNode, "");                
                while(child != null)
                {
                    YieldCurveSegment segment = new YieldCurveSegment();
                    string childName = GetNodeName(child);

                    if (childName == "Direct")
                    {
                        segment = new DirectYieldCurveSegment();
                    }
                    else if (childName == "Simple")
                    {
                        segment = new SimpleYieldCurveSegment();
                    }
                    else if (childName == "AverageOIS")
                    {
                        segment = new AverageOISYieldCurveSegment();
                    }
                    else if (childName == "TenorBasis")
                    {
                        segment = new TenorBasisYieldCurveSegment();
                    }
                    else if (childName == "CrossCurrency")
                    {
                        segment = new CrossCcyYieldCurveSegment();
                    }
                    else if (childName == "ZeroSpread")
                    {
                        segment = new ZeroSpreadedYieldCurveSegment();
                    }
                    else
                    {
                        Utils.QL_FAIL("Yield curve segment node name not recognized.");
                    }

                    if (segment != null)
                    {
                        try
                        {
                            segment.FromXML(child);
                        }
                        catch (Exception ex)
                        {
                            //ALOG("Exception parsing yield curve segment XML Node, name = " + childName + " and curveID = " + _curveID + " : " + ex.ToString();
                        }
                    }
                    else
                    {
                        //LOG("Unable to build yield curve segment for name = " << childName << " and curveID = " << curveID_);
                    }

                    _curveSegments.Add(segment);
                    child = GetNextSibling(child, "");
                }                    
            }
            else
            {
                Utils.QL_FAIL("No Segments node in XML doc for yield curve ID = " + _curveID);
            }

            // Read in the optional nodes.

            // Empty strings if not there (or if there and empty).
            _interpolationVariable = GetChildValue(node, "InterpolationVariable", false);
            _interpolationMethod = GetChildValue(node, "InterpolationMethod", false);
            _zeroDayCounter = GetChildValue(node, "YieldCurveDayCounter", false);

            // Add hardcoded defaults for now.
            if (_interpolationVariable == "")
            {
                _interpolationVariable = "Discount";
            }
            if (_interpolationMethod == "")
            {
                _interpolationMethod = _interpolationVariable == "Zero" ? "Linear" : "LogLinear";
            }
            if (_zeroDayCounter == "")
            {
                _zeroDayCounter = "A365";
            }
            XmlNode nodeToTest = GetChildNode(node, "Extrapolation");
            if (nodeToTest != null)
            {
                _extrapolation = GetChildValueAsBool(node, "Extrapolation", false);
            }
            else
            {
                _extrapolation = true;
            }
            nodeToTest = GetChildNode(node, "Tolerance");
            if (nodeToTest != null)
            {
                _tolerance = GetChildValueAsDouble(node, "Tolerance", false);
            }
            else
            {
                _tolerance = 1.0e-12;
            }

            PopulateRequiredYieldCurveIDs();
        }

        public string Currency()
        {
            return _currency;
        }
        public string DiscountCurveID()
        {
            return _discountCurveID;
        }
        public  List<YieldCurveSegment> CurveSegments()
        {
            return _curveSegments;
        }
        public string InterpolationVariable()
        {
            return _interpolationVariable;
        }
        public string InterpolationMethod()
        {
            return _interpolationMethod;
        }
        public string ZeroDayCounter()
        {
            return _zeroDayCounter;
        }
        public bool Extrapolation()
        {
            return _extrapolation;
        }
        public double Tolerance()
        {
            return _tolerance;
        }
        public HashSet<string> RequiredYieldCurveIDs()
        {
            return _requiredYieldCurveIDs;
        }

        public override List<string> Quotes()
        {
            if (_quotes.Count == 0)
            {
                List<string> segmentQuotes = new List<string>();
                foreach (var c in _curveSegments)
                {
                    segmentQuotes = c.Quotes();
                    _quotes.AddRange(segmentQuotes);
                }
            }
            return _quotes;
        }

        private void PopulateRequiredYieldCurveIDs()
        {
            if (_requiredYieldCurveIDs.Any())
            {
                _requiredYieldCurveIDs.Clear();
            }

            if (_curveID != _discountCurveID && _discountCurveID != "")
            {
                _requiredYieldCurveIDs.Add(_discountCurveID);
            }

            SegmentIDGetter segmentIDGetter = new SegmentIDGetter(_curveID, _requiredYieldCurveIDs);
            for (int i = 0; i < _curveSegments.Count; i++)
            {
                _curveSegments[i].Accept(segmentIDGetter);
            }
        }
    }

    public class SegmentIDGetter : IAcyclicVisitor
    //public Visitor<YieldCurveSegment>,
    //public Visitor<SimpleYieldCurveSegment>,
    //public Visitor<AverageOISYieldCurveSegment>,
    //public Visitor<TenorBasisYieldCurveSegment>,
    //public Visitor<CrossCcyYieldCurveSegment>,
    //public Visitor<ZeroSpreadedYieldCurveSegment> 
    {
        private string _curveID;
        private HashSet<string> _requiredYieldCurveIDs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="curveID"></param>
        /// <param name="requiredYieldCurveIDs"></param>
        public SegmentIDGetter(string curveID, HashSet<string> requiredYieldCurveIDs)
        {
            _curveID = curveID;
            _requiredYieldCurveIDs = requiredYieldCurveIDs;
        }

        public void visit(object o)
        {
            Type[] types = new Type[] { o.GetType() };
            MethodInfo methodInfo = Utils.GetMethodInfo(this, "visit", types);
            if (methodInfo != null)
            {
                methodInfo.Invoke(this, new object[] { o });
            }
        }

        public void visit(YieldCurveSegment s)
        {
            // Do nothing
        }    

        public void visit(SimpleYieldCurveSegment s)
        {
            string aCurveID = s.ProjectionCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
        }

        public void visit(AverageOISYieldCurveSegment s)
        {
            string aCurveID = s.ProjectionCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
        }

        public void visit(TenorBasisYieldCurveSegment s)
        {
            string aCurveID = s.ShortProjectionCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
            aCurveID = s.LongProjectionCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
        }

        public void visit(CrossCcyYieldCurveSegment s)
        {
            string aCurveID = s.ForeignDiscountCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
            aCurveID = s.DomesticProjectionCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
            aCurveID = s.ForeignProjectionCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
        }

        public void visit(ZeroSpreadedYieldCurveSegment s)
        {
            string aCurveID = s.ReferenceCurveID();
            if (_curveID != aCurveID && aCurveID != "")
            {
                _requiredYieldCurveIDs.Add(aCurveID);
            }
        }
    }
}
