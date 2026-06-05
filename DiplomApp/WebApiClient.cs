using System.Threading.Tasks;

namespace ClientApp
{
    /// <summary>
    /// Абстракция над веб-API приложения для возможности подмены в тестах
    /// </summary>
    internal interface IWebApiClient
    {
        Task<bool> LoginAsync(string userName, string password);
        Task<string> GetConnectionStringAsync();
    }

    /// <summary>
    /// Реализация по умолчанию, делегирующая статическому <see cref="API"/>
    /// </summary>
    internal sealed class WebApiClient : IWebApiClient
    {
        public Task<bool> LoginAsync(string userName, string password)
            => API.LoginAsync(userName, password);

        public Task<string> GetConnectionStringAsync()
            => API.GetConnectionStringAsync();
    }
}
