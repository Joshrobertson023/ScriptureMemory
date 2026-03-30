using ScriptureMemory.Server.Startup;
using ScriptureMemory.Server.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddDatabaseConnections()
    .AddServices()
    .AddDataAccess();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseMiddleware()
   .UseEndpoints();

using var scope = app.Services.CreateScope();
{
    var service = scope.ServiceProvider.GetRequiredService<VerseManagement>();
    await service.UploadVersesToPostgres();
}

app.Run();

public partial class Program() { }
