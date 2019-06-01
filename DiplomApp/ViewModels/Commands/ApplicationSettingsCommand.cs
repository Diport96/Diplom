using DiplomApp.Views;
using System;
using System.Windows.Input;

namespace DiplomApp.ViewModels.Commands
{
    class ApplicationSettingsCommand : ICommand
    {
        private readonly Func<object, bool> canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ApplicationSettingsCommand(Func<object, bool> canExecute = null)
        {
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            new ApplicationSettingsWindow().ShowDialog();
        }
    }
}
