using DataAccess.Data;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IServiceScope _scope;
    protected readonly ApplicationDbContext _dbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}