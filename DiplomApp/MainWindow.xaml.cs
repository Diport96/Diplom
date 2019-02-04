using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomApp
{   
    public partial class MainWindow : Window
    {
        List<Controller> Controllers;

        public MainWindow()
        {
            InitializeComponent();

            Controllers = new List<Controller>();
            ServerDevice server = ServerDevice.GetInstance();
            server.OnControllerConnected += Server_OnControllerConnected;
            server.Run();
        }

        private void Server_OnControllerConnected(object sender, Dictionary<string, string> args)
        {
            throw new NotImplementedException();
        }

        private Controller BuildController(string id, string name, string type, string value)
        {
            var cType = (CType)Enum.Parse(typeof(CType), type);
            if (cType == CType.Termometer)
            {
                var resID = Guid.Parse(id);
                var resValue = double.Parse(value);
                return new Termometer(resID, name, resValue);
            }
            else
            {
                throw new InvalidEnumArgumentException(); //MessageBox Alert
            }
        }
    }
}
