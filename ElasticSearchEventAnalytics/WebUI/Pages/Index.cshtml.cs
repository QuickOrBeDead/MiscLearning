using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using RabbitMQ.Client;

using System.Text.Json;
using System.Text;

using WebUI.Model;

namespace WebUI.Pages;

public class IndexModel : PageModel
{
    private readonly IConnection _rabbitMqConnection;
    public IndexModel(IConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    public void OnGet()
    {

    }

    public void OnPostAdd()
    {
        using var channel = _rabbitMqConnection.CreateModel();

        channel.QueueDeclare(queue: "ElasticSearchEventAnalytics.OrderCreated", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new OrderCreatedEvent()));

        channel.BasicPublish(exchange: string.Empty, routingKey: "ElasticSearchEventAnalytics.OrderCreated", basicProperties: null, body: body);
    }
}
