// (c) Konstantin Brownstein 2016

using System;
using System.Windows.Input;

namespace OptionPricing.Services {
    public class DelegateCommand : ICommand {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(Action<object> execute)
            : this(execute, null) { }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute) {
            _execute = execute;
            _canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }
    }
}