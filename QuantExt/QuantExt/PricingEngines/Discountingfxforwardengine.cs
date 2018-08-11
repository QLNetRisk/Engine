using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QuantExt
{
    public class DiscountingFxForwardEngine : FxForward.Engine
    {
        private Currency _ccy1;
        private Handle<YieldTermStructure> _currency1Discountcurve;
        private Currency _ccy2;
        private Handle<YieldTermStructure> _currency2Discountcurve;
        private Handle<Quote> _spotFX;
        private bool _includeSettlementDateFlows;
        private Date _settlementDate;
        private Date _npvDate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ccy1"></param>
        /// <param name="currency1Discountcurve"></param>
        /// <param name="ccy2"></param>
        /// <param name="currency2Discountcurve"></param>
        /// <param name="spotFX"></param>
        /// <param name="includeSettlementDateFlows"></param>
        /// <param name="settlementDate"></param>
        /// <param name="npvDate"></param>
        public DiscountingFxForwardEngine(Currency ccy1, Handle<YieldTermStructure> currency1Discountcurve, Currency ccy2, Handle<YieldTermStructure> currency2Discountcurve, Handle<Quote> spotFX, bool includeSettlementDateFlows, Date settlementDate, Date npvDate)
        {
            _ccy1 = ccy1;
            _currency1Discountcurve = currency1Discountcurve;
            _ccy2 = ccy2;
            _currency2Discountcurve = currency2Discountcurve;
            _spotFX = spotFX;
            _includeSettlementDateFlows = includeSettlementDateFlows;
            _settlementDate = settlementDate;
            _npvDate = npvDate;

            _currency1Discountcurve.registerWith(update);
            _currency2Discountcurve.registerWith(update);
            _spotFX.registerWith(update);      
        }

        // Inspectors
        public Handle<YieldTermStructure> Currency1Discountcurve()
        {
            return _currency1Discountcurve;
        }

        public Handle<YieldTermStructure> Currency2Discountcurve()
        {
            return _currency2Discountcurve;
        }

        public Currency Currency1()
        {
            return _ccy1;
        }

        public Currency Currency2()
        {
            return _ccy2;
        }

        public Handle<Quote> spotFX()
        {
            return _spotFX;
        }

        public override void calculate()
        {
            Date npvDate = _npvDate;

            if (npvDate == new Date())
            {
                npvDate = _currency1Discountcurve.link.referenceDate();
            }
            Date settlementDate = _settlementDate;
            if (settlementDate == new Date())
            {
                settlementDate = npvDate;
            }

            double tmpNominal1;
            double tmpNominal2;
            bool tmpPayCurrency1;

            if (_ccy1 == arguments_.Currency1)
            {
                QLNet.Utils.QL_REQUIRE(_ccy2 == arguments_.Currency2, 
                    () => "mismatched currency pairs (" + _ccy1 + "," + _ccy2 + ") in the egine and (" + arguments_.Currency1 + "," + arguments_.Currency2 + ") in the instrument");

                tmpNominal1 = arguments_.Nominal1;
                tmpNominal2 = arguments_.Nominal2;
                tmpPayCurrency1 = arguments_.PayCurrency1;
            }
            else
            {
                QLNet.Utils.QL_REQUIRE(_ccy1 == arguments_.Currency2 && _ccy2 == arguments_.Currency1,
                           () => "mismatched currency pairs (" + _ccy1 + "," + _ccy2 + ") in the egine and (" + arguments_.Currency1 + "," + arguments_.Currency2 + ") in the instrument");

                tmpNominal1 = arguments_.Nominal2;
                tmpNominal2 = arguments_.Nominal1;
                tmpPayCurrency1 = !arguments_.PayCurrency1;
            }

            QLNet.Utils.QL_REQUIRE(!_currency1Discountcurve.empty() && !_currency2Discountcurve.empty(), () => "Discounting term structure handle is empty.");
            QLNet.Utils.QL_REQUIRE(_currency1Discountcurve.link.referenceDate() == _currency2Discountcurve.link.referenceDate(), () => "Term structures should have the same reference date.");
            QLNet.Utils.QL_REQUIRE(arguments_.MaturityDate >= _currency1Discountcurve.link.referenceDate(), () => "FX forward maturity should exceed or equal the discount curve reference date.");

            results_.value = 0.0;
            results_.FairForwardRate = new ExchangeRate(_ccy2, _ccy1, tmpNominal1 / tmpNominal2); // strike rate

            // TODO: How to implement this simple event  interface?
            //if (!detail::simple_event(arguments_.MaturityDate).hasOccurred(settlementDate, _includeSettlementDateFlows))
            
            if (!(new simple_event(arguments_.MaturityDate)).hasOccurred(settlementDate, _includeSettlementDateFlows))
            {
                double disc1near = _currency1Discountcurve.link.discount(npvDate);
                double disc1far = _currency1Discountcurve.link.discount(arguments_.MaturityDate);
                double disc2near = _currency2Discountcurve.link.discount(npvDate);
                double disc2far = _currency2Discountcurve.link.discount(arguments_.MaturityDate);
                double fxfwd = disc1near / disc1far * disc2far / disc2near * _spotFX.link.value();
                
                results_.value = (tmpPayCurrency1 ? -1.0 : 1.0) * disc1far / disc1near * (tmpNominal1 - tmpNominal2 * fxfwd);
                results_.FairForwardRate = new ExchangeRate(_ccy2, _ccy1, fxfwd);
            }
            results_.NPV = new Money(_ccy1, results_.value.Value);
        }
    }
}
