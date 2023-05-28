using EventLogWorker;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
         services.AddSingleton<IConnection>(_ => {
            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
            return factory.CreateConnection();
        });
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
