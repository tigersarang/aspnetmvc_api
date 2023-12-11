using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HtmlTest
{
    public class RelayCommand : ICommand
    {   
        public event EventHandler? CanExecuteChanged;
        private Action<object?> _execute;
        private Predicate<object?> _canExecute;

        public RelayCommand(Action<object?> execute) : this(execute, null)
        {            
        }
        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this._canExecute == null ? true : this._canExecute.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            this._execute.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
