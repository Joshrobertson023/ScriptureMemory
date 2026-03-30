using DataAccess.Data;
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
    private IServiceScope _scope = null!;

    protected HttpClient Api { get; private set; } = null!;
    protected VerseData verseContext { get; private set; } = null!;
    protected NotificationData notificationContext { get; private set; } = null!;
    protected CollectionService collectionService { get; private set; } = null!;
    protected PublishedCollectionData publishedContext { get; private set; } = null!;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.DropSchemaAsync();
        await _factory.CreateSchemaAsync();

        Api = _factory.CreateClient();

        // Create a fresh scope per test so scoped services resolve correctly
        _scope = _factory.Services.CreateScope();
        verseContext = _scope.ServiceProvider.GetRequiredService<VerseData>();
        notificationContext = _scope.ServiceProvider.GetRequiredService<NotificationData>();
        collectionService = _scope.ServiceProvider.GetRequiredService<CollectionService>();
        publishedContext = _scope.ServiceProvider.GetRequiredService<PublishedCollectionData>();
    }

    public Task DisposeAsync()
    {
        _scope?.Dispose();
        Api.Dispose();
        return Task.CompletedTask;
    }
}