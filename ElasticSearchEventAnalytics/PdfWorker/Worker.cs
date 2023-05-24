using Microsoft.Extensions.Hosting;

using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using PdfWorker.Model;
using Nest;

namespace PdfWorker;

public sealed class Worker : BackgroundService
{
    private readonly IModel _consumerChannel;
    private readonly IConnection _rabbitMqConnection;
    private readonly IElasticClient _elasticClient;
    private string? _consumerTag;

    public Worker(IModel consumerChannel, IConnection rabbitMqConnection,  IElasticClient elasticClient)
    {
        _consumerChannel = consumerChannel;
        _rabbitMqConnection = rabbitMqConnection;
        _elasticClient = elasticClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            _consumerChannel.ExchangeDeclare("ElasticSearchEventAnalytics.OrderCreated", "fanout", false, false, null);
            _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.Pdf", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _consumerChannel.QueueBind("ElasticSearchEventAnalytics.Pdf", "ElasticSearchEventAnalytics.OrderCreated", string.Empty);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (_, e) =>
                {
                    try
                    {
                        var orderCreateEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(Encoding.UTF8.GetString(e.Body.Span));
                        
                        PublishPdfCreatedEvent(orderCreateEvent.Id);

                        _consumerChannel.BasicAck(e.DeliveryTag, false);

                        var eventLog = new PdfCreatedEventLog
                                        {
                                            DocumentId = $"2023-05-24_{Guid.NewGuid()}",
                                            EventId = Guid.NewGuid()
                                        };
                        _elasticClient.Index(eventLog, x => x.Index($"eventlog-{eventLog.CreateDate:yyyy-MM-dd}"));
                    }
                    catch (Exception)
                    {
                        _consumerChannel.BasicNack(e.DeliveryTag, false, true);
                    }
                };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerTag = _consumerChannel.BasicConsume("ElasticSearchEventAnalytics.Pdf", false, consumer);
        }

        return Task.CompletedTask;
    }

    private void PublishPdfCreatedEvent(Guid id)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.ExchangeDeclare("ElasticSearchEventAnalytics.PdfCreated", "fanout", false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new PdfCreatedEvent { Id = id }));

        channel.BasicPublish(exchange: "ElasticSearchEventAnalytics.OrderCreated", routingKey: string.Empty, basicProperties: null, body: body);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_consumerTag != null)
        {
            _consumerChannel?.BasicCancel(_consumerTag);
        }

        _consumerChannel?.Close(200, "Goodbye");
        _consumerChannel?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}