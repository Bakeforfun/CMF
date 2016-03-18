using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsynchronousUpdate.Services;
using SciChart.Charting.Model.DataSeries;

namespace AsynchronousUpdate.ViewModels {
    public class SciChartViewModel : INotifyPropertyChanged {
        private string _caption;
        private bool _isIdle = true;
        private XyDataSeries<int, double> _line1Data;
        private XyDataSeries<int, double> _line2Data;
        private readonly SynchronizationContext _uiContext;

        public SciChartViewModel() {
            Caption = "Solution Explorer -> References -> Add new reference -> SciChart.Charting \n" +
                      "SciChart.Core, SciChart.Data, SciChart.Drwaing,\n" +
                      "add to ShellView link to SciChart UserControls xmlns:s=\"http://schemas.abtsoftware.co.uk/scichart\"\n" +
                      "all SciChart UserControls then maybe typed as <s:...></s:...> you may find it in code";

            _uiContext = SynchronizationContext.Current;
        }

        public string Caption {
            get { return _caption; }
            set {
                _caption = value;
                OnPropertyChanged();
            }
        }

        public XyDataSeries<int, double> Line1Data {
            get { return _line1Data; }
            set {
                _line1Data = value;
                OnPropertyChanged();
            }
        }

        public XyDataSeries<int, double> Line2Data {
            get { return _line2Data; }
            set {
                _line2Data = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => new DelegateCommand(StartUpdate, _ => _isIdle);

        private void StartUpdate(object obj) {
            _isIdle = false;

            Task.Run(() => {
                _uiContext.Post(_ => {
                    Line1Data = new XyDataSeries<int, double>();
                    Line2Data = new XyDataSeries<int, double>();
                }, null);

                var line1Value = 99.5;
                var line2Value = 100.5;
                var rnd = new Random();
                for (var i = 1; i < 1000; i++) {
                    _uiContext.Post(_ => {
                        Line1Data.Append(i, line1Value);
                        Line2Data.Append(i, line2Value);
                    }, null);
                    line1Value += rnd.NextDouble() - 0.5;
                    line2Value += rnd.NextDouble() - 0.5;
                    Thread.Sleep(5);
                }

                _isIdle = true;
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}