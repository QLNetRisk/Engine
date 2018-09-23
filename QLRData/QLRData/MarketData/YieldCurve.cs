using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    /// <summary>
    /// Wrapper class for building yield term structures
    /// 
    /// Given yield curve specification and its configuration
    /// this class will actually build a QuantLib yield
    /// termstructure.
    /// </summary>
    public class YieldCurve
    {
        /// <summary>
        /// Supported interpolation variables
        /// </summary>
        public enum InterpolationVariable
        {
            Zero,
            Discount,
            Forward
        }

        /// <summary>
        /// Supported interpolation methods
        /// </summary>
        public enum InterpolationMethod
        {
            Linear,
            LogLinear,
            NaturalCubic,
            FinancialCubic,
            ConvexMonotone
        }

        private Date _asofDate;
        private Currency _currency;
        private YieldCurveSpec _curveSpec;
        private DayCounter _zeroDayCounter;
        private double _accuracy;
        private bool _extrapolation;
        private YieldCurve _discountCurve;

        private Loader _loader;
        private Conventions _conventions;
        private RelinkableHandle<YieldTermStructure> _h;
        private YieldTermStructure _p;

        private YieldCurveConfig _curveConfig;
        private List<YieldCurveSegment> _curveSegments;
        private InterpolationVariable _interpolationVariable;
        private InterpolationMethod _interpolationMethod;
        private Dictionary<string, YieldCurve> _requiredYieldCurves;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asof">Valuation date</param>
        /// <param name="curveSpec">Yield curve specification</param>
        /// <param name="curveConfigs">Repository of yield curve configurations</param>
        /// <param name="loader">Market data loader</param>
        /// <param name="conventions">Repository of market conventions for building bootstrap helper</param>
        /// <param name="requiredYieldCurves">Map of underlying yield curves if required</param>
        public YieldCurve(Date asof, YieldCurveSpec curveSpec, CurveConfigurations curveConfigs, Loader loader, Conventions conventions, Dictionary<string, YieldCurve> requiredYieldCurves = null)
        {

        }        

        public Handle<YieldTermStructure> Handle()
        {
            return _h;
        }
        public YieldCurveSpec CurveSpec()
        {
            return _curveSpec;
        }
        public Date AsofDate()
        {
            return _asofDate;
        }
        public Currency Currency()
        {
            return _currency;
        }            

        private void BuildDiscountCurve()
        {
            Utils.QL_REQUIRE(_curveSegments.Count <= 1, () => "More than one zero curve segment not supported yet.");
            Utils.QL_REQUIRE(_curveSegments[0].CurveSegmentType() == YieldCurveSegment.Type.Zero, () => "The curve segment is not of type Zero.");

            // Fill a vector of zero quotes.
            List<ZeroQuote> zeroQuotes = new List<ZeroQuote>();
            DirectYieldCurveSegment zeroCurveSegment = _curveSegments[0] as DirectYieldCurveSegment;

            List<string> zeroQuoteIDs = zeroCurveSegment.Quotes();

            for (int i = 0; i < zeroQuoteIDs.Count; ++i)
            {
                MarketDatum marketQuote = _loader.Get(zeroQuoteIDs[i], _asofDate);
                if (marketQuote != null)
                {
                    Utils.QL_REQUIRE(marketQuote.GetInstrumentType() == MarketDatum.InstrumentType.ZERO, () => "Market quote not of type zero.");
                    ZeroQuote zeroQuote = marketQuote as ZeroQuote;
                    zeroQuotes.Add(zeroQuote);
                }
                else
                {
                    Utils.QL_FAIL("Could not find quote for ID " + zeroQuoteIDs[i] + " with as of date " + _asofDate + ".");
                }
            }

            // Create the (date, zero) pairs.
            Dictionary<Date, double> data = new Dictionary<Date, double>();
            Convention convention = _conventions.Get(_curveSegments[0].ConventionsID());
            Utils.QL_REQUIRE(convention != null, () => "No conventions found with ID: " + _curveSegments[0].ConventionsID());
            Utils.QL_REQUIRE(convention.ConventionType() == Convention.Type.Zero, () => "Conventions ID does not give zero rate conventions.");
            ZeroRateConvention zeroConvention = convention as ZeroRateConvention;
            DayCounter quoteDayCounter = zeroConvention.DayCounter();
            for (int i = 0; i < zeroQuotes.Count; ++i)
            {
                Utils.QL_REQUIRE(quoteDayCounter == zeroQuotes[i].DayCounter(), () => "The day counter should be the same between the conventions and the quote.");
                
                if (!zeroQuotes[i].TenorBased())
                {
                    data[zeroQuotes[i].Date()] = zeroQuotes[i].Quote().link.value();
                }
                else
                {
                    Utils.QL_REQUIRE(zeroConvention.TenorBased(), () => "Using tenor based zero rates without tenor based zero rate conventions.");


                    Date zeroDate = _asofDate;
                    if (zeroConvention.SpotLag() > 0)
                    {                    }

                        zeroDate = zeroConvention.SpotCalendar().advance(zeroDate, new Period(zeroConvention.SpotLag(), TimeUnit.Days));
                    zeroDate = zeroConvention.TenorCalendar().advance(zeroDate, zeroQuotes[i].Tenor(), zeroConvention.RollConvention(), zeroConvention.Eom());
                    data[zeroDate] = zeroQuotes[i].Quote().link.value();
                }
            }

            Utils.QL_REQUIRE(data.Count > 0, () => "No market data found for curve spec " + _curveSpec.Name() + " with as of date " + _asofDate);


            // \todo review - more flexible (flat vs. linear extrap)?
            if (data.Keys.First() > _asofDate)
            {
                double rate = data.Values.First();
                data[_asofDate] = rate;
                //LOG("Insert zero curve point at time zero for " + curveSpec_.name() + ": "+ "date " + _asofDate + ", "+"zero " + data[_asofDate]);
            }

            Utils.QL_REQUIRE(data.Count > 1, () => "The single zero rate quote provided should be associated with a date greater than as of date.");
            
            // First build temporary curves
            List<Date> dates = new List<Date>();
            List<double> zeroes = new List<double>();
            List<double> discounts = new List<double>();                
            dates.Add(data.Keys.First());
            zeroes.Add(data.Values.First());
            discounts.Add(1.0);

            Compounding zeroCompounding = zeroConvention.Compounding();
            Frequency zeroCompoundingFreq = zeroConvention.CompoundingFrequency();
            Dictionary<Date, double> it;
            foreach(KeyValuePair<Date, double> kvp in data)
            {
                Date d = kvp.Key;
                double r = kvp.Value;

                dates.Add(d);
                InterestRate tempRate = new InterestRate(r, quoteDayCounter, zeroCompounding, zeroCompoundingFreq);
                double t = quoteDayCounter.yearFraction(_asofDate, d);
                /* Convert zero rate to continuously compounded if necessary */
                if (zeroCompounding == Compounding.Continuous)
                {
                    zeroes.Add(r);
                }
                else
                {
                    zeroes.Add(tempRate.equivalentRate(Compounding.Continuous, Frequency.Annual, t).value());
                }
                discounts.Add(tempRate.discountFactor(t));
                //LOG("Add zero curve point for " + curveSpec_.name() + ": " + dates.Last() + " " + zeroes.Last() + " / " + discounts.Last());
            }

            Utils.QL_REQUIRE(dates.Count == zeroes.Count, () => "Date and zero vectors differ in size.");
            Utils.QL_REQUIRE(dates.Count == discounts.Count, () => "Date and discount vectors differ in size.");
            
            // Now build curve with requested conventions
            if (_interpolationVariable == YieldCurve.InterpolationVariable.Zero) {
                YieldTermStructure tempCurve = Zerocurve(dates, zeroes, quoteDayCounter);
                zeroes.Clear();
                for (int i = 0; i<dates.Count; ++i)
                {
                    double zero = tempCurve.zeroRate(dates[i], _zeroDayCounter, Compounding.Continuous).value();
                    zeroes.Add(zero);
                }

                _p = Zerocurve(dates, zeroes, _zeroDayCounter);
            }
            else if (_interpolationVariable == YieldCurve.InterpolationVariable.Discount)
            {
                YieldTermStructure tempCurve = Discountcurve(dates, discounts, quoteDayCounter);
                discounts.Clear();
                for (int i = 0; i < dates.Count; ++i)
                {
                    double discount = tempCurve.discount(dates[i]);
                    discounts.Add(discount);
                }
                _p = Discountcurve(dates, discounts, _zeroDayCounter);
            }
            else 
            {
                Utils.QL_FAIL("Unknown yield curve interpolation variable.");
            }
        }
        private void BuildZeroCurve()
        {

        }
        private void BuildZeroSpreadedCurve()
        {

        }
        private void BuildBootstrappedCurve()
        {

        }        

        private YieldTermStructure Piecewisecurve(List<RateHelper> instruments)
        {
            YieldTermStructure yieldts; // = FastActivator<PiecewiseYieldCurve>.Create(); 
            switch (_interpolationVariable)
            {
                case InterpolationVariable.Zero:
                    switch (_interpolationMethod)
                    {
                        case InterpolationMethod.Linear:
                            yieldts = new PiecewiseYieldCurve<ZeroYield, Linear>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        case InterpolationMethod.LogLinear:
                            yieldts = new PiecewiseYieldCurve<ZeroYield, LogLinear>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        case InterpolationMethod.NaturalCubic:
                            yieldts = new PiecewiseYieldCurve<ZeroYield, Cubic>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy, 
                                                                                            new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));

                            break;
                        case InterpolationMethod.FinancialCubic:
                            yieldts =new PiecewiseYieldCurve<ZeroYield, Cubic>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy,
                                                                                          new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));
                            break;
                        case InterpolationMethod.ConvexMonotone:
                            yieldts = new PiecewiseYieldCurve<ZeroYield, ConvexMonotone>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        default:
                            Utils.QL_FAIL("Interpolation method not recognised.");
                            throw new Exception();
                    }
                    break;
                case InterpolationVariable.Discount:
                    switch (_interpolationMethod)
                    {
                        case InterpolationMethod.Linear:
                            yieldts = new PiecewiseYieldCurve<Discount, Linear>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        case InterpolationMethod.LogLinear:
                            yieldts = new PiecewiseYieldCurve<Discount, LogLinear>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        case InterpolationMethod.NaturalCubic:
                            yieldts = new PiecewiseYieldCurve<Discount, Cubic>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy, new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));

                            break;
                        case InterpolationMethod.FinancialCubic:
                            yieldts = new PiecewiseYieldCurve<Discount, Cubic>(
                                _asofDate, instruments, _zeroDayCounter, null, null, _accuracy,
                                new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.FirstDerivative, 0.0));
                            break;
                        case InterpolationMethod.ConvexMonotone:
                            yieldts = new PiecewiseYieldCurve<Discount, ConvexMonotone>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        default:
                            Utils.QL_FAIL("Interpolation method not recognised.");
                            throw new Exception();
                    }
                    break;
                case InterpolationVariable.Forward:
                    switch (_interpolationMethod)
                    {
                        case InterpolationMethod.Linear:
                            yieldts = new PiecewiseYieldCurve<ForwardRate, Linear>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        case InterpolationMethod.LogLinear:
                            yieldts = new PiecewiseYieldCurve<ForwardRate, LogLinear>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);

                            break;
                        case InterpolationMethod.NaturalCubic:
                            yieldts = new PiecewiseYieldCurve<ForwardRate, Cubic>(
                                _asofDate, instruments, _zeroDayCounter,  null, null, _accuracy,
                                new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));
                            break;
                        case InterpolationMethod.FinancialCubic:
                            yieldts = new PiecewiseYieldCurve<ForwardRate, Cubic>(
                                _asofDate, instruments, _zeroDayCounter, null, null, _accuracy,
                                new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.FirstDerivative, 0.0));
                            break;
                        case InterpolationMethod.ConvexMonotone:
                            yieldts = new PiecewiseYieldCurve<ForwardRate, ConvexMonotone>(_asofDate, instruments, _zeroDayCounter, null, null, _accuracy);
                            break;

                        default:
                            Utils.QL_FAIL("Interpolation method not recognised.");
                            throw new Exception();
                    }
                    break;
                default:
                    Utils.QL_FAIL("Interpolation variable not recognised.");
                    throw new Exception();
            }

            // Build fixed zero/discount curve that matches the boostrapped curve
            // initially, but does NOT react to quote changes: This is a workaround
            // for a QuantLib problem, where a fixed reference date piecewise
            // yield curve reacts to evaluation date changes because the bootstrap
            // helper recompute their start date (because they are realtive date
            // helper for deposits, fras, swaps, etc.).
            InitializedList<Date> dates = new InitializedList<Date>(instruments.Count +1, _asofDate);
            InitializedList<double> zeros = new InitializedList<double>(instruments.Count +1, 0.0);
            InitializedList<double> discounts = new InitializedList<double>(instruments.Count +1, 1.0);
            InitializedList<double> forwards = new InitializedList<double>(instruments.Count +1, 0.0);

            if (_extrapolation)
            {
                yieldts.enableExtrapolation();
            }
            for (int i = 0; i < instruments.Count; i++)
            {
                dates[i + 1] = instruments[i].latestDate();
                zeros[i + 1] = yieldts.zeroRate(dates[i + 1], _zeroDayCounter, Compounding.Continuous).value();
                discounts[i + 1] = yieldts.discount(dates[i + 1]);
                forwards[i + 1] = yieldts.forwardRate(dates[i + 1], dates[i + 1], _zeroDayCounter, Compounding.Continuous).value();
            }
            zeros[0] = zeros[1];
            forwards[0] = forwards[1];
            if (_interpolationVariable == InterpolationVariable.Zero)
                _p = Zerocurve(dates, zeros, _zeroDayCounter);
            else if (_interpolationVariable == InterpolationVariable.Discount)
                _p = Discountcurve(dates, discounts, _zeroDayCounter);
            else if (_interpolationVariable == InterpolationVariable.Forward)
                _p = Forwardcurve(dates, forwards, _zeroDayCounter);
            else
                Utils.QL_FAIL("Interpolation variable not recognised.");


            return _p;
        }

        private YieldTermStructure Zerocurve(List<Date> dates, List<double> yields, DayCounter dayCounter)
        {
            YieldTermStructure yieldts; // = FastActivator<YieldTermStructure>.Create();
            switch (_interpolationMethod)
            {
                case InterpolationMethod.Linear:
                    yieldts= new InterpolatedZeroCurve<Linear>(dates, yields, dayCounter,  new Linear());
                    break;
                case InterpolationMethod.LogLinear:
                    yieldts= new InterpolatedZeroCurve<LogLinear>(dates, yields, dayCounter,  new LogLinear());
                    break;
                case InterpolationMethod.NaturalCubic:
                    yieldts= new InterpolatedZeroCurve<Cubic>(dates, yields, dayCounter,
                                                                              new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));
                    break;
                case InterpolationMethod.FinancialCubic:
                    yieldts= new InterpolatedZeroCurve<Cubic>(
                        dates, yields, dayCounter,
                         new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.FirstDerivative, 0.0));
                    break;
                case InterpolationMethod.ConvexMonotone:
                    yieldts= new InterpolatedZeroCurve< ConvexMonotone>(dates, yields, dayCounter);
                    break;
                default:
                    Utils.QL_FAIL("Interpolation method not recognised.");
                    throw new Exception();
            }
            return yieldts;
        }
                                                            
        private YieldTermStructure Discountcurve(List<Date> dates, List<double> dfs, DayCounter dayCounter)
        {
            YieldTermStructure yieldts;
            switch (_interpolationMethod)
            {
                case InterpolationMethod.Linear:
                    yieldts = new InterpolatedDiscountCurve<Linear>(dates, dfs, dayCounter, new Linear());
                    break;
                case InterpolationMethod.LogLinear:
                    yieldts = 
                        new InterpolatedDiscountCurve<LogLinear>(dates, dfs, dayCounter, new LogLinear());
                    break;
                case InterpolationMethod.NaturalCubic:
                    yieldts = new InterpolatedDiscountCurve<Cubic>(
                        dates, dfs, dayCounter, new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));
                    break;
                case InterpolationMethod.FinancialCubic:
                    yieldts = new InterpolatedDiscountCurve<Cubic>(
                        dates, dfs, dayCounter,
                        new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.FirstDerivative, 0.0));
                    break;
                case InterpolationMethod.ConvexMonotone:
                    yieldts = new InterpolatedDiscountCurve<ConvexMonotone>(dates, dfs, dayCounter);
                    break;
                default:
                    Utils.QL_FAIL("Interpolation method not recognised.");
                    throw new Exception();
            }
            return yieldts;
        }

        private YieldTermStructure Forwardcurve(List<Date> dates, List<double> forwards, DayCounter dayCounter)
        {
            YieldTermStructure yieldts;
            switch (_interpolationMethod)
            {
                case InterpolationMethod.Linear:
                    yieldts = new InterpolatedForwardCurve<Linear>(dates, forwards, dayCounter, new Linear());
                    break;
                case InterpolationMethod.LogLinear:
                    yieldts = 
                        new InterpolatedForwardCurve<LogLinear>(dates, forwards, dayCounter, new LogLinear());
                    break;
                case InterpolationMethod.NaturalCubic:
                    yieldts = new InterpolatedForwardCurve<Cubic>(dates, forwards, dayCounter,
                                                                                new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0));
                    break;
                case InterpolationMethod.FinancialCubic:
                    yieldts = new InterpolatedForwardCurve<Cubic>(
                        dates, forwards, dayCounter,
                        new Cubic(CubicInterpolation.DerivativeApprox.Kruger, true, CubicInterpolation.BoundaryCondition.SecondDerivative, 0.0, CubicInterpolation.BoundaryCondition.FirstDerivative, 0.0));
                    break;
                case InterpolationMethod.ConvexMonotone:
                    yieldts = new InterpolatedForwardCurve<ConvexMonotone>(dates, forwards, dayCounter);
                    break;
                default:
                    Utils.QL_FAIL("Interpolation method not recognised.");
                    throw new Exception();
            }
            return yieldts;
        }

        // Functions to build RateHelpers from yield curve segments
        private void AddDeposits(YieldCurveSegment segment, List<RateHelper> instruments)
        {

        }

        private void addFutures(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addFras(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addOISs(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addSwaps(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addAverageOISs(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addTenorBasisSwaps(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addTenorBasisTwoSwaps(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addFXForwards(YieldCurveSegment segment,RateHelper instruments)
        {

        }
        private void addCrossCcyBasisSwaps(YieldCurveSegment segment,RateHelper instruments)
        {

        }
    }
}
