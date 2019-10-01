using DiplomApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.ViewModels
{
    abstract class DialogBaseViewModel : BaseViewModel
    {
        protected readonly Action<bool> dialogResultWindowAction;
        private RelayCommand submitButtonCommand;
        private RelayCommand cancelButtonCommand;

        public RelayCommand SubmitButtonCommand
        {
            get
            {
                return submitButtonCommand ??
                    (submitButtonCommand = new RelayCommand(obj => Submit()));
            }
        }
        public RelayCommand CancelButtonCommand
        {
            get
            {
                return cancelButtonCommand ??
                    (cancelButtonCommand = new RelayCommand(obj => Cancel()));
            }
        }

        public DialogBaseViewModel(Action<bool> dialogResultWindow)
        {
            dialogResultWindowAction = dialogResultWindow;
        }

        protected abstract void Submit();
        protected abstract void Cancel();
    }
}
