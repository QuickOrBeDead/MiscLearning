using Microsoft.AspNetCore.Mvc.RazorPages;

using RabbitMQ.Client;

using System.Text.Json;
using System.Text;

using WebUI.Model;
using System.Globalization;

namespace WebUI.Pages;

public class ViewModel : PageModel
{
    public string Message { get; set; }
    private readonly IConnection _rabbitMqConnection;
    public ViewModel(IConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    public void OnGet(string documentId)
    {
        var eventId = ParseEventId(documentId);

        PublishDocumentViewedEventLog(documentId, eventId);
        PublishDocumentViewedDocumentEventLog(documentId, eventId);

        Message = $"Document {documentId} is viewed at {DateTime.Now}";
    }

    private void PublishDocumentViewedEventLog(string documentId, Guid eventId)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.QueueDeclare("ElasticSearchEventAnalytics.EventLog", false, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new EventLog
        {
            DocumentId = documentId,
            EventId = eventId,
            EventType = "DocumentIsViewed"
        }));

        channel.BasicPublish(exchange: string.Empty, routingKey: "ElasticSearchEventAnalytics.EventLog", basicProperties: null, body: body);
    }

    private void PublishDocumentViewedDocumentEventLog(string documentId, Guid eventId)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.QueueDeclare("ElasticSearchEventAnalytics.DocumentEventLog", false, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new DocumentEventLog
                                                                    {
                                                                        DocumentId = documentId,
                                                                        EventId = eventId,
                                                                        IsDocumentViewed = true
                                                                    }));

        channel.BasicPublish(exchange: string.Empty, routingKey: "ElasticSearchEventAnalytics.DocumentEventLog", basicProperties: null, body: body);
    }

    private static Guid ParseEventId(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Guid.Empty;
        }

        var keyParts = key.Split('_', StringSplitOptions.RemoveEmptyEntries);
        switch (keyParts.Length)
        {
            case 0 or > 2:
                return Guid.Empty;
            case 1:
                {
                    if (!Guid.TryParse(keyParts[0], out var id))
                    {
                        return Guid.Empty;
                    }

                    return id;
                }
            case 2:
                {
                    if (!DateTime.TryParseExact(keyParts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        return Guid.Empty;
                    }

                    if (!Guid.TryParse(keyParts[1], out var id))
                    {
                        return Guid.Empty;
                    }

                    return id;
                }
            default:
                return Guid.Empty;
        }
    }
}