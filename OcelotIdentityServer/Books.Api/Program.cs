using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        {
            options.Authority = Constants.Constants.Endpoints.IdentityServer;
            options.RequireHttpsMetadata = false;
            options.Audience = "bookAPI";
        });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "bookClient"));
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
