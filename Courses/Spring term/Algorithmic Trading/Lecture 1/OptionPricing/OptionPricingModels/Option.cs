// (c) Konstantin Brownstein 2016, except implementation

using System;
using BaseEntities;
using OptionPricingModels.PricingModels;

namespace OptionPricingModels {
    [Serializable]
    public class Option {
        private DateTime _expirationDateTime = DateTime.Now;
        private double _interestRate = 0.05;
        private int _quantity = 1;
        private double _strike = 100;
        private double _underlyingPrice = 100;
        private double _volatility = 0.3;
        public Instrument Instrument { get; set; } = new Instrument("BASE");
        public Side Side { get; set; } = Side.Buy;

        public int Quantity {
            get { return _quantity; }
            set {
                if (_quantity <= 0) ThrowIfLessThanZero(nameof(Quantity));
                _quantity = value;
            }
        }

        public double UnderlyingPrice {
            get { return _underlyingPrice; }
            set {
                if (value <= 0) ThrowIfLessThanZero(nameof(UnderlyingPrice));
                _underlyingPrice = value;
            }
        }

        public OptionStyle Style { get; set; } = OptionStyle.Vanilla;
        public OptionExerciseType ExerciseType { get; set; } = OptionExerciseType.European;
        public OptionType Type { get; set; } = OptionType.Call;
        public OptionPricingModel Model { get; set; } = OptionPricingModel.BlackScholes;

        public double Strike {
            get { return _strike; }
            set {
                if (value <= 0) ThrowIfLessThanZero(nameof(Strike));
                _strike = value;
            }
        }

        public double PctStrike {
            get {
                if (Math.Abs(UnderlyingPrice) > 0.00001) return Strike/UnderlyingPrice;
                return double.NaN;
            }
        }

        public DateTime ValuationDateTime { get; set; } = DateTime.Today;

        public DateTime ExpirationDateTime {
            get {
                if (_expirationDateTime.Date <= ValuationDateTime)
                    _expirationDateTime = ValuationDateTime.AddMonths(3);
                return _expirationDateTime;
            }
            set { _expirationDateTime = value; }
        }

        public TimeSpan TimeToExpiry => ExpirationDateTime - ValuationDateTime;

        public double Volatility {
            get { return _volatility; }
            set {
                if (value <= 0) ThrowIfLessThanZero(nameof(Volatility));
                _volatility = value;
            }
        }

        public double InterestRate {
            get { return _interestRate; }
            set {
                if (value <= 0) ThrowIfLessThanZero(nameof(InterestRate));
                _interestRate = value;
            }
        }

        public double OptionPrice { get;  set; }
        public double Delta { get;  set; }
        public double CashDelta => 0; // TODO : Find and append formula for CashDelta
        public double Gamma { get;  set; }
        public double CashGamma => 0; // TODO : Find and append formula for CashGamma 
        public double Vega { get;  set; }
        public double CashVega => 0; // TODO : Find and append formula for CashVega (vol shift 1%)
        public double Theta { get;  set; }
        public double CashTheta => 0; // TODO : Find and append formula for CashTheta (time shift one day)
        public double Rho { get;  set; }
        public double CashRho => 0; // TODO : Find and append formula for CashRho (interest rate shift 1%)

        public void Compute() {
            PricingModelFacade.Compute(this);
        }

        private void ThrowIfLessThanZero(string s) {
            throw new InvalidOperationException($"{s} should be more than 0");
        }
    }
}