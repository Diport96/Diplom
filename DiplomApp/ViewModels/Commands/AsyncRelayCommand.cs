using System;
using System.Threading.Tasks;

namespace DiplomApp.ViewModels.Commands
{
    class AsyncRelayCommand : AsyncCommandBase
    {
        private readonly Func<object, Task> execute;
        private readonly Func<object, bool> canExecute;

        public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        public override Task ExecuteAsync(object parameter)
        {
            return execute(parameter);
        }
    }
}
