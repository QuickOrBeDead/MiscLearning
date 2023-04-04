using Microsoft.IdentityModel.Tokens;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddAuthentication()
    .AddJwtBearer(x =>
        {
            x.Authority = Constants.Constants.Endpoints.IdentityServer;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = new TokenValidationParameters
                                              {
                                                  ValidateAudience = false
                                              };
        });

builder.Services.AddOcelot();
var app = builder.Build();

// Configure the HTTP request pipeline.
await app.UseOcelot();

app.Run();
