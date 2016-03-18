using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsynchronousUpdate.Services;

namespace AsynchronousUpdate.ViewModels {
    public class SimpleUpdateViewModel : INotifyPropertyChanged {
        private string _asynchronousUpdateText;
        private int _badUpdate;
        private int _goodUpdate;
        private bool _isIdle = true;
        private string _loadingApplicationText;

        public SimpleUpdateViewModel() {
            LoadingApplicationText = "Application loading the foolowing way:\n" +
                                     "1. App.xaml has no value for StartupUri property setted for Application class\n" +
                                     "2. App.xaml.cs has overriden method OnStartup\n" +
                                     "3. In that method we create instance of Bootstrapper class and run its method Start()\n" +
                                     "4. This method creates window ShellView and show it\n" +
                                     "5. ViewModel for ShellView window is set for each TabItem separately\n" +
                                     "   you may find it in <TabItem.DataContext>...</TabItem.DataContext>";

            AsynchronousUpdateText = "TextBlocks below are binded to values BadUpdate and GoodUpdate,\n" +
                                     "Command of Start Button below is binded to StartCommand, which runs StartUpdate(object param).\n" +
                                     "Method updates value of BadUpdate in same Thread,\n" +
                                     "while GoodUpdate updates as new Task (starts it in different thread).\n" +
                                     "Please notice that Thread.Sleep(1) used only for numbers not to blink too fast.";
        }

        public int BadUpdate {
            get { return _badUpdate; }
            set {
                _badUpdate = value;
                OnPropertyChanged();
            }
        }

        public int GoodUpdate {
            get { return _goodUpdate; }
            set {
                _goodUpdate = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => new DelegateCommand(StartUpdate, _ => _isIdle);

        public string LoadingApplicationText {
            get { return _loadingApplicationText; }
            set {
                _loadingApplicationText = value;
                OnPropertyChanged();
            }
        }

        public string AsynchronousUpdateText {
            get { return _asynchronousUpdateText; }
            set {
                _asynchronousUpdateText = value;
                OnPropertyChanged();
            }
        }

        public void StartUpdate(object param) {
            _isIdle = false;

            for (var i = 0; i < 10000; i++)
                BadUpdate += 1;

            Task.Run(() => {
                for (var i = 0; i < 10000; i++) {
                    GoodUpdate += 1;
                    Thread.Sleep(1);
                }
            });

            _isIdle = true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}