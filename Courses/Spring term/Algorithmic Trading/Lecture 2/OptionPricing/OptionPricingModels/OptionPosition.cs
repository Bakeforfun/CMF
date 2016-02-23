// (c) Konstantin Brownstein 2016, except implementation

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BaseEntities;
using OptionPricingModels.Annotations;
using OptionPricingModels.PricingModels;

namespace OptionPricingModels {
    [Serializable]
    public class OptionPosition : INotifyPropertyChanged {
        private double _delta;
        private DateTime _expirationDateTime = DateTime.Now.AddDays(90);
        private double _gamma;
        private double _interestRate = 0.05;
        private double _optionPrice;
        private int _quantity = 1;
        private double _rho;
        private double _strike = 100;
        private double _theta;
        private double _underlyingPrice = 100;
        private DateTime _valuationDateTime = DateTime.Today;
        private double _vega;
        private double _volatility = 0.3;
        public int Id { get; set; }
        private int _sign => Side == Side.Buy ? 1 : -1;
        public Instrument Instrument { get; set; } = new Instrument("BASE");
        public Side Side { get; set; } = Side.Buy;

        public int Quantity {
            get { return _quantity; }
            set {
                if (_quantity <= 0) ThrowIfLessThanZero(nameof(Quantity));
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public double UnderlyingPrice {
            get { return _underlyingPrice; }
            set {
                if (value <= 0) ThrowIfLessThanZero(nameof(UnderlyingPrice));
                _underlyingPrice = value;
                OnPropertyChanged();
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

        public DateTime ValuationDateTime {
            get { return _valuationDateTime; }
            set {
                _valuationDateTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeToExpiry));
            }
        }

        public DateTime ExpirationDateTime {
            get {
                if (_expirationDateTime.Date < ValuationDateTime.Date)
                    _expirationDateTime = ValuationDateTime.AddMonths(3);
                return _expirationDateTime;
            }
            set {
                _expirationDateTime = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan TimeToExpiry => ExpirationDateTime - ValuationDateTime;

        public double Volatility {
            get { return _volatility; }
            set {
                if (value <= 0) ThrowIfLessThanZero(nameof(Volatility));
                _volatility = value;
                OnPropertyChanged();
            }
        }

        public double InterestRate {
            get { return _interestRate; }
            set {
                _interestRate = value;
                OnPropertyChanged();
            }
        }

        public double CostPrice { get; set; }
        public double PL => _sign*(PositionPrice - CostPrice);

        public double OptionPrice {
            get { return _optionPrice; }
            set {
                _optionPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PositionPrice));
                OnPropertyChanged(nameof(PL));
                OnPropertyChanged(nameof(PctPrice));
            }
        }

        public double PctPrice => OptionPrice/UnderlyingPrice;
        public double PositionPrice => OptionPrice*Quantity;

        public double Delta {
            get { return _delta; }
            set {
                _delta = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CashDelta));
                OnPropertyChanged(nameof(PositionDelta));
            }
        }

        public double PositionDelta => Delta*Quantity;
        public double CashDelta => Delta*UnderlyingPrice*Quantity;

        public double Gamma {
            get { return _gamma; }
            set {
                _gamma = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CashGamma));
            }
        }

        public double CashGamma => Gamma*UnderlyingPrice*UnderlyingPrice*Quantity/100;

        public double Vega {
            get { return _vega; }
            set {
                _vega = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CashVega));
            }
        }

        public double CashVega => Vega*Quantity/100;

        public double Theta {
            get { return _theta; }
            set {
                _theta = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CashTheta));
            }
        }

        public double CashTheta => Theta*Quantity/365;

        public double Rho {
            get { return _rho; }
            set {
                _rho = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CashRho));
            }
        }

        public double CashRho => Rho*Quantity/100;

        public void Compute() {
            PricingModelFacade.Compute(this);
        }

        private void ThrowIfLessThanZero(string s) {
            // For simplixcity purposes I remove error checking
            //throw new InvalidOperationException($"{s} should be more than 0");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}