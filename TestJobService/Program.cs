using Serilog;
using TestJobService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;

var builder = Host.CreateApplicationBuilder(args);

var logPath = builder.Configuration.GetSection("Settings").GetValue<string>("logPath") ?? "logs/";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: logPath + "log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    .WriteTo.EventLog(source: "System Monitor Agent", logName: "Application", manageEventSource: true)
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.TryGetValue("SourceContext", out var value) &&
        value.ToString().Contains("LogicalHandler"))    //Иначе http запрос дублируется в логах
    .CreateLogger();



builder.Services.AddWindowsService((options) => options.ServiceName = "System Monitor Agent");

builder.Logging.ClearProviders();
builder.Services.AddSerilog();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

var host = builder.Build();
host.Run();
