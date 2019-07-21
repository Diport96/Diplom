using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.Controllers;
using DiplomApp.Controllers.Models;
using DiplomApp.Data;
using DiplomApp.Server;
using DiplomApp.ViewModels.Commands;
using DiplomApp.Views;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiplomApp.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private string userHelloTitle;
        private string serverStartStopButtonContent;
        private AsyncRelayCommand signOutCommand;
        private RelayCommand deviceSettingsCommand;
        private AsyncRelayCommand serverStartStopCommand;

        public AsyncRelayCommand SignOutCommand
        {
            get
            {
                return signOutCommand ??
                    (signOutCommand = new AsyncRelayCommand(obj => SignOut(App.Server)));
            }
        }
        public RelayCommand DeviceSettingsCommand
        {
            get
            {
                return deviceSettingsCommand ??
                    (deviceSettingsCommand = new RelayCommand(obj => DeviceSettings(obj as Controller)));
            }
        }
        public AsyncRelayCommand ServerStartStopCommand
        {
            get
            {
                return serverStartStopCommand ??
                    (serverStartStopCommand = new AsyncRelayCommand(obj => ServerStartStopAsync(App.Server, obj as Button)));
            }
        }
        public ApplicationSettingsCommand ApplicationSettingsCommand { get; }

        public string UserHelloTitle
        {
            get { return userHelloTitle; }
            set
            {
                userHelloTitle = value;
                OnPropertyChanged("UserHelloTitle");
            }
        }
        public string ServerStartStopButtonContent
        {
            get { return serverStartStopButtonContent; }
            set
            {
                serverStartStopButtonContent = value;
                OnPropertyChanged("ServerStartStopButtonContent");
            }
        }
        public bool IsLocalSession { get; private set; }
        public ObservableCollection<Controller> Controllers { get; }

        public MainWindowViewModel(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp, Action closingWindow, Action<bool> dialogResultWindow)
            : base(closingWindow, dialogResultWindow)
        {
            UserHelloTitle = $"Здравствуйте {username}";
            IsLocalSession = isLocalSession;
            ApplicationSettingsCommand = new ApplicationSettingsCommand();
            Task.Run(() => ConnectingToWebApp(connectToWebApp));

            Controllers = App.ControllersFactory.Controllers;
            App.Server.PropertyChanged += Server_PropertyChanged;
            SetServerStartStopButtonContent(App.Server.IsRun);
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRun") SetServerStartStopButtonContent(App.Server.IsRun);
        }
        private async Task ConnectingToWebApp(Func<Task<bool>> connectToWebApp)
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
        private async Task ServerStartStopAsync(IMqttProtocolManager server, Button button)
        {
            button.IsEnabled = false;
            if (server.IsRun) await server.StopAsync();
            else await server.RunAsync();
            button.IsEnabled = true;
        }
        private void SetServerStartStopButtonContent(bool serverIsRunningStatus)
        {
            if (serverIsRunningStatus) ServerStartStopButtonContent = "Остановить сервер";
            else ServerStartStopButtonContent = "Запустить сервер";
        }
        private void DeviceSettings(Controller device)
        {
            if (device is Switch)
                new SwitchSettingsWindow(device.ID).ShowDialog();
            else if (device is Sensor)
                new SensorSettingsWindow(device.ID).ShowDialog();
        }
        private async Task SignOut(IMqttProtocolManager server)
        {
            if (server.IsRun) await server.StopAsync();
            App.UserAccountManager.Logout();
            new AuthentificationWindow().Show();
            closingWindowAction();
        }

        ~MainWindowViewModel()
        {
            App.Server.Stop();
        }
    }
}
