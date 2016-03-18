using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsynchronousUpdate.Services;

namespace AsynchronousUpdate.ViewModels {
    public class AdvancedUpdateViewModel : INotifyPropertyChanged {
        private readonly SynchronizationContext _uiContext;
        private string _caption;
        private bool _isIdle = true;
        private ObservableCollection<NaiveQuote> _quotes;

        public AdvancedUpdateViewModel() {
            Caption = "When updating with ObservableCollection (populating grid)\n" +
                      "we need to update it in same SynchronizationContext (same thread) in which our GUI is running.\n" +
                      "It's value may be obtained in viewmodel constructor, as View and ViewModel created in one thread.\n\n" +
                      "Please note that BaseClass for ObservableCollection also uses interface INotifyPropertyChanged.\n\n" +
                      "All question either to StackOverflow.com or book Pro Synchronous programming with .NET";

            _uiContext = SynchronizationContext.Current;
        }

        public ObservableCollection<NaiveQuote> Quotes {
            get { return _quotes; }
            set {
                _quotes = value;
                OnPropertyChanged();
            }
        }

        public string Caption {
            get { return _caption; }
            set {
                _caption = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => new DelegateCommand(StartUpdate, _ => _isIdle);

        public void StartUpdate(object param) {
            _isIdle = false;

            Task.Run(() => {
                var quote1 = new NaiveQuote("Asset1") {
                    Bid = 99.5,
                    Ask = 100.5
                };
                var quote2 = new NaiveQuote("Asset1") {
                    Bid = 99.5,
                    Ask = 100.5
                };

                // OBSERVABLE COLLECTION MAY BE MODIFIED ONLY IN SAME CONTEXT
                _uiContext.Post(_ => {
                    Quotes = new ObservableCollection<NaiveQuote> {
                        quote1,
                        quote2
                    };
                }, null);

                // If you would uncomment this code, it will show you an error
                //
                // Quotes.Add(new NaiveQuote("Some item"));
                //
                // But this code will work
                //
                //_uiContext.Post(_ => {
                //    Quotes.Add(new NaiveQuote("Some item"));
                //}, null);

                for (var i = 0; i < 1000; i++) {
                    var rnd = new Random();
                    quote1.Bid += rnd.NextDouble();
                    quote1.Ask += rnd.NextDouble();
                    quote2.Bid += rnd.NextDouble();
                    quote2.Ask += rnd.NextDouble();
                    Thread.Sleep(5);
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