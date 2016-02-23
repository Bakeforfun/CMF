using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NaiveOptionMVVM.Services;

namespace NaiveOptionMVVM {
    public class ViewModel : INotifyPropertyChanged {
        private NaiveOption _option;
        public NaiveOption Option { get { return _option; } set { _option = value; OnPropertyChanged(); } }
        private ObservableCollection<OptionType> _optionTypes;
        public ObservableCollection<OptionType> OptionTypes { get { return _optionTypes; } set { _optionTypes = value; OnPropertyChanged(); } }

        public ViewModel() {
            Option = new NaiveOption();
            OptionTypes = new ObservableCollection<OptionType>(Enum.GetValues(typeof(OptionType)).Cast<OptionType>());
        }







        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}