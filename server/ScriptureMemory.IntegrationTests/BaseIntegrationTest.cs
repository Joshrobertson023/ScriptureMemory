using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Testcontainers.Oracle;
using DataAccess.DataInterfaces;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope scope;
    private readonly IntegrationTestWebAppFactory factory;

    protected readonly HttpClient api;

    protected readonly IUserData userContext;
    protected readonly IUserService userService;
    protected readonly IActivityLogger activityLogger;
    protected readonly IActivityLoggingData activityLogContext;
    protected readonly INotificationData notificationContext;
    protected readonly IVerseData verseContext;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        this.factory = factory;
        scope = factory.Services.CreateScope();
        api = factory.CreateClient();

        userContext = scope.ServiceProvider.GetRequiredService<IUserData>();
        userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        activityLogger = scope.ServiceProvider.GetRequiredService<IActivityLogger>();
        activityLogContext = scope.ServiceProvider.GetRequiredService<IActivityLoggingData>();
        notificationContext = scope.ServiceProvider.GetRequiredService<INotificationData>();
        verseContext = scope.ServiceProvider.GetRequiredService<IVerseData>();
    }

    public async Task InitializeAsync()
    {
        await factory.CleanDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}