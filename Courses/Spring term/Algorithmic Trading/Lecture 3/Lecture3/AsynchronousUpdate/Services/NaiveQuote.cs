using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsynchronousUpdate.Services {
    public class NaiveQuote : INotifyPropertyChanged {
        private double _ask;
        private double _bid;

        public NaiveQuote(string name) {
            Name = name;
        }

        public string Name { get; private set; }

        public double Bid {
            get { return _bid; }
            set {
                _bid = value;
                OnPropertyChanged();
            }
        }

        public double Ask {
            get { return _ask; }
            set {
                _ask = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}