using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyBudgetController.ViewModel
{
    public class CommandVM : ICommand
    {

        public Action _Execute;

        public CommandVM(Action ExecuteMethod)
        {
            _Execute = ExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _Execute();
        }

    }

    public class CommandVM<T> : ICommand
    {

        public Action<T> _Execute;

        public CommandVM(Action<T> ExecuteMethod)
        {
            _Execute = ExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _Execute((T)parameter);
        }

    }
}
