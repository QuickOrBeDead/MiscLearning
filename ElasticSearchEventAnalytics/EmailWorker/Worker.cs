using System.Net.Mail;
using System.Text;
using System.Text.Json;
using EmailWorker.Model;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailWorker;

public sealed class Worker : BackgroundService
{
    private readonly IModel _consumerChannel;
    private readonly IConnection _rabbitMqConnection;
    private string _consumerTag;

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
            _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.EMail", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _consumerChannel.QueueBind("ElasticSearchEventAnalytics.EMail", "ElasticSearchEventAnalytics.PdfCreated", string.Empty);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (_, e) =>
                {
                    try
                    {
                        Thread.Sleep(3_000);
                        
                        var pdfCreatedEvent = JsonSerializer.Deserialize<PdfCreatedEvent>(Encoding.UTF8.GetString(e.Body.Span));
                        if (pdfCreatedEvent != null)
                        {
                            SendEmail(pdfCreatedEvent);
                            PublishEmailSentEventLog(pdfCreatedEvent);
                            PublishEmailSentDocumentEventLog(pdfCreatedEvent);
                        }

                        _consumerChannel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        _consumerChannel.BasicNack(e.DeliveryTag, false, true);
                    }
                };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerTag = _consumerChannel.BasicConsume("ElasticSearchEventAnalytics.EMail", false, consumer);
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

    private void SendEmail(PdfCreatedEvent pdfCreatedEvent)
    {
        using var message = new MailMessage();
        using var smtp = new SmtpClient();
        message.From = new MailAddress("from@test.com");
        message.To.Add(new MailAddress("to@test.com"));
        message.Subject = $"Document {pdfCreatedEvent.DocumentId} Email";
        message.IsBodyHtml = true;
        message.Body = $"Hi,<br>You can view document at <a href=\"http://localhost:8090/view/{pdfCreatedEvent.DocumentId}\">{pdfCreatedEvent.DocumentId}</a>";
        smtp.Port = 25;
        smtp.Host = "host.docker.internal";
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.Send(message);
    }

    private void PublishEmailSentEventLog(PdfCreatedEvent pdfCreatedEvent)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.QueueDeclare("ElasticSearchEventAnalytics.EventLog", false, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new EventLog
                                                                    {
                                                                        DocumentId = pdfCreatedEvent.DocumentId,
                                                                        EventId = pdfCreatedEvent.Id,
                                                                        EventType = "EmailIsSent"
                                                                    }));

        channel.BasicPublish(exchange: string.Empty, routingKey: "ElasticSearchEventAnalytics.EventLog", basicProperties: null, body: body);
    }

    private void PublishEmailSentDocumentEventLog(PdfCreatedEvent pdfCreatedEvent)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.QueueDeclare("ElasticSearchEventAnalytics.DocumentEventLog", false, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new DocumentEventLog
                                                                    {
                                                                        DocumentId = pdfCreatedEvent.DocumentId,
                                                                        EventId = pdfCreatedEvent.Id,
                                                                        CreateDate = pdfCreatedEvent.CreateDate,
                                                                        IsEmailSent = true
                                                                    }));

        channel.BasicPublish(exchange: string.Empty, routingKey: "ElasticSearchEventAnalytics.DocumentEventLog", basicProperties: null, body: body);
    }
}