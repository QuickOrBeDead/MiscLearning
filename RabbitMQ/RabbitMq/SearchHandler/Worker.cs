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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var channel = _connection.CreateModel();

            channel.ExchangeDeclare("Search", "fanout", true, false, null);
            channel.QueueDeclare("Search", true, false, false);
            channel.QueueBind("Search", "Search", string.Empty);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, e) =>
            {
                try
                {
                    var q = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetString(e.Body.Span));

                    _logger.LogInformation($"q: {q}");

                    channel.BasicAck(e.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicNack(e.DeliveryTag, false, true);

                    throw;
                }
            };
        }

        return Task.CompletedTask;
    }
}