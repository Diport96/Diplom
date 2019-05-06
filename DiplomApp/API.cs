using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using NLog;

namespace ClientApp
{
    public static class API
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
                catch (Exception e)
                {
                    logger.Error(e, "Ошибка запроса на аутентификацию пользователя");
                    return false;
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

        public static async Task SubmitDevicesDataAsync()
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("connectionString", "mongodb://localhost/DevicesData")
            };

            var content = new FormUrlEncodedContent(pairs);
            using (var client = CreateClientWithToken(AccessToken))
            {
                var response = await client.PostAsync(Path + "/api/Authentification/SubmitDeviceData", content);
            }
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
