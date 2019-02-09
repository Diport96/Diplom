using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        static Logger logger = LogManager.GetCurrentClassLogger();
        static ServerDevice server;
        List<Controller> controllers;

        public MainWindow()
        {
            InitializeComponent();

            controllers = new List<Controller>();
            server = ServerDevice.Instance;
            server.OnControllerMessageReceived += Server_OnControllerConnected;
            server.RunAsync();            
            Closed += MainWindow_Closed;            
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
                case CType.Termometer:
                    controllers.Add(new Termometer(guid, name, value));
                    //LOG
                    logger.Debug($"Добавлен новый контроллер с названием: {name}. Текущее количество: {controllers.Count}.");
                    break;
                default:
                    break;
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
           server.Stop();
        }

        ~MainWindow()
        {
            server.Stop();
        }
    }
}
