using Serilog;
using TestJobService;
using Microsoft.Extensions.Hosting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true);

//builder.Logging.ClearProviders();\
builder.Services.AddSerilog();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

var host = builder.Build();
host.Run();
