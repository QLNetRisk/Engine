using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public static class CurveSpecParser
    {
        public static CurveSpec.CurveType ParseCurveSpecType(string  s)
        {
            Dictionary<string, CurveSpec.CurveType> b = new Dictionary<string, CurveSpec.CurveType>
            {
                {"Yield", CurveSpec.CurveType.Yield},
                {"CapFloorVolatility", CurveSpec.CurveType.CapFloorVolatility},
                {"SwaptionVolatility", CurveSpec.CurveType.SwaptionVolatility},
                {"FX", CurveSpec.CurveType.FX},
                {"FXVolatility", CurveSpec.CurveType.FXVolatility},
                {"Default", CurveSpec.CurveType.Default},
                {"CDSVolatility", CurveSpec.CurveType.CDSVolatility},
                {"BaseCorrelation", CurveSpec.CurveType.BaseCorrelation},
                {"Inflation", CurveSpec.CurveType.Inflation},
                {"InflationCapFloorPrice", CurveSpec.CurveType.InflationCapFloorPrice},
                {"Equity", CurveSpec.CurveType.Equity},
                {"EquityVolatility", CurveSpec.CurveType.EquityVolatility},
                {"Security", CurveSpec.CurveType.Security}
            };

            if (b.ContainsKey(s))
            {
                return b[s];
            }
            else
            {
                Utils.QL_FAIL("Cannot convert " + s + " to CurveSpecType");
                throw new Exception();
            }
        }


        //function to convert a string into a curve spec
        public static CurveSpec ParseCurveSpec(string s)
        {
            List<string> tokens = s.Split('/').ToList();           

            Utils.QL_REQUIRE(tokens.Count > 1, () => "number of tokens too small in curve spec " + s);

            CurveSpec.CurveType curveType = ParseCurveSpecType(tokens[0]);

            switch (curveType)
            {
                case CurveSpec.CurveType.Yield:
                    {
                        // Expected format: Yield/CCY/CurveConfigID
                        // Example: Yield/EUR/eur-6M-curve
                        Utils.QL_REQUIRE(tokens.Count == 3, () => "Unexpected number of tokens in yield curve spec " + s);
                        
                        string ccy = tokens[1];
                        string curveConfigID = tokens[2];
                        return new YieldCurveSpec(ccy, curveConfigID);
                    }

                //case CurveSpec::CurveType::Default:
                //    {
                //        // Default/USD/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in default curve spec "
                //                                           << s);
                //        const string&ccy = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<DefaultCurveSpec>(ccy, curveConfigID);
                //    }

                //case CurveSpec::CurveType::CDSVolatility:
                //    {
                //        // CDSVolatility/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 2, "Unexpected number"


                //                                       " of tokens in cds vol spec "
                //                                           << s);
                //        const string&curveConfigID = tokens[1];
                //        return boost::make_shared<CDSVolatilityCurveSpec>(curveConfigID);
                //    }

                //case CurveSpec::CurveType::BaseCorrelation:
                //    {
                //        // BaseCorrelation/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 2, "Unexpected number"


                //                                       " of tokens in cds vol spec "
                //                                           << s);
                //        const string&curveConfigID = tokens[1];
                //        return boost::make_shared<BaseCorrelationCurveSpec>(curveConfigID);
                //    }

                //case CurveSpec::CurveType::FX:
                //    {
                //        // FX/USD/CHF
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in FX curve spec "
                //                                           << s);
                //        const string&unitCcy = tokens[1];
                //        const string&ccy = tokens[2];
                //        return boost::make_shared<FXSpotSpec>(unitCcy, ccy);
                //    }

                //case CurveSpec::CurveType::FXVolatility:
                //    {
                //        // FX/USD/CHF/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 4, "Unexpected number"


                //                                       " of tokens in fx vol curve spec "
                //                                           << s);
                //        const string&unitCcy = tokens[1];
                //        const string&ccy = tokens[2];
                //        const string&curveConfigID = tokens[3];
                //        return boost::make_shared<FXVolatilityCurveSpec>(unitCcy, ccy, curveConfigID);
                //    }

                //case CurveSpec::CurveType::SwaptionVolatility:
                //    {
                //        // SwaptionVolatility/EUR/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in swaption vol curve spec "
                //                                           << s);
                //        const string&ccy = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<SwaptionVolatilityCurveSpec>(ccy, curveConfigID);
                //    }

                //case CurveSpec::CurveType::CapFloorVolatility:
                //    {
                //        // e.g. CapFloorVolatility/EUR/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in CapFloor volatility curve spec "
                //                                           << s);
                //        const string&ccy = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<CapFloorVolatilityCurveSpec>(ccy, curveConfigID);
                //    }

                //case CurveSpec::CurveType::Inflation:
                //    {
                //        // Inflation/EUHICPXT/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in inflation curve spec "
                //                                           << s);
                //        const string&index = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<InflationCurveSpec>(index, curveConfigID);
                //    }

                //case CurveSpec::CurveType::InflationCapFloorPrice:
                //    {
                //        // InflationCapFloorPrice/EUHICPXT/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in inflation cap floor price surface spec "
                //                                           << s);
                //        const string&index = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<InflationCapFloorPriceSurfaceSpec>(index, curveConfigID);
                //    }

                //case CurveSpec::CurveType::Equity:
                //    {
                //        // Equity/USD/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in default curve spec "
                //                                           << s);
                //        const string&ccy = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<EquityCurveSpec>(ccy, curveConfigID);
                //    }

                //case CurveSpec::CurveType::EquityVolatility:
                //    {
                //        // EquityVolatility/USD/CurveConfigID
                //        QL_REQUIRE(tokens.size() == 3, "Unexpected number"


                //                                       " of tokens in default curve spec "
                //                                           << s);
                //        const string&ccy = tokens[1];
                //        const string&curveConfigID = tokens[2];
                //        return boost::make_shared<EquityVolatilityCurveSpec>(ccy, curveConfigID);
                //    }

                //case CurveSpec::CurveType::Security:
                //    {
                //        // Security/ISIN
                //        QL_REQUIRE(tokens.size() == 2, "Unexpected number"


                //                                       " of tokens in Security Spread spec "
                //                                           << s);
                //        const string&securityID = tokens[1];
                //        return boost::make_shared<SecuritySpec>(securityID);
                //    }

                    // TODO: the rest...
            }

            Utils.QL_FAIL("Unable to convert \"" + s + "\" into CurveSpec");
            throw new Exception();
        }
    }
}
