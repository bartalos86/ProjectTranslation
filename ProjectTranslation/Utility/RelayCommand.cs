using System;
using System.Windows.Input;

namespace ProjectTranslation
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action mAction;
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction.Invoke();
        }
    }
}
