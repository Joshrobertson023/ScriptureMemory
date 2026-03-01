using DataAccess.DataInterfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.IntegrationTests;

/// <summary>
/// Base class for all integration tests. Provides an <see cref="Api"/> HTTP client
/// and resets the database schema before each test so every test starts from a clean slate.
/// The Oracle container is shared across the entire test collection via
/// <see cref="IntegrationTestWebAppFactory"/>.
/// </summary>
public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebAppFactory _factory;

    /// <summary>HTTP client pointed at the test server. Use this for all interactions.</summary>
    protected HttpClient Api { get; private set; } = null!;

    protected readonly IVerseData verseContext;
    protected readonly INotificationData notificationContext;
    protected readonly ICollectionService collectionService;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        verseContext = _factory.Services.GetRequiredService<IVerseData>();
        notificationContext = _factory.Services.GetRequiredService<INotificationData>();
        collectionService = _factory.Services.GetRequiredService<ICollectionService>();
    }

    /// <summary>
    /// Runs before every test. Drops and recreates the schema so each test
    /// starts with empty tables, then creates a fresh HTTP client.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _factory.DropSchemaAsync();
        await _factory.CreateSchemaAsync();

        Api = _factory.CreateClient();
    }

    /// <summary>Runs after every test. Disposes the HTTP client.</summary>
    public Task DisposeAsync()
    {
        Api.Dispose();
        return Task.CompletedTask;
    }
}