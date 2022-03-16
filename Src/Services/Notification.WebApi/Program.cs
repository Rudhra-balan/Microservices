




using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Observability;
using Notification.WebApi;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

#region [Configure Services]

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile(
        $"Notification.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.log.json",
        true, true);
    config.AddJsonFile("Notification.appsettings.json", true, true);
    config.AddJsonFile($"Notification.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
    config.AddEnvironmentVariables();
});
builder.WebHost.UseUrls("http://*:5003");
builder.Host.UseSerilog(SerilogConfiguration.Configure);

if (builder.Environment.IsDevelopment())
    SelfLog.Enable(msg =>
    {
        Debug.Print(msg);
        Debugger.Break();
    });


#region Setup CORS

var corsBuilder = new CorsPolicyBuilder();

corsBuilder.SetIsOriginAllowed(_ => true);
corsBuilder.AllowAnyHeader();
corsBuilder.AllowCredentials();
corsBuilder.AllowAnyMethod();

builder.Services.AddCors(options => {
    options.AddPolicy("_CorsPolicy", corsBuilder.Build());
});

#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
    options.Filters.Add(new ValidateModelAttribute());
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = _ => new ValidationResult();
});

#region Metric

builder.Services.AddMetric();
builder.Host.UseMetricsWebTrack();

#endregion

builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddSignalR(options => {
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(20);
    options.HandshakeTimeout = TimeSpan.FromMinutes(20);
    options.KeepAliveInterval = TimeSpan.FromMinutes(20);
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = null;
    options.StreamBufferCapacity = null;
});

builder.Services.AddCacheService();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomResponseCompression();
builder.Services.AddJwtTokenAuthentication(builder.Configuration);
builder.Services.AddLogger();
builder.Services.AddLocalizationResource();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.RabbitMQPersistentConnection(builder.Configuration);
builder.Services.RegisterEventBus(builder.Configuration);
builder.Services.AddCustomHealthCheck(builder.Configuration);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(contianerBuilder=> {
                contianerBuilder.RegisterModule(new ApplicationModule());
               
            });

#endregion
#region [Configure]

//This method gets called by the runtime. Use this method to configure the HTTP request pipeline

var app = builder.Build();

app.UseRouting();
app.Services.Configure();
//app.UseCorsMiddleware();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();


#region [Resilience - Explore Metric]
app.UsePrometheusMetric();

#endregion

app.UseEndpoints(endpoints =>
{
  
    endpoints.MapHub<NotificationHub>("/eHub/notification", mapper =>
    {
        mapper.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
    });
});


var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<BalanceCheckEvent, BalanceCheckEventHandler>();
eventBus.Subscribe<DepositEvent, DepositEventHandler>();
eventBus.Subscribe<WithdrawEvent, WithdrawEventHandler>();

app.Run();

#endregion