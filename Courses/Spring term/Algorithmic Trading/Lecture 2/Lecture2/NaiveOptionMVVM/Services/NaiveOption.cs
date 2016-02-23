using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NaiveOptionMVVM.Annotations;

namespace NaiveOptionMVVM.Services {
    public class NaiveOption : INotifyPropertyChanged {
        private double _interestRate = 0.02;
        private double _strikePrice = 100;
        private double _timeToExpiration = 90.0/365;
        private OptionType _type = OptionType.Call;
        private double _underlyingPrice = 100;
        private double _volatility = 0.3;

        public OptionType Type {
            get { return _type; }
            set {
                _type = value;
                OnPropertyChanged();
                changedPropertyForResults();
            }
        }

        public double UnderlyingPrice {
            get { return _underlyingPrice; }
            set {
                _underlyingPrice = value;
                OnPropertyChanged();
                changedPropertyForResults();
            }
        }

        public double StrikePrice {
            get { return _strikePrice; }
            set {
                _strikePrice = value;
                OnPropertyChanged();
                changedPropertyForResults();
            }
        }

        public double TimeToExpiration {
            get { return _timeToExpiration; }
            set {
                _timeToExpiration = value;
                OnPropertyChanged();
                changedPropertyForResults();
            }
        }

        public double InterestRate {
            get { return _interestRate; }
            set {
                _interestRate = value;
                OnPropertyChanged();
                changedPropertyForResults();
            }
        }

        public double Volatility {
            get { return _volatility; }
            set {
                _volatility = value;
                OnPropertyChanged();
                changedPropertyForResults();
            }
        }

        private double _d1
            =>
                (Math.Log(UnderlyingPrice/StrikePrice) + (InterestRate + Volatility*Volatility/2)*TimeToExpiration)/
                (Volatility*Math.Sqrt(TimeToExpiration));

        private double _d2 => _d1 - Volatility*Math.Sqrt(TimeToExpiration);

        public double Price {
            get {
                if (Type == OptionType.Call)
                    return UnderlyingPrice*NormalDistribution.Phi(_d1) -
                           StrikePrice*Math.Pow(Math.E, -InterestRate*TimeToExpiration)*NormalDistribution.Phi(_d2);
                return -UnderlyingPrice*NormalDistribution.Phi(-_d1) +
                       StrikePrice*Math.Pow(Math.E, -InterestRate*TimeToExpiration)*NormalDistribution.Phi(-_d2);
            }
        }

        public double Delta {
            get {
                if (Type == OptionType.Call)
                    return NormalDistribution.Phi(_d1);
                return NormalDistribution.Phi(_d1) - 1;
            }
        }

        public double CashDelta => Delta*UnderlyingPrice;
        public double Gamma => NormalDistribution.Density(_d1)/(UnderlyingPrice*Volatility*Math.Sqrt(TimeToExpiration));
        public double CashGamma => Gamma*UnderlyingPrice*UnderlyingPrice/100;

        public double Theta {
            get {
                if (Type == OptionType.Call)
                    return -(UnderlyingPrice*NormalDistribution.Density(_d1)*Volatility)/(2*Math.Sqrt(TimeToExpiration)) -
                           InterestRate*StrikePrice*Math.Pow(Math.E, -InterestRate*TimeToExpiration)*
                           NormalDistribution.Phi(_d2);
                return -(UnderlyingPrice*NormalDistribution.Density(_d1)*Volatility)/(2*Math.Sqrt(TimeToExpiration)) +
                       InterestRate*StrikePrice*Math.Pow(Math.E, -InterestRate*TimeToExpiration)*
                       NormalDistribution.Phi(-_d2);
            }
        }

        public double CashTheta => Theta/365;
        public double Vega => UnderlyingPrice*Math.Sqrt(TimeToExpiration)*NormalDistribution.Density(_d1);
        public double CashVega => Vega/100;

        public double Rho {
            get {
                if (Type == OptionType.Call)
                    return StrikePrice*TimeToExpiration*Math.Pow(Math.E, -InterestRate*TimeToExpiration)*
                           NormalDistribution.Phi(_d2);
                return -StrikePrice*TimeToExpiration*Math.Pow(Math.E, -InterestRate*TimeToExpiration)*
                       NormalDistribution.Phi(-_d2);
            }
        }

        public double CashRho => Rho/100;

        private void changedPropertyForResults() {
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(Delta));
            OnPropertyChanged(nameof(CashDelta));
            OnPropertyChanged(nameof(Gamma));
            OnPropertyChanged(nameof(CashGamma));
            OnPropertyChanged(nameof(Vega));
            OnPropertyChanged(nameof(CashVega));
            OnPropertyChanged(nameof(Theta));
            OnPropertyChanged(nameof(CashTheta));
            OnPropertyChanged(nameof(Rho));
            OnPropertyChanged(nameof(CashRho));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}