using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmsWorker.Model;

namespace SmsWorker;

public sealed class Worker : BackgroundService
{
    private readonly IModel _consumerChannel;
    private readonly IConnection _rabbitMqConnection;
    private string? _consumerTag;

    public Worker(IModel consumerChannel, IConnection rabbitMqConnection)
    {
        _consumerChannel = consumerChannel;  
        _rabbitMqConnection = rabbitMqConnection;  
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            _consumerChannel.ExchangeDeclare("ElasticSearchEventAnalytics.PdfCreated", "fanout", false, false, null);
            _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.Sms", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _consumerChannel.QueueBind("ElasticSearchEventAnalytics.Sms", "ElasticSearchEventAnalytics.PdfCreated", string.Empty);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (_, e) =>
                {
                    try
                    {
                        Thread.Sleep(3_000);
                        
                        var pdfCreatedEvent = JsonSerializer.Deserialize<PdfCreatedEvent>(Encoding.UTF8.GetString(e.Body.Span));
                        if (pdfCreatedEvent != null)
                        {
                            PublishSmsSentEventLog(pdfCreatedEvent);
                        }

                        _consumerChannel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        _consumerChannel.BasicNack(e.DeliveryTag, false, true);
                    }
                };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerTag = _consumerChannel.BasicConsume("ElasticSearchEventAnalytics.Sms", false, consumer);
        }

        return Task.CompletedTask;
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

    private void PublishSmsSentEventLog(PdfCreatedEvent pdfCreatedEvent)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.QueueDeclare("ElasticSearchEventAnalytics.EventLog", false, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new EventLog
                                                                    {
                                                                        DocumentId = pdfCreatedEvent.DocumentId,
                                                                        EventId = pdfCreatedEvent.Id,
                                                                        EventType = "SmsIsSent"
                                                                    }));

        channel.BasicPublish(exchange: string.Empty, routingKey: "ElasticSearchEventAnalytics.EventLog", basicProperties: null, body: body);
    }
}