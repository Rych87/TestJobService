using TestJobService;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true);

//builder.Logging.ClearProviders();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

var host = builder.Build();
host.Run();
