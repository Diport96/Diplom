using DiplomApp.Controllers;
using DiplomApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DiplomApp.Models;

namespace DiplomApp.ViewModels
{
    // Реализовать ObservableCollection. MVVM dialog window whth answer
    class SelectSensorDialogViewModel : DialogBaseViewModel
    {
        public IEnumerable<Controller> Sensors { get; }
        public Controller SelectedSensor { get; set; }

        public SelectSensorDialogViewModel(Action<bool> dialogResultWindow)
            : base(dialogResultWindow)
        {
            Sensors = App.ControllersFactory.GetControllers().Where(x => x is Sensor);
        }

        protected override void Submit()
        {
            if (SelectedSensor != null)
            {
                dialogResultWindowAction(true);
            }
        }
        protected override void Cancel()
        {
            dialogResultWindowAction(false);
        }
    }
}
