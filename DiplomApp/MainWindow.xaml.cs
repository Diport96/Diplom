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
            args.TryGetValue("ID", out string id);
            args.TryGetValue("Name", out string name);
            args.TryGetValue("Type", out string t);
            args.TryGetValue("Value", out string val);

            var guid = Guid.Parse(id);
            var type = (CType)Enum.Parse(typeof(CType), t);
            var value = double.Parse(val);

            switch (type)
            {
                case CType.Switch:
                    break;
                case CType.Termometer: Controllers.Add(new Termometer(guid, name, value));
                    break;
                default:
                    break;
            }
        }               
    }
}
