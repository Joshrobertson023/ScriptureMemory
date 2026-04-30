using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql;
using ScriptureMemory.Server.Startup;
using ScriptureMemory.Server.Tools;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services
    .AddServices()
    .AddDataAccess();

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Admin", policy => policy.RequireClaim(
        "role", Enums.UserRole.Admin.ToString(), Enums.UserRole.SuperAdmin.ToString()));
    o.AddPolicy("SuperAdmin", policy => policy.RequireClaim(
        "role", Enums.UserRole.SuperAdmin.ToString()));
    o.AddPolicy("UserOnly", policy => policy.RequireClaim(
        "role", Enums.UserRole.User.ToString()));
    o.AddPolicy("UserOrAdmin", policy => policy.RequireClaim(
        "role", 
        Enums.UserRole.User.ToString(), 
        Enums.UserRole.Admin.ToString(), 
        Enums.UserRole.SuperAdmin.ToString()));
}); 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");

NpgsqlDataSourceBuilder dataSourceBuilder = new(connectionString);
dataSourceBuilder.UseVector();
NpgsqlDataSource dataSource = dataSourceBuilder.Build();

builder.Services.AddSingleton(dataSource);

var app = builder.Build();

app.UseMiddleware()
    .UseEndpoints();

using var scope = app.Services.CreateScope();
{
    var service = scope.ServiceProvider.GetRequiredService<BibleApi>();
    await service.SyncDatabaseWithApiBible(app.Logger, app.Configuration);
}

app.Run();

public partial class Program() { }
