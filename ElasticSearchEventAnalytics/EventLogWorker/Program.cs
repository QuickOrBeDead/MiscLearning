using EventLogWorker;
//using EventLogWorker.Model;

using Nest;

using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IConnection>(_ => {
            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
            return factory.CreateConnection();
        });
        services.AddSingleton<IModel>(x => {
            return x.GetRequiredService<IConnection>().CreateModel();
        });
        services.AddSingleton<IElasticClient>(_ => new ElasticClient(new ConnectionSettings(new Uri("http://elasticsearch:9200"))/*.DefaultMappingFor<DocumentEventLog>(x => x.IdProperty(y => y.EventId).IndexName("documenteventlog"))*/));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
