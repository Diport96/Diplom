using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiplomApp.ViewModels
{
    // Реализовать ObservableCollection. MVVM dialog window whth answer
    class SelectSensorDialogViewModel : BaseViewModel
    {
        private readonly Action<bool> dialogResultWindowAction;
        private RelayCommand submitSelectionCommand;
        private RelayCommand cancelSelectionCommand;

        public RelayCommand SubmitSelectionCommand
        {
            get
            {
                return submitSelectionCommand ??
                    (submitSelectionCommand = new RelayCommand(obj => SubmitSelection()));
            }
        }
        public RelayCommand CancelSelectionCommand
        {
            get
            {
                return cancelSelectionCommand ??
                    (cancelSelectionCommand = new RelayCommand(obj => CancelSelection()));
            }
        }
        public IEnumerable<Controller> Sensors { get; }
        public Controller SelectedSensor { get; set; }

        public SelectSensorDialogViewModel(Action<bool> dialogResultWindow)
        {
            Sensors = App.ControllersFactory.GetControllers().Where(x => x is Sensor);
            dialogResultWindowAction = dialogResultWindow;
        }

        private void SubmitSelection()
        {
            if (SelectedSensor != null)
            {
                dialogResultWindowAction(true);
            }
        }
        private void CancelSelection()
        {
            dialogResultWindowAction(false);
        }
    }
}
