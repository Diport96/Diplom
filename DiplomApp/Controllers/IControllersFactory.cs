using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiplomApp.Controllers
{
    interface IControllersFactory
    {
        ObservableCollection<Controller> Controllers { get; }

        void Create(Controller controller, string controllerType);
        void Remove(string id);
        IEnumerable<Controller> GetControllers();
        Controller GetById(string id);
        RegisteredDeviceInfo GetControllerInfo(string id);
        Type GetType(string Type);
    }
}
