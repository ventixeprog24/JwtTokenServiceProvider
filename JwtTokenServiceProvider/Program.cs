using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JwtTokenServiceProvider.Services;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddSingleton<JwtSecurityTokenHandler>();
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    var key = Encoding.UTF8.GetBytes(configuration!["Jwt:Key"] ?? "failed-key");
    var issuer = configuration["Jwt:Issuer"];
    var audience = configuration["Jwt:Audience"];
    return new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.FromMinutes(3),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience
    };
});

var app = builder.Build();

app.MapGrpcService<JwtTokenService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();