using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QuantExt
{
    public class FxForward : QLNet.Instrument
    {
        private double _nominal1;
        private Currency _currency1;
        private double _nominal2;
        private Currency _currency2;
        private Date _maturityDate;
        private bool _payCurrency1;

        private ExchangeRate _fairForwardRate;

        private Money _npv;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nominal1"></param>
        /// <param name="nominal2"></param>
        /// <param name="currency1"></param>
        /// <param name="currency2"></param>
        /// <param name="maturityDate"></param>
        /// <param name="payCurrency1"></param>
        public FxForward(double nominal1, Currency currency1, double nominal2, Currency currency2, Date maturityDate, bool payCurrency1)
        {
            _nominal1 = nominal1;
            _currency1 = currency1;
            _nominal2 = nominal2;
            _currency2 = currency2;
            _maturityDate = maturityDate;
            _payCurrency1 = payCurrency1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nominal1"></param>
        /// <param name="forwardRate"></param>
        /// <param name="maturityDate"></param>
        /// <param name="sellingNominal"></param>
        public FxForward(Money nominal1, ExchangeRate forwardRate, Date maturityDate, bool sellingNominal)
        {
            _nominal1 = nominal1.value;
            _currency1 = nominal1.currency;
            _maturityDate = maturityDate;
            _payCurrency1 = sellingNominal;

            QLNet.Utils.QL_REQUIRE(_currency1 == forwardRate.target, () => "Currency of nominal1 does not match target (domestic) currency in the exchange rate.");

            Money otherNominal = forwardRate.exchange(nominal1);
            _nominal2 = otherNominal.value;
            _currency2 = otherNominal.currency;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nominal1"></param>
        /// <param name="fxForwardQuote"></param>
        /// <param name="currency2"></param>
        /// <param name="maturityDate"></param>
        /// <param name="sellingNominal"></param>
        public FxForward(Money nominal1, Handle<Quote> fxForwardQuote, Currency currency2, Date maturityDate, bool sellingNominal)
        {
            _nominal1 = nominal1.value;
            _currency1 = nominal1.currency;
            _maturityDate = maturityDate;
            _payCurrency1 = sellingNominal;

            QLNet.Utils.QL_REQUIRE(fxForwardQuote.link.isValid(), () => "The FX Forward quote is not valid.");

            _nominal2 = _nominal1 / fxForwardQuote.link.value();
        }

        public override bool isExpired()
        {
            Date today = Settings.evaluationDate();
            return today > _maturityDate;
        }

        protected override void setupExpired()
        {
            base.setupExpired();
            _npv = new Money(0.0, new Currency());
            _fairForwardRate = new ExchangeRate();
        }

        public override void setupArguments(QLNet.IPricingEngineArguments args)
        {            
            FxForward.Arguments arguments = args as FxForward.Arguments;

            // TODO: Consider implementing overloaded QL_REQUIRE that takes an object as argument and checks for null
            QLNet.Utils.QL_REQUIRE(arguments != null, () => "wrong argument type in fxforward");

            arguments.Nominal1 = _nominal1;
            arguments.Currency1 = _currency1;
            arguments.Nominal2 = _nominal2;
            arguments.Currency2 = _currency2;
            arguments.MaturityDate = _maturityDate;
            arguments.PayCurrency1 = _payCurrency1;
        }

        public override void fetchResults(QLNet.IPricingEngineResults r)
        {
            base.fetchResults(r);

            FxForward.Results results = r as FxForward.Results;

            QLNet.Utils.QL_REQUIRE(results != null, () => "wrong result type");
            Money npv_ = results.NPV;
            _fairForwardRate = results.FairForwardRate;
        }

        public class Arguments : QLNet.IPricingEngineArguments
        {
            public double Nominal1 { get; set; }            
            public Currency Currency1 { get; set; }
            public double Nominal2 { get; set; }
            public Currency Currency2 { get; set; }
            public Date MaturityDate { get; set; }
            public bool PayCurrency1 { get; set; }            

            public virtual void validate()
            {
                QLNet.Utils.QL_REQUIRE(Nominal1 > 0.0, () => "nominal1  should be positive: " + Nominal1);
                QLNet.Utils.QL_REQUIRE(Nominal2 > 0.0, () => "nominal2 should be positive: " + Nominal2);
            }
        }

        public new class Results : QLNet.Instrument.Results
        {
            public Money NPV { get; set; }
            public ExchangeRate FairForwardRate { get; set; }

            public override void reset()
            {
                base.reset();

                // clear all previous results
                NPV = new Money(0.0, new Currency());
                FairForwardRate = new ExchangeRate();
            }
        }

        public class Engine : QLNet.GenericEngine<FxForward.Arguments, FxForward.Results> { }        
    }
}
