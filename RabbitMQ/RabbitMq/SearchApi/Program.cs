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
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(
    b =>
        {
            b.AllowAnyOrigin();
        });

app.MapGet("/search-product/{id}", (int id, IConnection connection) =>
    {
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare("Search", true, false, false);
            channel.BasicPublish(
                "",
                "Search",
            null,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(id)));
        }

        return Results.Ok();
    });

app.Run();