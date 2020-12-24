using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EmuDevicesUtility
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        internal ObservableCollection<Controller> Controllers { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            Controllers = new ObservableCollection<Controller>();
            CBType.ItemsSource = Enum.GetValues(typeof(CType)).Cast<CType>();
            CBType.SelectedIndex = 0;
            tStack.ItemsSource = Controllers;

            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            foreach (var item in Controllers)
            {
                item.Remove();
            }
        }

        private void GenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TCount.Text)) { }//throw new ArgumentNullException();
            if (!int.TryParse(TCount.Text, out int count)) { }// throw new ArgumentException();
            if (CBType.SelectedItem == null) { }// throw new ArgumentNullException();
            for (int i = 0; i < count; i++)
            {
                if ((CType)CBType.SelectedValue == CType.Sensor)
                    Controllers.Add(new Sensor(CType.Sensor + " " + i.ToString(), 20));
                if ((CType)CBType.SelectedValue == CType.Switch)
                    Controllers.Add(new Switch(CType.Switch + " " + i.ToString(), false));
                if ((CType)CBType.SelectedValue == CType.Termometer)
                    Controllers.Add(new Termometer(CType.Termometer + " " + i.ToString(), 20));
            }
            OnPropertyChanged("Controllers");
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TStack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                if (item is Switch @switch)
                {
                    @switch.SetValue(true);
                }
                else if (item is Sensor sensor)
                {
                    sensor.SetValue(40);
                }
            }
        }
    }
}
