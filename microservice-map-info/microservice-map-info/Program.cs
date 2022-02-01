using GoogleMapInfo;

using microservice_map_info.Services;

using Prometheus;

using Serilog;

//IConfiguration configuration = new ConfigurationBuilder()
//            .SetBasePath(Directory.GetCurrentDirectory())
//            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
//            .AddEnvironmentVariables()
//            .Build();

// Mimic the configuration options in Host.CreateDefaultBuilder()
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", true)
    .AddEnvironmentVariables()
    .AddUserSecrets(typeof(Program).Assembly)
    .AddCommandLine(args)
    .Build();

// Initialize Serilog's logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var jeagerHost = configuration.GetValue<string>("openTelemetry:jeagerHost");
var distanceMicroserviceUrl = configuration.GetSection("DistanceMicroservice:Location").Value;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddTransient<GoogleDistanceApi>();
builder.Services.AddScoped(typeof(IDistanceInfoSvc), typeof(DistanceInfoSvc));
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddHttpClient("DistanceMicroservice", client =>
{
    client.BaseAddress = new Uri(distanceMicroserviceUrl);
}).UseHttpClientMetrics();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseHttpMetrics();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DistanceInfoService>();
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

try
{
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}