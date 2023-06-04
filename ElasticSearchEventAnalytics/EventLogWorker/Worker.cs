namespace EventLogWorker;

using EventLogWorker.Model;

using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Nest;

public class Worker : BackgroundService
{
    private readonly IModel _consumerChannel;
    private readonly IElasticClient _elasticClient;
    private IList<string> _consumerTags;

    public Worker(IModel consumerChannel, IElasticClient elasticClient)
    {
        _consumerChannel = consumerChannel;
        _elasticClient = elasticClient;
        _consumerTags = new List<string>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            ConsumeEventLog();
            ConsumeDocumentEventLog();
        }

        return Task.CompletedTask;
    }

    private void ConsumeEventLog()
    {
        _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.EventLog", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_consumerChannel);
        consumer.Received += (_, e) =>
        {
            try
            {
                var eventLog = JsonSerializer.Deserialize<EventLog>(Encoding.UTF8.GetString(e.Body.Span));

                if (eventLog != null)
                {
                    _elasticClient.Index(eventLog, x => x.Index($"eventlog-{eventLog.CreateDate:yyyy-MM-dd}"));
                }

                _consumerChannel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception)
            {
                _consumerChannel.BasicNack(e.DeliveryTag, false, true);
            }
        };
        _consumerChannel.BasicQos(0, 1, false);
        _consumerTags.Add(_consumerChannel.BasicConsume("ElasticSearchEventAnalytics.EventLog", false, consumer));
    }

    private void ConsumeDocumentEventLog()
    {
        _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.DocumentEventLog", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_consumerChannel);
        consumer.Received += (_, e) =>
        {
            try
            {
                var eventLog = JsonSerializer.Deserialize<DocumentEventLog>(Encoding.UTF8.GetString(e.Body.Span));

                if (eventLog != null)
                {
                    var indexName = $"documenteventlog-{eventLog.CreateDate:yyyy-MM-dd}";

                    var getResponse = _elasticClient.Get<DocumentEventLog>(eventLog.EventId, x => x.Index(indexName));
                    if (getResponse.IsValid && getResponse.Found)
                    {
                        var document = getResponse.Source;
                        if (eventLog.IsPdfCreated.HasValue)
                        {
                            document.IsPdfCreated = eventLog.IsPdfCreated;
                        }

                        if (eventLog.IsEmailSent.HasValue)
                        {
                            document.IsEmailSent = eventLog.IsEmailSent;
                        }

                        if (eventLog.IsSmsSent.HasValue)
                        {
                            document.IsSmsSent = eventLog.IsSmsSent;
                        }

                        if (eventLog.IsDocumentViewed.HasValue)
                        {
                            document.IsDocumentViewed = eventLog.IsDocumentViewed;
                        }

                        _elasticClient.Update<DocumentEventLog>(eventLog.EventId, x => x.Doc(document));
                    }
                    else
                    {
                        _elasticClient.Index(eventLog, x => x.Index(indexName));
                    }
                }

                _consumerChannel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception)
            {
                _consumerChannel.BasicNack(e.DeliveryTag, false, true);
            }
        };
        _consumerChannel.BasicQos(0, 1, false);
        _consumerTags.Add(_consumerChannel.BasicConsume("ElasticSearchEventAnalytics.DocumentEventLog", false, consumer));
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var consumerTag in _consumerTags)
        {
            _consumerChannel?.BasicCancel(consumerTag);
        }

        _consumerTags.Clear();

        _consumerChannel?.Close(200, "Goodbye");
        _consumerChannel?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}
