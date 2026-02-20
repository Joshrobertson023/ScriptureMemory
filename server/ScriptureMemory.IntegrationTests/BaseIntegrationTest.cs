using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Testcontainers.Oracle;
using DataAccess.DataInterfaces;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope scope;

    protected readonly HttpClient client;

    protected readonly IUserData userContext;
    protected readonly IUserService userService;
    protected readonly IActivityLogger activityLogger;
    protected readonly IActivityLoggingData activityLogContext;
    protected readonly INotificationData notificationContext;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        scope = factory.Services.CreateScope();
        client = factory.CreateClient();

        userContext = scope.ServiceProvider.GetRequiredService<IUserData>();
        userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        activityLogger = scope.ServiceProvider.GetRequiredService<IActivityLogger>();
        activityLogContext = scope.ServiceProvider.GetRequiredService<IActivityLoggingData>();
        notificationContext = scope.ServiceProvider.GetRequiredService<INotificationData>();
    }
}