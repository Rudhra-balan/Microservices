
using Common.Observability;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

#region [Configure Services]

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile(
        $"Transaction.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.log.json",
        true, true);
    config.AddJsonFile("Transaction.appsettings.json", true, true);
    config.AddJsonFile($"Transaction.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
    config.AddEnvironmentVariables();
});
builder.WebHost.UseUrls("http://*:5002");
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
});
#region Metric

builder.Services.AddMetric();
builder.Host.UseMetricsWebTrack();

#endregion

builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddTransactionFramework(builder.Configuration);
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddCacheService();
builder.Services.AddHttpContextAccessor();

builder.Services.AddJwtTokenAuthentication(builder.Configuration);
builder.Services.AddLogger();
builder.Services.AddScoped<ExceptionHandlerMiddleware>();

builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();
builder.Services.AddLocalizationResource();
builder.Services.RabbitMQPersistentConnection(builder.Configuration);
builder.Services.RegisterEventBus(builder.Configuration);
builder.Services.AddCustomHealthCheck(builder.Configuration);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());



#endregion
#region [Configure]

//This method gets called by the runtime. Use this method to configure the HTTP request pipeline

var app = builder.Build();
app.Services.Configure();
app.UseResponseAndExceptionWrapper();
app.UseSwaggerSetup(builder.Configuration);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandlerMiddleware();
app.UseCors("CorsPolicy");

#region [Resilience - Explore Metric]
app.UsePrometheusMetric();

#endregion
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});



app.Run();

#endregion


