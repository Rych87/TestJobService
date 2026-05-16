using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace TestJobService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<Settings> _settings;
        private readonly HttpClient _httpClient;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<Settings> settings, HttpClient httpClient)
        {
            _logger = logger;
            _settings = settings;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Запуск сервиса " + _settings.CurrentValue.ApiUri);
            var channel = Channel.CreateBounded<string>(500);

            // Таск очереди отправки
            var sendTask = Task.Run(async () =>
            {
                await foreach (var json in channel.Reader.ReadAllAsync(stoppingToken))
                {
                    bool sent = false;

                    while (!sent && !stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var settings = _settings.CurrentValue;
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response =
                                await _httpClient.PostAsync(
                                settings.ApiUri,
                                content,
                                stoppingToken);

                            if (response.IsSuccessStatusCode)
                            {
                                _logger.LogInformation("Успешно отправлено");
                                sent = true;
                            }
                            else
                            {
                                _logger.LogError($"HTTP {(int)response.StatusCode}");
                                await Task.Delay(5000, stoppingToken);
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            await Task.Delay(5000, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                            throw;
                        }
                    }
                }
            });

            try
            {
                while (!stoppingToken.IsCancellationRequested && !sendTask.IsCompleted)
                {
                    var settings = _settings.CurrentValue;
                    var info = SystemInfo.GetInfo();
                    var json = JsonSerializer.Serialize(info);
                    await channel.Writer.WriteAsync(json);
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException ex) { }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                channel.Writer.Complete();
                _logger.LogInformation("Завершение работы сервиса");
            }
        }
    }
}
