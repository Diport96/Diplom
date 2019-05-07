using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using DiplomApp.Server;
using DiplomApp.Views;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

namespace DiplomApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly ServerDevice server;
        public bool IsLocalSession { get; private set; }
        public MainWindow(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp)
        {
            InitializeComponent();

            server = App.Server;
            HelloLabel.Content = $"Здравствуйте {username}";
            IsLocalSession = isLocalSession;
            StackOfDevices.ItemsSource = ControllersFactory.GetControllers();
            SetServerStartStopButtonState(server, serverStartStopButton);
            Closed += MainWindow_Closed;
                     
            Task.Run(() => ConnectingToWebAppTask(connectToWebApp));
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            server.StopAsync().Wait();
        }
        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            AccountManager.Logout();
            new AuthentificationWindow().Show();
            Close();
        }
        private void DeviceItemSettings_Click(object sender, RoutedEventArgs e)
        {
            var device = ((e.Source as Button).DataContext as Controller);

            if (device is Switch)
                new SwitchSettingsWindow(device.ID).ShowDialog();
            else if (device is Sensor)
                new SensorSettingsWindow(device.ID).ShowDialog();
        }
        private void Application_Settings_Click(object sender, RoutedEventArgs e)
        {
            new ApplicationSettingsWindow().ShowDialog();
        }
        private async void Server_Start_Stop_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            btn.IsEnabled = false;
            if (server.IsRun)
                await server.StopAsync();
            else
                await server.RunAsync();
            SetServerStartStopButtonState(server, btn);
            btn.IsEnabled = true;
        }

        private void SetServerStartStopButtonState(ServerDevice server, Button button)
        {
            if (server.IsRun)
                button.Content = "Остановить сервер";
            else
                button.Content = "Запустить сервер";
        }
        private async Task ConnectingToWebAppTask(Func<Task<bool>> connectToWebApp)
        {
            Thread.Sleep(Properties.Settings.Default.AutoSendDataEvery);
            if (IsLocalSession)
            {
                while (!await connectToWebApp())
                {
                    Thread.Sleep(Properties.Settings.Default.AutoSendDataEvery);
                }
                var connectionString = await API.GetConnectionStringAsync();
                MongoDbInstance.Instance.SetDatabase(connectionString);
                IsLocalSession = false;
                await SubmitDataToRemoteDatabase(IsLocalSession);
            }
        }
        private async Task SubmitDataToRemoteDatabase(bool isLocalSession)
        {
            if (!isLocalSession)
            {
                var client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString);
                var database = client.GetDatabase("DevicesData");

                using (var collectionNamesCursor = await database.ListCollectionNamesAsync())
                {
                    while (await collectionNamesCursor.MoveNextAsync())
                    {
                        var collectionNames = collectionNamesCursor.Current;
                        foreach (var collectionName in collectionNames)
                        {
                            var collection = database.GetCollection<BsonDocument>(collectionName);
                            var remoteCollection = MongoDbInstance.Instance.Database.GetCollection<BsonDocument>(collectionName);
                            await remoteCollection.InsertManyAsync(collection.Find(new BsonDocument()).ToEnumerable());
                            await collection.DeleteManyAsync(new BsonDocument());
                        }
                    }
                }

            }
        }
    }
}
