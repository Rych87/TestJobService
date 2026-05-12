namespace TestJobService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var info = SystemInfo.GetInfo(stoppingToken);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time} {hostname}", DateTimeOffset.Now, info.HostName);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
