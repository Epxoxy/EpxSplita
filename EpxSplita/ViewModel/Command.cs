using System;
using System.ComponentModel;
using System.Windows.Input;

namespace EpxSplita
{
    class DelegateCommand : ICommand
    {
        //A method prototype without return value.
        public Action<object> ExecuteCommand = null;
        //A method prototype return a bool type.
        public Func<object, bool> CanExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
            {
                return this.CanExecuteCommand(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            ExecuteCommand?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(String propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
