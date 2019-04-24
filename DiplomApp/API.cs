using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace ClientApp
{
    public static class API
    {
        public static string Path = DiplomApp.Properties.Settings.Default.WebAppDomain;
        private static string AccessToken;

        public static async Task<bool> LoginAsync(string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "grant_type", "password" ),
                new KeyValuePair<string, string>( "Username", userName ),
                new KeyValuePair<string, string> ( "Password", password )
            };
            var content = new FormUrlEncodedContent(pairs);


            using (var client = new HttpClient())
            {
                HttpResponseMessage response = default;
                try
                {
                    response = await client.PostAsync(Path + "/api/Authentification", content);
                }

                catch (Exception)
                {
                    MessageBox.Show("Ошибка запроса к серверу. Приложение будет закрыто!", "Ошибка!", MessageBoxButton.OK);
                    Environment.Exit(1);
                }

                if (!response.IsSuccessStatusCode) return false;
                var result = await response.Content.ReadAsStringAsync();

                Dictionary<string, string> tokenDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                tokenDictionary.TryGetValue("access_token", out AccessToken);
            }
            return true;
        }

        public static void Logout()
        {
            AccessToken = null;
        }

        public static async Task<List<Dictionary<string, string>>> GetDevices(string userName)
        {
            using (var client = CreateClientWithToken(AccessToken))
            {
                var response =
                   await client.GetAsync(Path + $"/api/Things/GetDevices/{userName}");
#if DEBUG
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Bad Request: " + Path + $"/api/Things/GetDevices/{userName}");
                }
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest: throw new HttpRequestException();
                    case System.Net.HttpStatusCode.Unauthorized: throw new UnauthorizedAccessException();
                    default: break;
                }
#endif
                var result =
                  await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);
            }
        }

        private static HttpClient CreateClientWithToken(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
            return client;
        }

    }
}
