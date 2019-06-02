using ClientApp;
using DiplomApp.Accounts;
using DiplomApp.Controllers;
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiplomApp.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string userHelloTitle;
        private string serverStartStopButtonContent;
        private readonly Window ownerWindow;
        private RelayCommand signOutCommand;
        private AsyncRelayCommand serverStartStopCommand;
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand SignOutCommand
        {
            get
            {
                return signOutCommand ??
                    (signOutCommand = new RelayCommand(obj => SignOut(ownerWindow)));
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
        private async Task ServerStartStopAsync(ServerDevice server, Button button)
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
        private void SignOut(Window owner)
        {
            AccountManager.Logout();
            new AuthentificationWindow().Show();
            owner.Close();
        }      

        ~MainWindowViewModel()
        {
            App.Server.StopAsync().Wait();
        }
    }
}
