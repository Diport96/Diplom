using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MqttWebApp.Areas.Identity.IdentityHostingStartup))]
namespace MqttWebApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}