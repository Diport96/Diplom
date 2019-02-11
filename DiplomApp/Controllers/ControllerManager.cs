using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Controllers
{
    static class ControllerManager
    {
        static readonly List<Controller> controllers;
        public static IEnumerable<CType> SupportedTypes;

        static ControllerManager()
        {
            controllers = new List<Controller>();
            SupportedTypes = new List<CType>()
            {
                CType.Termometer
            }.AsReadOnly();

        }

        public static void Add(Controller controller)
        {
            controllers.Add(controller);
        }
        public static void Remove<T>(Func<T,bool> predicate) where T : Controller
        {
            //!!!!
            throw new NotImplementedException();
        }           
    }
}
