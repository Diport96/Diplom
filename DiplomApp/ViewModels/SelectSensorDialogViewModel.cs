using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.ViewModels.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiplomApp.ViewModels
{
    // Реализовать ObservableCollection. MVVM dialog window whth answer
    class SelectSensorDialogViewModel
    {
        private readonly Window owner;
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

        public SelectSensorDialogViewModel(Window owner)
        {
            this.owner = owner;
            Sensors = ControllersFactory.GetControllers().Where(x => x is Sensor);
        }

        private void SubmitSelection()
        {
            if (SelectedSensor != null)
            {
                owner.DialogResult = true;
            }
        }
        private void CancelSelection()
        {
            owner.DialogResult = false;
        }
    }
}
