using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Testcontainers.Oracle;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using J2N.Collections.Generic.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ScriptureMemory.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private OracleContainer? _dbContainer;

    public async Task InitializeAsync()
    {
        _dbContainer = new OracleBuilder()
            .WithImage("gvenzl/oracle-xe")
            .WithPassword("oracle")
            .Build();
        
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_dbContainer is not null)
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbConnection>();

            services.AddScoped<IDbConnection>(_ =>
                new OracleConnection(_dbContainer!.GetConnectionString()));
        });
    }
}
