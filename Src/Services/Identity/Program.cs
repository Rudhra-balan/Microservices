

using Common.Observability;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

#region [Configure Services]

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile(
        $"Identity.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.log.json",
        true, true);
    config.AddJsonFile("Identity.appsettings.json", true, true);
    config.AddJsonFile($"Identity.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
    config.AddEnvironmentVariables();
});
builder.WebHost.UseUrls("http://*:5001");

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

#region Metric

builder.Services.AddMetric();
builder.Host.UseMetricsWebTrack();

#endregion

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
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromMilliseconds(60);
});
builder.Services.AddCacheService();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomResponseCompression();
builder.Services.AddJwtTokenAuthentication(builder.Configuration);
builder.Services.AddLogger();
builder.Services.AddBusinessManager();
builder.Services.AddRepository();
builder.Services.AddLocalizationResource();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#endregion

#region [Configure]

//This method gets called by the runtime. Use this method to configure the HTTP request pipeline

var app = builder.Build();

app.UseRouting();
app.Services.Configure();
app.UseSwaggerSetup(builder.Configuration);

app.UseResponseAndExceptionWrapper();

app.UseCustomResponseCompression();

app.UseAntiXssMiddleware();
app.UseSecurityHeadersMiddleware(
    new SecurityHeadersBuilder()
        .AddDefaultSecurePolicy());
app.UseDosAttackMiddleware();
app.UseCors("_CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

#region [Resilience - Explore Metric]

app.UsePrometheusMetric();

#endregion

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseLocalizationResource();

app.Run();

#endregion