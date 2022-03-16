
using Client.App.BusinessManager;
using Client.App.BusinessManager.Interface;
using Client.App.Infrastructure;
using Client.App.Services;
using Client.App.Services.Interface;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ConsoleDependencyInjection
{
    public class Program
    {

        private static ILogger _logger = null;
        private static IServiceProvider? _serviceProvider = null;
        private static IConfiguration? _configuration = null;

        static async Task<int> Main(string[] args)
        {
            var cts = new CancellationTokenSource();

         
            #region [ Create Service Collection ]

            var services = new ServiceCollection();
            ConfigureServices(services);

            #endregion

            #region [  create service provider ]
          
            _serviceProvider = services.BuildServiceProvider();

            IServiceScope scope = _serviceProvider.CreateScope();

            #endregion

            #region Global Exception

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                _logger.Information("Exit");
                cts.Cancel();
            };

            #endregion

            var transactionService = scope.ServiceProvider.GetRequiredService<ISimpleTransaction>();
            
            if(transactionService != null)
               await transactionService.RunAsync();
           
            try
            {        
                await Task.Delay(-1, cts.Token);
            }
            catch (OperationCanceledException) { }
            return 0;
        }
       
        private static void ConfigureServices(IServiceCollection services)
        {

            #region build config

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("client.app.appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            #endregion
            #region Logging
            // configure logging

            _logger = new LoggerConfiguration()
                     .WriteTo.Debug()
                     .WriteTo.Console()
                     .ReadFrom.Configuration(_configuration)
                     .CreateLogger();
            
            services.AddLogging(builder =>
            {
                builder.AddSerilog(logger: _logger, dispose: true);
            });

            #endregion



            #region [Http Client Configuration]
            
            services.AddTransient<LoggingDelegatingHandler>();
           

            services.AddHttpClient<IAuthentication, Authentication>(c =>
             c.BaseAddress = new Uri(_configuration["ApiSettings:GatewayAddress"]))
             .AddHttpMessageHandler<LoggingDelegatingHandler>()
             .AddPolicyHandler(Resilience.GetRetryPolicy())
             .AddPolicyHandler(Resilience.Timeout())
             .AddPolicyHandler(Resilience.GetCircuitBreakerPolicy());
            
            services.AddHttpClient<ITransaction, Transaction>(c =>
             c.BaseAddress = new Uri(_configuration["ApiSettings:GatewayAddress"]))
             .AddHttpMessageHandler<LoggingDelegatingHandler>()
             .AddPolicyHandler(Resilience.GetRetryPolicy())
             .AddPolicyHandler(Resilience.Timeout())
             .AddPolicyHandler(Resilience.GetCircuitBreakerPolicy());


            #endregion

            #region add services:

            services.AddMemoryCache();

            services.AddTransient<ISimpleTransaction, SimpleTransaction>();
          
            services.AddTransient<IGenericMemoryCache, GenericMemoryCache>();


            #endregion

        }


    }
}