var builder = WebApplication.CreateBuilder(args);

var proxyBuilder = builder.Services.AddReverseProxy();
proxyBuilder.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.MapReverseProxy();
app.Run();