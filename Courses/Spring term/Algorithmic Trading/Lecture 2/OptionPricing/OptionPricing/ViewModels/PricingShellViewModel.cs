using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BaseEntities;
using OptionPricing.Annotations;
using OptionPricing.Services;
using OptionPricingModels;

namespace OptionPricing.ViewModels {
    public class PricingShellViewModel : INotifyPropertyChanged {
        private OptionPosition _currentOption;
        private int _idCounter = 1;
        private ObservableCollection<OptionExerciseType> _optionExerciseTypes;
        private ObservableCollection<OptionPosition> _optionsPortfolio = new ObservableCollection<OptionPosition>();
        private ObservableCollection<OptionStyle> _optionStyles;
        private ObservableCollection<OptionType> _optionTypes;
        private double _prevRateShift;
        private double _prevVolShift;
        private ObservableCollection<OptionPricingModel> _pricingModels;
        private double _rateShift;
        private double _volShift;

        public PricingShellViewModel() {
            _currentOption = new OptionPosition();
            OptionStyles =
                new ObservableCollection<OptionStyle>(Enum.GetValues(typeof (OptionStyle)).Cast<OptionStyle>());
            OptionExerciseTypes =
                new ObservableCollection<OptionExerciseType>(
                    Enum.GetValues(typeof (OptionExerciseType)).Cast<OptionExerciseType>());
            OptionTypes = new ObservableCollection<OptionType>(Enum.GetValues(typeof (OptionType)).Cast<OptionType>());
            PricingModels =
                new ObservableCollection<OptionPricingModel>(
                    Enum.GetValues(typeof (OptionPricingModel)).Cast<OptionPricingModel>());
            computeThis();
            OptionsPortfolio.CollectionChanged += (sender, args) => computePortfolio();
        }

        public double VolShift {
            get { return _volShift; }
            set {
                _volShift = value;
                OnPropertyChanged();
                foreach (var option in OptionsPortfolio) {
                    option.Volatility += value - _prevVolShift;
                }
                _prevVolShift = value;
                computePortfolio();
            }
        }

        public double RateShift {
            get { return _rateShift; }
            set {
                _rateShift = value;
                OnPropertyChanged();
                foreach (var option in OptionsPortfolio) {
                    option.InterestRate += value - _prevRateShift;
                }
                _prevRateShift = value;
                computePortfolio();
            }
        }

        public string Underlying {
            get { return _currentOption.Instrument.Name; }
            set {
                _currentOption.Instrument.Name = value;
                OnPropertyChanged();
            }
        }

        public OptionPosition CurrentOption {
            get { return _currentOption; }
            set {
                _currentOption = value;
                OnPropertyChanged();
            }
        }

        public double UnderlyingPrice {
            get { return _currentOption.UnderlyingPrice; }
            set {
                _currentOption.UnderlyingPrice = value;
                _currentOption.Compute();
                OnPropertyChanged();
                foreach (var option in OptionsPortfolio) {
                    option.UnderlyingPrice = value;
                }
                computeThis();
                computePortfolio();
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
            get { return _currentOption.Style; }
            set {
                _currentOption.Style = value;
                OnPropertyChanged();
                computeThis();
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
            get { return _currentOption.ExerciseType; }
            set {
                _currentOption.ExerciseType = value;
                OnPropertyChanged();
                computeThis();
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
            get { return _currentOption.Model; }
            set {
                _currentOption.Model = value;
                OnPropertyChanged();
                computeThis();
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
            get { return _currentOption.Type; }
            set {
                _currentOption.Type = value;
                OnPropertyChanged();
                computeThis();
            }
        }

        public bool IsBuy {
            get { return _currentOption.Side == Side.Buy; }
            set {
                _currentOption.Side = value ? Side.Buy : Side.Sell;
                OnPropertyChanged();
            }
        }

        public double StrikePrice {
            get { return _currentOption.Strike; }
            set {
                _currentOption.Strike = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StrikePricePct));
                computeThis();
            }
        }

        public double StrikePricePct => _currentOption.PctStrike;

        public DateTime ValuationDate {
            get { return _currentOption.ValuationDateTime; }
            set {
                _currentOption.ValuationDateTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeToExpiry));
                foreach (var option in OptionsPortfolio) {
                    option.ValuationDateTime = value;
                }
                computePortfolio();
                computeThis();
            }
        }

        public DateTime ExpirationDate {
            get { return _currentOption.ExpirationDateTime; }
            set {
                _currentOption.ExpirationDateTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeToExpiry));
                computeThis();
            }
        }

        public TimeSpan TimeToExpiry => _currentOption.TimeToExpiry;

        public double Volatility {
            get { return _currentOption.Volatility; }
            set {
                _currentOption.Volatility = value;
                OnPropertyChanged();
                computeThis();
            }
        }

        public double InterestRate {
            get { return _currentOption.InterestRate; }
            set {
                _currentOption.InterestRate = value;
                OnPropertyChanged();
                computeThis();
            }
        }

        public ICommand AddCommand => new DelegateCommand(addOption);

        public ObservableCollection<OptionPosition> OptionsPortfolio {
            get { return _optionsPortfolio; }
            set {
                _optionsPortfolio = value;
                OnPropertyChanged();
            }
        }

        public double PL => OptionsPortfolio.Sum(p => p.PL);
        public double PortfolioDelta => OptionsPortfolio.Sum(p => p.PositionDelta);
        public double PortfolioCashDelta => OptionsPortfolio.Sum(p => p.CashDelta);
        public double PortfolioCashGamma => OptionsPortfolio.Sum(p => p.CashGamma);
        public double PortfolioCashVega => OptionsPortfolio.Sum(p => p.CashVega);
        public double PortfolioCashTheta => OptionsPortfolio.Sum(p => p.CashTheta);
        public double PortfolioCashRho => OptionsPortfolio.Sum(p => p.CashRho);

        public int Quantity {
            get { return _currentOption.Quantity; }
            set {
                _currentOption.Quantity = value;
                OnPropertyChanged();
                computeThis();
            }
        }

        private void addOption(object obj) {
            _currentOption.Id = _idCounter;
            _currentOption.CostPrice = _currentOption.PositionPrice;
            var option = ObjectCopier.Clone(_currentOption);
            option.PropertyChanged += evaluatePortfolio;
            OptionsPortfolio.Add(option);
            _idCounter++;
            computePortfolio();
        }

        private void evaluatePortfolio(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            OnPropertyChanged(nameof(PL));
            OnPropertyChanged(nameof(PortfolioDelta));
            OnPropertyChanged(nameof(PortfolioCashDelta));
            OnPropertyChanged(nameof(PortfolioCashGamma));
            OnPropertyChanged(nameof(PortfolioCashRho));
            OnPropertyChanged(nameof(PortfolioCashTheta));
            OnPropertyChanged(nameof(PortfolioCashVega));
        }

        private void computeThis() {
            CurrentOption.Compute();
        }

        private void computePortfolio() {
            foreach (var option in OptionsPortfolio) {
                option.Compute();
            }
            evaluatePortfolio(null, null);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            try {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}