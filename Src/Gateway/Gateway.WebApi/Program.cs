using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Gateway.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("Gateway.appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"Gateway.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true)
                      .AddJsonFile($"Gateway.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.configuration.json",
                            optional: true, reloadOnChange: true)
                       
                        .AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
    }
}
