using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
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
using System.Windows.Shapes;

namespace DiplomApp.Views
{
    public partial class SelectSensorDialogWindow : Window
    {
        public Controller Answer { get; private set; }
       

        public SelectSensorDialogWindow()
        {
            InitializeComponent();

            StackOfDevices.ItemsSource = ControllersFactory.GetControllers().Where(x => x is Sensor);

            StackOfDevices.SelectionChanged += StackOfDevices_SelectionChanged;
        }

        private void StackOfDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Answer = StackOfDevices.SelectedItem as Controller;            
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
