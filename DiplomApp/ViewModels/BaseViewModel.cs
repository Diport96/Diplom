using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiplomApp.ViewModels
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected readonly Action closingWindowAction;
        protected readonly Action<bool> dialogResultWindowAction;

        public BaseViewModel(Action closingWindow, Action<bool> dialogResultWindow)
        {
            closingWindowAction = closingWindow;
            dialogResultWindowAction = dialogResultWindow;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
