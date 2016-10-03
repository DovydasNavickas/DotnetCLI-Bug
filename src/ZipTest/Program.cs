using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ZipTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: true, reloadOnChange: true)
#if DEBUG
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.development.json"), optional: true, reloadOnChange: true)
#endif
                .Build();

            var host = new WebHostBuilder()
                .ConfigureLogging(options => options.AddConsole(configuration.GetSection("Logging")))
                .ConfigureLogging(options => options.AddDebug())
                .UseConfiguration(configuration)
                .UseIISIntegration()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
