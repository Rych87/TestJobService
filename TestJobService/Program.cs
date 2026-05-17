using Serilog;
using TestJobService;

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
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.TryGetValue("SourceContext", out var value) &&
        value.ToString().Contains("LogicalHandler"))    //Иначе http запрос дублируется в логах
    .CreateLogger();



//builder.Configuration.AddJsonFile(
//    "appsettings.json",
//    optional: false,
//    reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Services.AddSerilog();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

var host = builder.Build();
host.Run();
