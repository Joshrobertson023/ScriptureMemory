using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Npgsql;
using ScriptureMemory.Server.Startup;
using ScriptureMemory.Server.Tools;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestProperties;
});

builder.Services
    .AddServices(builder.Configuration) 
    .AddSecurity(builder.Configuration) // Add authentication & authorization
    .AddDataAccess();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseMiddleware()
    .UseEndpoints();

// Convert all errors into Problem Details responses
app.UseStatusCodePages();

using var scope = app.Services.CreateScope();
{
    var service = scope.ServiceProvider.GetRequiredService<BibleApi>();
    await service.SyncDatabaseWithApiBible(app.Logger, app.Configuration);
}

app.Run();

public partial class Program() { }
