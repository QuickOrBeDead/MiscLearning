using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;

using PdfWorker;
using Nest;

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
        services.AddSingleton<IElasticClient>(_ => new ElasticClient(new ConnectionSettings(new Uri("http://elasticsearch:9200"))));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
