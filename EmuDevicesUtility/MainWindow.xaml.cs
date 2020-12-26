using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace EmuDevicesUtility
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        internal ObservableCollection<Controller> Controllers { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            Controllers = new ObservableCollection<Controller>();
            CbType.ItemsSource = Enum.GetValues(typeof(CType)).Cast<CType>();
            CbType.SelectedIndex = 0;
            TStack.ItemsSource = Controllers;

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
            if (!int.TryParse(TCount.Text, out var count)) { }// throw new ArgumentException();
            if (CbType.SelectedItem == null) { }// throw new ArgumentNullException();
            for (var i = 0; i < count; i++)
            {
                if ((CType)CbType.SelectedValue == CType.Sensor)
                    Controllers.Add(new Sensor(CType.Sensor + " " + i, 20));
                if ((CType)CbType.SelectedValue == CType.Switch)
                    Controllers.Add(new Switch(CType.Switch + " " + i, false));
                if ((CType)CbType.SelectedValue == CType.Thermometer)
                    Controllers.Add(new Termometer(CType.Thermometer + " " + i, 20));
            }
            OnPropertyChanged(nameof(Controllers));
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
