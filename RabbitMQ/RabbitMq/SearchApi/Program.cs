using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(
    _ =>
        {
            var factory = new ConnectionFactory {HostName = "rabbitmq", UserName = "guest", Password = "guest"};
            return factory.CreateConnection();
        });

var app = builder.Build();

app.MapGet("/search", (string q, IConnection connection) =>
    {
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("Search", "fanout", true, false, null);
            channel.QueueDeclare("Search", false, false, false, null);
            channel.BasicPublish(
                "Search",
                string.Empty,
            null,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(q)));
        }

        return Results.Ok();
    });

app.Run();