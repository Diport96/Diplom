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
    /// <summary>
    /// Предоставляет набор методов для взаимодействия с API веб-приложения
    /// </summary>
    public static class API
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static string AccessToken;

        /// <summary>
        /// URL адрес веб-приложения
        /// </summary>
        public static string Path = DiplomApp.Properties.Settings.Default.WebAppUrl;

        /// <summary>
        /// Аминхронный запрос на удаленную аутентификацию пользователя
        /// </summary>
        /// <param name="userName">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Удалось ли выполнить удаленную аутентификацию</returns>
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

        /// <summary>
        /// Производит выход из удаленного аккаунта пользователя
        /// </summary>
        public static void Logout()
        {
            AccessToken = null;
        }

        /// <summary>
        /// Получает строку подключения к базе дыанных веб-приложения MongoDB
        /// </summary>
        /// <returns>Строка подключения к базе данных</returns>
        public static async Task<string> GetConnectionStringAsync()
        {
            using (var client = CreateClientWithToken(AccessToken))
            {                
                var response = await client.GetAsync(Path + "/api/Authentification/GetConnectionString");
                if(response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<string>(message);
                    return result;
                }
                else
                {
                    // !!!
                    return default;
                }
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
