using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QLRData
{
    public class YieldCurveConfig : CurveConfig
    {
        // Mandatory members
        private string _currency;
        private string _discountCurveID;
        private List<YieldCurveSegment> _curveSegments;
        private HashSet<string> _requiredYieldCurveIDs;

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
        {


        }

        public override void FromXML(XmlNode node)
        {
//            XMLUtils::checkNode(node, "YieldCurve");

//            // Read in the mandatory nodes.
//            curveID_ = XMLUtils::getChildValue(node, "CurveId", true);
//            curveDescription_ = XMLUtils::getChildValue(node, "CurveDescription", true);
//            currency_ = XMLUtils::getChildValue(node, "Currency", true);
//            discountCurveID_ = XMLUtils::getChildValue(node, "DiscountCurve", true);

//            // Read in the segments.
//            XMLNode* segmentsNode = XMLUtils::getChildNode(node, "Segments");
//            if (segmentsNode)
//            {
//                for (XMLNode* child = XMLUtils::getChildNode(segmentsNode); child; child = XMLUtils::getNextSibling(child))
//                {

//                    boost::shared_ptr<YieldCurveSegment> segment;
//                    string childName = XMLUtils::getNodeName(child);

//                    if (childName == "Direct")
//                    {
//                        segment.reset(new DirectYieldCurveSegment());
//                    }
//                    else if (childName == "Simple")
//                    {
//                        segment.reset(new SimpleYieldCurveSegment());
//                    }
//                    else if (childName == "AverageOIS")
//                    {
//                        segment.reset(new AverageOISYieldCurveSegment());
//                    }
//                    else if (childName == "TenorBasis")
//                    {
//                        segment.reset(new TenorBasisYieldCurveSegment());
//                    }
//                    else if (childName == "CrossCurrency")
//                    {
//                        segment.reset(new CrossCcyYieldCurveSegment());
//                    }
//                    else if (childName == "ZeroSpread")
//                    {
//                        segment.reset(new ZeroSpreadedYieldCurveSegment());
//                    }
//                    else
//                    {
//                        QL_FAIL("Yield curve segment node name not recognized.");
//                    }

//                    if (segment)
//                    {
//                        try
//                        {
//                            segment->fromXML(child);
//                        }
//                        catch (std::exception&ex) {
//                    ALOG("Exception parsing yield curve segment XML Node, name = " << childName << " and curveID = "
//                                                                                   << curveID_ << " : " << ex.what());
//                }
//            }
//            else
//            {
//                LOG("Unable to build yield curve segment for name = " << childName << " and curveID = " << curveID_);
//            }
//            curveSegments_.push_back(segment);
//        }
//    } else {
//        QL_FAIL("No Segments node in XML doc for yield curve ID = " << curveID_);
//    }

//// Read in the optional nodes.

//// Empty strings if not there (or if there and empty).
//interpolationVariable_ = XMLUtils::getChildValue(node, "InterpolationVariable", false);
//    interpolationMethod_ = XMLUtils::getChildValue(node, "InterpolationMethod", false);
//    zeroDayCounter_ = XMLUtils::getChildValue(node, "YieldCurveDayCounter", false);

//    // Add hardcoded defaults for now.
//    if (interpolationVariable_.empty()) {
//        interpolationVariable_ = "Discount";
//    }
//    if (interpolationMethod_.empty()) {
//        interpolationMethod_ = interpolationVariable_ == "Zero" ? "Linear" : "LogLinear";
//    }
//    if (zeroDayCounter_.empty()) {
//        zeroDayCounter_ = "A365";
//    }
//    XMLNode* nodeToTest = XMLUtils::getChildNode(node, "Extrapolation");
//    if (nodeToTest) {
//        extrapolation_ = XMLUtils::getChildValueAsBool(node, "Extrapolation", false);
//    } else {
//        extrapolation_ = true;
//    }
//    nodeToTest = XMLUtils::getChildNode(node, "Tolerance");
//    if (nodeToTest) {
//        tolerance_ = XMLUtils::getChildValueAsDouble(node, "Tolerance", false);
//    } else {
//        tolerance_ = 1.0e-12;
//    }

//    populateRequiredYieldCurveIDs();
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

        //public override List<string> Quotes()
        //{

        //}

        private void PopulateRequiredYieldCurveIDs()
        {

        }
    }
}
