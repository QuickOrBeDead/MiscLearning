using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;

using PdfWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IConnection>(_ => {
            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
            return factory.CreateConnection();
        });
        services.AddSingleton<IModel>(x => {
            return x.GetRequiredService<IConnection>().CreateModel();
        });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
