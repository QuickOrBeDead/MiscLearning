using MessageQueue;

using WebApp.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IRabbitMqConnection>(_ => new DefaultRabbitMqConnection(builder.Configuration.GetSection("RabbitMqConnection").Get<RabbitMqConnectionSettings>()!));
builder.Services.AddSingleton<IMessageQueuePublisherService, RabbitMqMessageQueuePublisherService>();

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
