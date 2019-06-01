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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DiplomApp.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string userHelloTitle;
        private string serverStartStopButtonContent;
        private readonly Window ownerWindow;
        private Controller selectedDevice;
        private RelayCommand signOutCommand;
        private RelayCommand deviceSettingsCommand;
        private AsyncRelayCommand serverStartStopCommand;

        public RelayCommand SignOutCommand
        {
            get
            {
                return signOutCommand ??
                    (signOutCommand = new RelayCommand(obj => SignOut(ownerWindow)));
            }
        }
        public RelayCommand DeviceSettingsCommand
        {
            get
            {
                return deviceSettingsCommand ??
                    (deviceSettingsCommand = new RelayCommand(obj => DeviceSettings(obj as Controller), obj => false));
            }
        }
        public AsyncRelayCommand ServerStartStopCommand
        {
            get
            {
                return serverStartStopCommand ??
                    (serverStartStopCommand = new AsyncRelayCommand(obj => ServerStartStopAsync(App.Server)));
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
        public Controller SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindowViewModel(string username, bool isLocalSession, Func<Task<bool>> connectToWebApp, Window owner)
        {
            ownerWindow = owner;
            UserHelloTitle = $"Здравствуйте {username}";
            IsLocalSession = isLocalSession;
            ApplicationSettingsCommand = new ApplicationSettingsCommand();
            Task.Run(() => ConnectingToWebApp(connectToWebApp));

            Controllers = ControllersFactory.Controllers;
            App.Server.PropertyChanged += Server_PropertyChanged;
            SetServerStartStopButtonContent(App.Server.IsRun);
        }

        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
        private async Task ServerStartStopAsync(ServerDevice server)
        {
            if (server.IsRun) await server.StopAsync();
            else await server.RunAsync();
        }
        private void SignOut(Window owner)
        {
            AccountManager.Logout();
            new AuthentificationWindow().Show();
            owner.Close();
        }
        private void DeviceSettings(Controller selectedDevice)
        {
            if (selectedDevice is Switch)
                new SwitchSettingsWindow(selectedDevice.ID).ShowDialog();
            else if (selectedDevice is Sensor)
                new SensorSettingsWindow(selectedDevice.ID).ShowDialog();
        }
        private void SetServerStartStopButtonContent(bool serverIsRunningStatus)
        {
            if (serverIsRunningStatus) ServerStartStopButtonContent = "Остановить сервер";
            else ServerStartStopButtonContent = "Запустить сервер";
        }

        ~MainWindowViewModel()
        {
            App.Server.StopAsync().Wait();
        }
    }
}
