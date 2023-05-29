using Microsoft.AspNetCore.Mvc.RazorPages;

using RabbitMQ.Client;

using System.Text.Json;
using System.Text;

using WebUI.Model;

namespace WebUI.Pages;

public class IndexModel : PageModel
{
    public string Message { get; set; }
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

        channel.ExchangeDeclare("ElasticSearchEventAnalytics.OrderCreated", "fanout", false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new OrderCreatedEvent()));

        channel.BasicPublish(exchange: "ElasticSearchEventAnalytics.OrderCreated", routingKey: string.Empty, basicProperties: null, body: body);

        Message = $"Order is create at {DateTime.Now}";
    }
}
