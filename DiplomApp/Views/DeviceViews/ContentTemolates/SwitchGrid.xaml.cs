using System;
using System.Collections.Generic;
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

namespace ClientApp.DeviceViews.Grids
{
    /// <summary>
    /// Логика взаимодействия для SwitchGrid.xaml
    /// </summary>
    public partial class SwitchGrid : UserControl
    {
        public SwitchGrid(string name, bool state)
        {
            InitializeComponent();
            SwitchName.Text = name;
            EnableChesck.IsChecked = state;
        }
    }
}
