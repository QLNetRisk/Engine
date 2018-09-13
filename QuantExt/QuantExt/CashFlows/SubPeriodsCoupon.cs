using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QuantExt
{
    /// <summary>
    /// Sub-periods coupon
    /// The coupon period tenor is a multiple of the tenor associated with
    /// the index. The index tenor divides the coupon period into sub-periods.
    /// The index fixing for each sub-period is compounded or averaged over
    /// the full coupon period.
    /// \ingroup cashflows
    /// </summary>
    public class SubPeriodsCoupon : FloatingRateCoupon
    {
        public enum Type { Averaging, Compounding };

        private Type _type;
        private bool _includeSpread;
        private List<Date> _valueDates;
        private List<Date> _fixingDates;
        private List<double> _fixings;
        private int _numPeriods;
        private List<double> _accrualFractions;

        public SubPeriodsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate,
                                InterestRateIndex index, Type type, BusinessDayConvention convention,
                                double spread = 0.0, DayCounter dayCounter = null, bool includeSpread = false, double gearing = 1.0)
            : base(paymentDate, nominal, startDate, endDate, index.fixingDays(), index, gearing, spread, new Date(), new Date(), dayCounter, false)
        {
            _type = type;
            _includeSpread = includeSpread;

            // Populate the value dates.
            Schedule sch = new MakeSchedule()
                               .from(startDate)
                               .to(endDate)
                               .withTenor(index.tenor())
                               .withCalendar(index.fixingCalendar())
                               .withConvention(convention)
                               .withTerminationDateConvention(convention)
                               .backwards().value();
            _valueDates = sch.dates();
            Utils.QL_REQUIRE(_valueDates.Count >= 2, () => "Degenerate schedule.");

            // Populate the fixing dates.
            _numPeriods = _valueDates.Count - 1;
            if (index.fixingDays() == 0)
            {
                _fixingDates = new List<Date>{ _valueDates.First(), _valueDates.Last() - 1};
            }
            else
            {
                _fixingDates.Resize(_numPeriods);
                for (int i = 0; i < _numPeriods; ++i)
                    _fixingDates[i] = index.fixingDate(_valueDates[i]);
            }

            // Populate the accrual periods.
            _accrualFractions.Resize(_numPeriods);
            for (int i = 0; i < _numPeriods; ++i)
            {
                _accrualFractions[i] = dayCounter.yearFraction(_valueDates[i], _valueDates[i + 1]);
            }
        }


        /// <summary>
        /// fixing dates for the sub-periods
        /// </summary>
        /// <returns></returns>
        public List<Date> FixingDates()
        {
            return _fixingDates;
        }
        /// <summary>
        /// accrual periods for the sub-periods
        /// </summary>
        /// <returns></returns>
        public List<double> AccrualFractions()
        {
            return _accrualFractions;
        }
        /// <summary>
        /// fixings for the sub-periods
        /// </summary>
        /// <returns></returns>
        public virtual List<double> IndexFixings()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// value dates for the sub-periods
        /// </summary>
        /// <returns></returns>
        public List<Date> ValueDates()
        {
            return _valueDates;
        }
        /// <summary>
        /// whether sub-period fixings are averaged or compounded
        /// </summary>
        /// <returns></returns>
        public Type CouponType()
        {
            return _type;
        }
        /// <summary>
        /// whether to include/exclude spread in compounding/averaging
        /// </summary>
        /// <returns></returns>
        public bool IncludeSpread()
        {
            return _includeSpread;
        }
        /// <summary>
        /// Need to be able to change spread to solve for fair spread
        /// </summary>
        /// <returns></returns>
        public double Spread()
        {
            return spread_;
        }        
        //@}
        //! \name FloatingRateCoupon interface
        //@{
        //! the date when the coupon is fully determined
        public Date FixingDate()
        {
            return _fixingDates.Last();
        }
    }
}
