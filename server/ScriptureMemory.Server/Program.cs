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

builder.Logging.ConfigureOpenTelemetrySignalRLogging(builder.Configuration);

builder.Services
    .AddServices(builder.Configuration) 
    .AddAuthenticationAndAuthorization(builder.Configuration)
    .ConfigureTracingAndMetricsExporting(builder.Configuration)
    .AddDataAccess();

var app = builder.Build();

app.UseMiddleware()
    .UseEndpoints();

await app.AskToRunOptionalStartupTasks();

app.Run();

public partial class Program() { }
