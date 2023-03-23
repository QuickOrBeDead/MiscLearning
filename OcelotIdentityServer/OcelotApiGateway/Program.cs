using Microsoft.IdentityModel.Tokens;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication()
    .AddJwtBearer(x =>
        {
            x.Authority = "http://localhost:5100";
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
