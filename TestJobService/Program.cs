using TestJobService;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
