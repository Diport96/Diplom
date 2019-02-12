using DiplomApp.Server;
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

        public MainWindow()
        {
            InitializeComponent();
                        
            server = ServerDevice.Instance;            
            server.RunAsync();            
            Closed += MainWindow_Closed;            
        }
        
        private void MainWindow_Closed(object sender, EventArgs e)
        {
           server.StopAsync();
        }

        ~MainWindow()
        {
            server.StopAsync();
        }
    }
}
