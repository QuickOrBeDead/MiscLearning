using IdentityServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityServerConfig.Clients)
    .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
    .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
    .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
    .AddTestUsers(IdentityServerConfig.TestUsers)
    .AddDeveloperSigningCredential();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseIdentityServer();

app.Run();
