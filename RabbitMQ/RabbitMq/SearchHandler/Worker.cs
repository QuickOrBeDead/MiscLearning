namespace SearchHandler;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConnection _connection;

    public Worker(ILogger<Worker> logger, IConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumeTask = Task.Factory.StartNew(() =>
            {
                var model = _connection.CreateModel();

                model.QueueDeclare("Search", true, false, false);

                var consumer = new EventingBasicConsumer(model);
                consumer.Received += (_, e) =>
                    {
                        try
                        {
                            var q = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetString(e.Body.Span));

                            _logger.LogInformation($"q: {q}");

                            model.BasicAck(e.DeliveryTag, false);
                        }
                        catch (Exception)
                        {
                            model.BasicNack(e.DeliveryTag, false, true);

                            throw;
                        }
                    };
                return (model ,model.BasicConsume("Search", false, consumer));
            }, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);

        var (channel, consumeTag) = await consumeTask;
        if (channel != null)
        {
            if (consumeTag != null)
            {
                channel.BasicCancel(consumeTag);
            }

            channel.Close(200, "Goodbye");
            channel.Dispose();
        }
    }
}