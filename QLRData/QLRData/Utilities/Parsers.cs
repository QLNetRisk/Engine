﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Global = System.Globalization;
using QLNet;

namespace QLRData
{
    public static class Parsers
    {
        public static Period ParsePeriod(string s)
        {
            return PeriodParser.Parse(s);
        }

        public static IborIndex ParseIborIndex(string s, Handle<YieldTermStructure> h = null)
        {
            List<string> tokens = s.Split('-').ToList();

            QLNet.Utils.QL_REQUIRE(tokens.Count == 2 || tokens.Count == 3, () => "Two or three tokens required in " + s + ": CCY-INDEX or CCY-INDEX-TERM");

            Period p = new Period();
            if (tokens.Count == 3)
            {
                p = ParsePeriod(tokens[2]);
            }
            else
            {
                p = new Period(1, TimeUnit.Days); // TODO: Verify!
            }

            Dictionary<string, IborIndexParser> m = new Dictionary<string, IborIndexParser>()
                {{"EUR-EONIA", new IborIndexParserOIS<Eonia>()},
                {"GBP-SONIA", new IborIndexParserOIS<Sonia>()},
                //{"JPY-TONAR", new IborIndexParserOIS<Tonar>()},
                //{"CHF-TOIS", new IborIndexParserOIS<CHFTois>()},
                {"USD-FedFunds", new IborIndexParserOIS<FedFunds>()},
                //{"CAD-CORRA", new IborIndexParserOIS<CORRA>()},
                //{"AUD-BBSW", new IborIndexParserWithPeriod<AUDbbsw>()},
                {"AUD-LIBOR", new IborIndexParserWithPeriod<AUDLibor>()},
                {"EUR-EURIBOR", new IborIndexParserWithPeriod<Euribor>()},
                {"EUR-EURIB", new IborIndexParserWithPeriod<Euribor>()},
                {"CAD-CDOR", new IborIndexParserWithPeriod<Cdor>()},
                {"CAD-BA", new IborIndexParserWithPeriod<Cdor>()},
                //{"CZK-PRIBOR", new IborIndexParserWithPeriod<CZKPribor>()},
                {"EUR-LIBOR", new IborIndexParserWithPeriod<EURLibor>()},
                {"USD-LIBOR", new IborIndexParserWithPeriod<USDLibor>()},
                {"GBP-LIBOR", new IborIndexParserWithPeriod<GBPLibor>()},
                {"JPY-LIBOR", new IborIndexParserWithPeriod<JPYLibor>()},
                {"JPY-TIBOR", new IborIndexParserWithPeriod<Tibor>()},
                {"CAD-LIBOR", new IborIndexParserWithPeriod<CADLibor>()},
                {"CHF-LIBOR", new IborIndexParserWithPeriod<CHFLibor>()},
                {"SEK-LIBOR", new IborIndexParserWithPeriod<SEKLibor>()},
                //{"SEK-STIBOR", new IborIndexParserWithPeriod<SEKStibor>()},
                //{"NOK-NIBOR", new IborIndexParserWithPeriod<NOKNibor>()},
                //{"HKD-HIBOR", new IborIndexParserWithPeriod<HKDHibor>()},
                //{"SGD-SIBOR", new IborIndexParserWithPeriod<SGDSibor>()},
                //{"SGD-SOR", new IborIndexParserWithPeriod<SGDSor>()},
                //{"DKK-CIBOR", new IborIndexParserWithPeriod<DKKCibor>()},
                {"DKK-LIBOR", new IborIndexParserWithPeriod<DKKLibor>()},
                //{"HUF-BUBOR", new IborIndexParserWithPeriod<HUFBubor>()},
                //{"IDR-IDRFIX", new IborIndexParserWithPeriod<IDRIdrfix>()},
                //{"INR-MIFOR", new IborIndexParserWithPeriod<INRMifor>()},
                //{"MXN-TIIE", new IborIndexParserWithPeriod<MXNTiie>()},
                //{"PLN-WIBOR", new IborIndexParserWithPeriod<PLNWibor>()},
                //{"SKK-BRIBOR", new IborIndexParserWithPeriod<SKKBribor>()},
                //{"NZD-BKBM", new IborIndexParserWithPeriod<NZDBKBM>()},
                //{"TWD-TAIBOR", new IborIndexParserWithPeriod<TWDTaibor>()},
                //{"MYR-KLIBOR", new IborIndexParserWithPeriod<MYRKlibor>()},
                //{"KRW-KORIBOR", new IborIndexParserWithPeriod<KRWKoribor>()},
                {"ZAR-JIBAR", new IborIndexParserWithPeriod<Jibar>()}};

            string key = tokens[0] + "-" + tokens[1];
            if (m.ContainsKey(key))
            {
                return m[key].Build(p, h);
            }
            else
            {
                QLNet.Utils.QL_FAIL("parseIborIndex \"" + s + "\" not recognized");
                return null;
            }
        }

        public static DateGeneration.Rule ParseDateGenerationRule(string s)
        {
            Dictionary<string, DateGeneration.Rule> m = new Dictionary<string, DateGeneration.Rule>
            {
                { "Backward", DateGeneration.Rule.Backward},
                { "Forward", DateGeneration.Rule.Forward},
                { "Zero", DateGeneration.Rule.Zero},
                { "ThirdWednesday", DateGeneration.Rule.ThirdWednesday},
                { "Twentieth", DateGeneration.Rule.Twentieth},
                { "TwentiethIMM", DateGeneration.Rule.TwentiethIMM},
                { "OldCDS", DateGeneration.Rule.OldCDS},
                { "CDS2015", DateGeneration.Rule.CDS2015},
                { "CDS", DateGeneration.Rule.CDS}
            };

            if (m.ContainsKey(s)) return m[s];
            else
            {
                // fall back for CDS2015
                if (s == "CDS2015")
                {
                    //ALOG("Date Generation Rule CDS2015 replaced with CDS because QuantLib Version is < 1.10");
                    return DateGeneration.Rule.CDS;
                }
                Utils.QL_FAIL("Date Generation Rule " + s + " not recognized");
                throw new Exception();
            }
        }

        public static Currency ParseCurrency(string s)
        {
            //Dictionary<string, Currency> m = new Dictionary<string, Currency>
            //{
            //    {"ATS", new ATSCurrency()}, {"AUD", new AUDCurrency()}, {"BEF", new BEFCurrency()}, {"BRL", new BRLCurrency()},
            //    {"CAD", new CADCurrency()}, {"CHF", new CHFCurrency()}, {"CNY", new CNYCurrency()}, {"CZK", new CZKCurrency()},
            //    {"DEM", new DEMCurrency()}, {"DKK", new DKKCurrency()}, {"EUR", new EURCurrency()}, {"ESP", new ESPCurrency()},
            //    {"FIM", new FIMCurrency()}, {"FRF", new FRFCurrency()}, {"GBP", new GBPCurrency()}, {"GRD", new GRDCurrency()},
            //    {"HKD", new HKDCurrency()}, {"HUF", new HUFCurrency()}, {"IEP", new IEPCurrency()}, {"ITL", new ITLCurrency()},
            //    {"INR", new INRCurrency()}, {"ISK", new ISKCurrency()}, {"JPY", new JPYCurrency()}, {"KRW", new KRWCurrency()},
            //    {"LUF", new LUFCurrency()}, {"NLG", new NLGCurrency()}, {"NOK", new NOKCurrency()}, {"NZD", new NZDCurrency()},
            //    {"PLN", new PLNCurrency()}, {"PTE", new PTECurrency()}, {"RON", new RONCurrency()}, {"SEK", new SEKCurrency()},
            //    {"SGD", new SGDCurrency()}, {"THB", new THBCurrency()}, {"TRY", new TRYCurrency()}, {"TWD", new TWDCurrency()},
            //    {"USD", new USDCurrency()}, {"ZAR", new ZARCurrency()}, {"ARS", new ARSCurrency()}, {"CLP", new CLPCurrency()},
            //    {"COP", new COPCurrency()}, {"IDR", new IDRCurrency()}, {"ILS", new ILSCurrency()}, {"KWD", new KWDCurrency()},
            //    {"PEN", new PENCurrency()}, {"MXN", new MXNCurrency()}, {"SAR", new SARCurrency()}, {"RUB", new RUBCurrency()},
            //    {"TND", new TNDCurrency()}, {"MYR", new MYRCurrency()}, {"UAH", new UAHCurrency()}, {"KZT", new KZTCurrency()},
            //    {"QAR", new QARCurrency()}, {"MXV", new MXVCurrency()}, {"CLF", new CLFCurrency()}, {"EGP", new EGPCurrency()},
            //    {"BHD", new BHDCurrency()}, {"OMR", new OMRCurrency()}, {"VND", new VNDCurrency()}, {"AED", new AEDCurrency()},
            //    {"PHP", new PHPCurrency()}, {"NGN", new NGNCurrency()}, {"MAD", new MADCurrency()}
            //};

            Dictionary<string, Currency> m = new Dictionary<string, Currency>
            {
                {"ATS", new ATSCurrency()}, {"AUD", new AUDCurrency()}, {"BEF", new BEFCurrency()}, {"BRL", new BRLCurrency()},
                {"CAD", new CADCurrency()}, {"CHF", new CHFCurrency()}, {"CNY", new CNYCurrency()}, {"CZK", new CZKCurrency()},
                {"DEM", new DEMCurrency()}, {"DKK", new DKKCurrency()}, {"EUR", new EURCurrency()}, {"ESP", new ESPCurrency()},
                {"FIM", new FIMCurrency()}, {"FRF", new FRFCurrency()}, {"GBP", new GBPCurrency()}, {"GRD", new GRDCurrency()},
                {"HKD", new HKDCurrency()}, {"HUF", new HUFCurrency()}, {"IEP", new IEPCurrency()}, {"ITL", new ITLCurrency()},
                {"INR", new INRCurrency()}, {"ISK", new ISKCurrency()}, {"JPY", new JPYCurrency()}, {"KRW", new KRWCurrency()},
                {"LUF", new LUFCurrency()}, {"NLG", new NLGCurrency()}, {"NOK", new NOKCurrency()}, {"NZD", new NZDCurrency()},
                {"PLN", new PLNCurrency()}, {"PTE", new PTECurrency()}, {"RON", new RONCurrency()}, {"SEK", new SEKCurrency()},
                {"SGD", new SGDCurrency()}, {"THB", new THBCurrency()}, {"TRY", new TRYCurrency()}, {"TWD", new TWDCurrency()},
                {"USD", new USDCurrency()}, {"ZAR", new ZARCurrency()}, {"ARS", new ARSCurrency()}, {"CLP", new CLPCurrency()},
                {"COP", new COPCurrency()}, {"IDR", new IDRCurrency()}, {"ILS", new ILSCurrency()}, {"KWD", new KWDCurrency()},
                {"PEN", new PENCurrency()}, {"MXN", new MXNCurrency()}, {"SAR", new SARCurrency()}, {"RUB", new RUBCurrency()},
                {"MYR", new MYRCurrency()}, {"UAH", new UAHCurrency()},{"VND", new VNDCurrency()}
            };

            if (m.ContainsKey(s)) return m[s];
            else
            {
                QLNet.Utils.QL_FAIL("Currency " + s + " not recognized");
                throw new Exception();
            }
        }

        public static Date ParseDateExact(string s, string format)
        {
            DateTime date = DateTime.ParseExact(s, "yyyyMMdd", Global.CultureInfo.InvariantCulture);

            return date;
        }

        public static Date ParseDate(string s)
        {
            DateTime date = new DateTime();

            if (DateTime.TryParse(s, out date)) return date;
            else
            {
                QLNet.Utils.QL_FAIL("Cannot convert \"" + s + "\" to Date.");
                return new Date();
            }
        }

        public static double ParseDouble(string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch (Exception ex)
            {
                QLNet.Utils.QL_FAIL("Failed to parse double " + s + " " + ex.ToString());
                throw new Exception();
            }
        }

        public static int ParseInteger(string s)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception ex)
            {
                QLNet.Utils.QL_FAIL("Failed to parse integer " + s + " " + ex.ToString());
                throw new Exception();
            }
        }

        public static bool ParseBool(string s)
        {
            Dictionary<string, bool> b = new Dictionary<string, bool>{{"Y", true}, {"YES", true}, {"TRUE", true},   {"true", true},   {"1", true},
                                  {"N", false}, {"NO", false}, {"FALSE", false}, {"false", false}, {"0", false}};

            if (b.ContainsKey(s)) return b[s];
            else
            {
                QLNet.Utils.QL_FAIL("Cannot convert " + s + " to bool");
                throw new Exception(); // TODO avoid doubling throwing exception
            }
        }

        public static Calendar ParseCalendar(string s)
        {
            Dictionary<string, Calendar> m = new Dictionary<string, Calendar>{{"TGT", new TARGET()},
                                          {"TARGET", new TARGET()},
                                          {"EUR", new TARGET()},
                                          {"ZUB", new Switzerland()},
                                          {"CHF", new Switzerland()},
                                          {"CHZU", new Switzerland()},
                                          {"Switzerland", new Switzerland()},
                                          {"US", new UnitedStates()},
                                          {"USNY", new UnitedStates()},
                                          {"USD", new UnitedStates()},
                                          {"NYB", new UnitedStates()},
                                          {"US-SET", new UnitedStates(UnitedStates.Market.Settlement)},
                                          {"US settlement", new UnitedStates(UnitedStates.Market.Settlement)},
                                          {"US-GOV", new UnitedStates(UnitedStates.Market.GovernmentBond)},
                                          {"US-NYSE", new UnitedStates(UnitedStates.Market.NYSE)},
                                          {"US-NERC", new UnitedStates(UnitedStates.Market.NERC)},
                                          {"GB", new UnitedKingdom()},
                                          {"GBP", new UnitedKingdom()},
                                          {"UK", new UnitedKingdom()},
                                          {"UK settlement", new UnitedKingdom()},
                                          {"LNB", new UnitedKingdom()},
                                          {"CA", new Canada()},
                                          {"TRB", new Canada()},
                                          {"CAD", new Canada()},
                                          {"CATO", new Canada()},
                                          {"Canada", new Canada()},
                                          {"SYB", new Australia()},
                                          {"AU", new Australia()},
                                          {"AUD", new Australia()},
                                          {"Australia", new Australia()},
                                          {"TKB", new Japan()},
                                          {"JP", new Japan()},
                                          {"JPTO", new Japan()},
                                          {"JPY", new Japan()},
                                          {"Japan", new Japan()},
                                          {"ZAR", new SouthAfrica()},
                                          {"SA", new SouthAfrica()},
                                          {"SS", new Sweden()},
                                          {"SEK", new Sweden()},
                                          {"SEST", new Sweden()},
                                          {"ARS", new Argentina()},
                                          {"BRL", new Brazil()},
                                          {"CNY", new China()},
                                          {"CZK", new CzechRepublic()},
                                          {"DKK", new Denmark()},
                                          {"DEN", new Denmark()},
                                          {"FIN", new Finland()},
                                          {"HKD", new HongKong()},
                                          {"ISK", new Iceland()},
                                          {"INR", new India()},
                                          {"IDR", new Indonesia()},
                                          {"MXN", new Mexico()},
                                          {"NZD", new NewZealand()},
                                          {"NOK", new Norway()},
                                          {"PLN", new Poland()},
                                          {"RUB", new Russia()},
                                          {"SAR", new SaudiArabia()},
                                          {"SGD", new Singapore()},
                                          {"KRW", new SouthKorea()},
                                          {"TWD", new Taiwan()},
                                          {"TRY", new Turkey()},
                                          {"TRIS", new Turkey()},
                                          {"UAH", new Ukraine()},
                                          {"HUF", new Hungary()},
                                          {"GBLO", new UnitedKingdom()},
                                          // fallback to TARGET for these emerging ccys
                                          {"CLP", new TARGET()},
                                          {"RON", new TARGET()},
                                          {"THB", new TARGET()},
                                          {"COP", new TARGET()},
                                          {"ILS", new TARGET()},
                                          {"KWD", new TARGET()},
                                          {"PEN", new TARGET()},
                                          {"TND", new TARGET()},
                                          {"MYR", new TARGET()},
                                          {"KZT", new TARGET()},
                                          {"QAR", new TARGET()},
                                          {"MXV", new TARGET()},
                                          {"CLF", new TARGET()},
                                          {"EGP", new TARGET()},
                                          {"BHD", new TARGET()},
                                          {"OMR", new TARGET()},
                                          {"VND", new TARGET()},
                                          {"AED", new TARGET()},
                                          {"PHP", new TARGET()},
                                          {"NGN", new TARGET()},
                                          {"MAD", new TARGET()},
                                          // ISDA http://www.fpml.org/coding-scheme/business-center-7-15.xml
                                          {"EUTA", new TARGET()},
                                          {"BEBR", new TARGET()}, // Belgium, Brussels not in QL
                                          {"WeekendsOnly", new WeekendsOnly()},
                                          {"UNMAPPED", new WeekendsOnly()},
                                          {"NullCalendar", new NullCalendar()}};

            if (m.ContainsKey(s))
            {
                return m[s];
            }
            else
            {
                // Try to split them up
                List<string> calendarNames = s.Split(',').ToList();
                //string.Split(calendarNames, s, boost::is_any_of(",()")); // , is delimiter, the brackets may arise if joint calendar
                // now remove any leading strings indicating a joint calendar
                // TODO: Fixme!
                //calendarNames.erase(std::remove(calendarNames.begin(), calendarNames.end(), "JoinHolidays"), calendarNames.end());            
                //calendarNames.erase(std::remove(calendarNames.begin(), calendarNames.end(), "JoinBusinessDays"), calendarNames.end());
                //calendarNames.erase(std::remove(calendarNames.begin(), calendarNames.end(), ""), calendarNames.end());
                Utils.QL_REQUIRE(calendarNames.Count > 1 && calendarNames.Count <= 4, () => "Cannot convert " + s + " to Calendar");

                // Populate a vector of calendars.
                List<Calendar> calendars = new List<Calendar>();
                for (int i = 0; i < calendarNames.Count; i++)
                {
                    calendarNames[i].Trim();
                    try
                    {
                        calendars.Add(ParseCalendar(calendarNames[i]));
                    }
                    catch (Exception ex)
                    {
                        QLNet.Utils.QL_FAIL("Cannot convert " + s + " to Calendar [exception:" + ex.ToString() + "]");
                    }
                }

                switch (calendarNames.Count)
                {
                    case 2:
                        return new JointCalendar(calendars[0], calendars[1]);
                    case 3:
                        return new JointCalendar(calendars[0], calendars[1], calendars[2]);
                    case 4:
                        return new JointCalendar(calendars[0], calendars[1], calendars[2], calendars[3]);
                    default:
                        QLNet.Utils.QL_FAIL("Cannot convert " + s + " to Calendar");
                        throw new Exception();
                }
            }
        }

        public static BusinessDayConvention ParseBusinessDayConvention(string s)
        {
            Dictionary<string, BusinessDayConvention> m = new Dictionary<string, BusinessDayConvention>{{"F", BusinessDayConvention.Following},
                                                   {"Following", BusinessDayConvention.Following},
                                                   {"FOLLOWING", BusinessDayConvention.Following},
                                                   {"MF", BusinessDayConvention.ModifiedFollowing},
                                                   {"ModifiedFollowing", BusinessDayConvention.ModifiedFollowing},
                                                   {"Modified Following", BusinessDayConvention.ModifiedFollowing},
                                                   {"MODIFIEDF", BusinessDayConvention.ModifiedFollowing},
                                                   {"MODFOLLOWING", BusinessDayConvention.ModifiedFollowing},
                                                   {"P", BusinessDayConvention.Preceding},
                                                   {"Preceding", BusinessDayConvention.Preceding},
                                                   {"PRECEDING", BusinessDayConvention.Preceding},
                                                   {"MP", BusinessDayConvention.ModifiedPreceding},
                                                   {"ModifiedPreceding", BusinessDayConvention.ModifiedPreceding},
                                                   {"Modified Preceding", BusinessDayConvention.ModifiedPreceding},
                                                   {"MODIFIEDP", BusinessDayConvention.ModifiedPreceding},
                                                   {"U", BusinessDayConvention.Unadjusted},
                                                   {"Unadjusted", BusinessDayConvention.Unadjusted},
                                                   {"INDIFF", BusinessDayConvention.Unadjusted},
                                                   {"NEAREST", BusinessDayConvention.Nearest},
                                                   {"NONE", BusinessDayConvention.Unadjusted},
                                                   {"NotApplicable", BusinessDayConvention.Unadjusted}};

            if (m.ContainsKey(s))
            {
                return m[s];
            }
            else
            {
                Utils.QL_FAIL("Cannot convert " + s + " to BusinessDayConvention");
                throw new Exception();
            }
        }

        public static DayCounter ParseDayCounter(string s)
        {
            Dictionary<string, DayCounter> m = new Dictionary<string, DayCounter>{{"A360", new Actual360()},
                                        {"Actual/360", new Actual360()},
                                        {"ACT/360", new Actual360()},
                                        {"A365", new Actual365Fixed()},
                                        {"A365F", new Actual365Fixed()},
                                        {"Actual/365 (Fixed)", new Actual365Fixed()},
                                        {"ACT/365.FIXED", new Actual365Fixed()},
                                        {"ACT/365", new Actual365Fixed()},
                                        {"ACT/365L", new Actual365Fixed()},
                                        {"T360", new Thirty360(Thirty360.Thirty360Convention.USA)},
                                        {"30/360", new Thirty360(Thirty360.Thirty360Convention.USA)},
                                        {"30/360 (Bond Basis)", new Thirty360(Thirty360.Thirty360Convention.USA)},
                                        {"ACT/nACT", new Thirty360(Thirty360.Thirty360Convention.USA)},
                                        {"30E/360 (Eurobond Basis)", new Thirty360(Thirty360.Thirty360Convention.European)},
                                        {"30E/360", new Thirty360(Thirty360.Thirty360Convention.European)},
                                        {"30E/360.ISDA", new Thirty360(Thirty360.Thirty360Convention.European)},
                                        {"30/360 (Italian)", new Thirty360(Thirty360.Thirty360Convention.Italian)},
                                        {"ActActISDA", new ActualActual(ActualActual.Convention.ISDA)},
                                        {"ACT/ACT.ISDA", new ActualActual(ActualActual.Convention.ISDA)},
                                        {"Actual/Actual (ISDA)", new ActualActual(ActualActual.Convention.ISDA)},
                                        {"ACT/ACT", new ActualActual(ActualActual.Convention.ISDA)},
                                        {"ACT29", new ActualActual(ActualActual.Convention.ISDA)},
                                        {"ACT", new ActualActual(ActualActual.Convention.ISDA)},
                                        {"ActActISMA", new ActualActual(ActualActual.Convention.ISMA)},
                                        {"Actual/Actual (ISMA)", new ActualActual(ActualActual.Convention.ISMA)},
                                        {"ActActAFB", new ActualActual(ActualActual.Convention.AFB)},
                                        {"ACT/ACT.AFB", new ActualActual(ActualActual.Convention.AFB)},
                                        {"ACT/ACT.ISMA", new ActualActual(ActualActual.Convention.ISMA)},
                                        {"Actual/Actual (AFB)", new ActualActual(ActualActual.Convention.AFB)},
                                        {"1/1", new OneDayCounter()},
                                        {"BUS/252", new Business252()},
                                        {"Business/252", new Business252()},
                                        // TODO: Check missing conventions enum
                                        //{"Actual/365 (No Leap)", new Actual365Fixed(Actual365Fixed.Convention.NoLeap)},
                                        //{"Act/365 (NL)", new Actual365Fixed(Actual365Fixed.Convention.NoLeap)},
                                        //{"NL/365", new Actual365Fixed(Actual365Fixed.Convention.NoLeap)},
                                        //{"Actual/365 (JGB)", new Actual365Fixed(Actual365Fixed.Convention.NoLeap)}
                                        {"Actual/365 (No Leap)", new Actual365Fixed()},
                                        {"Act/365 (NL)", new Actual365Fixed()},
                                        {"NL/365", new Actual365Fixed()},
                                        {"Actual/365 (JGB)", new Actual365Fixed()}};

            if (m.ContainsKey(s))
            {
                return m[s];
            }
            else
            {
                Utils.QL_FAIL("DayCounter " + s + " not recognized");
                throw new Exception();
            }
        }

        public static Compounding ParseCompounding(string s)
        {
            Dictionary<string, Compounding> m = new Dictionary<string, Compounding>{
                {"Simple", Compounding.Simple},
                {"Compounded", Compounding.Compounded},
                {"Continuous", Compounding.Continuous},
                {"SimpleThenCompounded", Compounding.SimpleThenCompounded}};

            if (m.ContainsKey(s))
            {
                return m[s];
            }
            else
            {
                Utils.QL_FAIL("Compounding \"" + s + "\" not recognized");
                throw new Exception();
            }
        }

        public static Frequency ParseFrequency(string s)
        {
            Dictionary<string, Frequency> m = new Dictionary<string, Frequency>{{"Z", Frequency.Once},
                                       {"Once", Frequency.Once},
                                       {"A", Frequency.Annual},
                                       {"Annual", Frequency.Annual},
                                       {"S", Frequency.Semiannual},
                                       {"Semiannual", Frequency.Semiannual},
                                       {"Q", Frequency.Quarterly},
                                       {"Quarterly", Frequency.Quarterly},
                                       {"B", Frequency.Bimonthly},
                                       {"Bimonthly", Frequency.Bimonthly},
                                       {"M", Frequency.Monthly},
                                       {"Monthly", Frequency.Monthly},
                                       {"L", Frequency.EveryFourthWeek},
                                       {"Lunarmonth", Frequency.EveryFourthWeek},
                                       {"W", Frequency.Weekly},
                                       {"Weekly", Frequency.Weekly},
                                       {"D", Frequency.Daily},
                                       {"Daily", Frequency.Daily}};

            if (m.ContainsKey(s))
            {
                return m[s];
            }
            else
            {
                Utils.QL_FAIL("Frequency \"" + s + "\" not recognized");
                throw new Exception();
            }
        }

        public static void ParseDateOrPeriod(string s, Date d, Period p, out bool isDate)
        {
            Utils.QL_REQUIRE(s != string.Empty, () => "Cannot parse empty string as date or period");
            string c = s + "1"; // (1, s.back());
            bool isPeriod = c.IndexOfAny(new char[] { 'D', 'd', 'W', 'w', 'M', 'm', 'Y', 'y' }) != 0;
            if (isPeriod)
            {
                p = ParsePeriod(s);
                isDate = false;
            }
            else
            {
                d = ParseDate(s);
                Utils.QL_REQUIRE(d != new Date(), () => "Cannot parse \"" + s + "\" as date");
                isDate = true;
            }
        }

        public static List<T> ParseListOfValues<T>(string s, Func<string, T> parser)
        {
            s = s.Trim();

            // Return empy list if string is empty
            if (s == string.Empty) return new List<T>();

            List<T> vec = new List<T>();
            List<string> tokens = s.Split(',').ToList();
            
            foreach(string r in tokens)
            {            
                vec.Add(parser(r.Trim()));
            }
            return vec;
        }
    }
}

    

