using System.Threading.Tasks;
using System.Windows.Input;

namespace DiplomApp.ViewModels.Commands
{
    interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
