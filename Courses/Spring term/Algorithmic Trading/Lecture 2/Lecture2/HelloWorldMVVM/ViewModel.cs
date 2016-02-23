using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HelloWorldMVVM {
    public class ViewModel : INotifyPropertyChanged {
        private string _text1;
        private string _text2;

        public string Text1 {
            get { return _text1; }
            set {
                _text1 = value;
                OnPropertyChanged();
            }
        }

        public string Text2 {
            get { return _text2; }
            set {
                _text2 = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}