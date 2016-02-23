using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BaseEntities;
using OptionPricing.Annotations;
using OptionPricing.Services;
using OptionPricingModels;

namespace OptionPricing.ViewModels {
    public class PricingShellViewModel : INotifyPropertyChanged {
        private readonly Option _option;
        private ObservableCollection<OptionExerciseType> _optionExerciseTypes;
        private ObservableCollection<Option> _optionsPortfolio = new ObservableCollection<Option>();
        private ObservableCollection<OptionStyle> _optionStyles;
        private ObservableCollection<OptionType> _optionTypes;
        private ObservableCollection<OptionPricingModel> _pricingModels;
        private ObservableCollection<NameValue<double>> _results;

        public PricingShellViewModel() {
            _option = new Option();
            OptionStyles =
                new ObservableCollection<OptionStyle>(Enum.GetValues(typeof (OptionStyle)).Cast<OptionStyle>());
            OptionExerciseTypes =
                new ObservableCollection<OptionExerciseType>(
                    Enum.GetValues(typeof (OptionExerciseType)).Cast<OptionExerciseType>());
            OptionTypes = new ObservableCollection<OptionType>(Enum.GetValues(typeof (OptionType)).Cast<OptionType>());
            PricingModels =
                new ObservableCollection<OptionPricingModel>(
                    Enum.GetValues(typeof (OptionPricingModel)).Cast<OptionPricingModel>());
        }

        public string Underlying {
            get { return _option.Instrument.Name; }
            set {
                _option.Instrument.Name = value;
                OnPropertyChanged();
            }
        }

        public double UnderlyingPrice {
            get { return _option.UnderlyingPrice; }
            set {
                _option.UnderlyingPrice = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OptionStyle> OptionStyles {
            get { return _optionStyles; }
            set {
                _optionStyles = value;
                OnPropertyChanged();
            }
        }

        public OptionStyle SelectedStyle {
            get { return _option.Style; }
            set {
                _option.Style = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OptionExerciseType> OptionExerciseTypes {
            get { return _optionExerciseTypes; }
            set {
                _optionExerciseTypes = value;
                OnPropertyChanged();
            }
        }

        public OptionExerciseType SelectedExerciseType {
            get { return _option.ExerciseType; }
            set {
                _option.ExerciseType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OptionPricingModel> PricingModels {
            get { return _pricingModels; }
            set {
                _pricingModels = value;
                OnPropertyChanged();
            }
        }

        public OptionPricingModel SelectedPricingModel {
            get { return _option.Model; }
            set {
                _option.Model = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OptionType> OptionTypes {
            get { return _optionTypes; }
            set {
                _optionTypes = value;
                OnPropertyChanged();
            }
        }

        public OptionType SelectedType {
            get { return _option.Type; }
            set {
                _option.Type = value;
                OnPropertyChanged();
            }
        }

        public bool IsBuy {
            get { return _option.Side == Side.Buy; }
            set {
                _option.Side = value ? Side.Buy : Side.Sell;
                OnPropertyChanged();
            }
        }

        public double StrikePrice {
            get { return _option.Strike; }
            set {
                _option.Strike = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StrikePricePct));
            }
        }

        public double StrikePricePct => _option.PctStrike;

        public DateTime ValuationDate {
            get { return _option.ValuationDateTime; }
            set {
                _option.ValuationDateTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeToExpiry));
            }
        }

        public DateTime ExpirationDate {
            get { return _option.ExpirationDateTime; }
            set {
                _option.ExpirationDateTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeToExpiry));
            }
        }

        public TimeSpan TimeToExpiry => _option.TimeToExpiry;

        public double Volatility {
            get { return _option.Volatility; }
            set {
                _option.Volatility = value;
                OnPropertyChanged();
            }
        }

        public double InterestRate {
            get { return _option.InterestRate; }
            set {
                _option.InterestRate = value;
                OnPropertyChanged();
            }
        }

        public ICommand ComputeCommand => new DelegateCommand(Compute);
        public ICommand AddCommand => new DelegateCommand(AddOption);

        public ObservableCollection<NameValue<double>> Results {
            get { return _results; }
            set {
                _results = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Option> OptionsPortfolio {
            get { return _optionsPortfolio; }
            set {
                _optionsPortfolio = value;
                OnPropertyChanged();
            }
        }

        public double PortfolioCashDelta => OptionsPortfolio.Sum(p => p.CashDelta);
        public double PortfolioCashGamma => OptionsPortfolio.Sum(p => p.CashGamma);
        public double PortfolioCashVega => OptionsPortfolio.Sum(p => p.CashVega);
        public double PortfolioCashTheta => OptionsPortfolio.Sum(p => p.CashTheta);
        public double PortfolioCashRho => OptionsPortfolio.Sum(p => p.CashRho);

        public int Quantity {
            get { return _option.Quantity; }
            set {
                _option.Quantity = value;
                OnPropertyChanged();
            }
        }

        private void AddOption(object obj) {
            OptionsPortfolio.Add(ObjectCopier.Clone(_option));
        }

        private void Compute(object param) {
            _option.Compute();
            Results = new ObservableCollection<NameValue<double>> {
                new NameValue<double>("Option Price", _option.OptionPrice),
                new NameValue<double>("Delta", _option.Delta),
                new NameValue<double>("Cash Delta", _option.CashDelta),
                new NameValue<double>("Gamma", _option.Gamma),
                new NameValue<double>("Cash Gamma", _option.CashGamma),
                new NameValue<double>("Vega", _option.Vega),
                new NameValue<double>("Cash Vega", _option.CashVega),
                new NameValue<double>("Theta", _option.Theta),
                new NameValue<double>("Cash Theta", _option.CashTheta),
                new NameValue<double>("Rho", _option.Rho),
                new NameValue<double>("Cash Rho", _option.CashRho)
            };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}