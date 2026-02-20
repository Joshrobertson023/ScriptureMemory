using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace VerseApp.IntegrationTests;

public class WebAppFactory : WebApplicationFactory<Program>
{
    private const string testConnectionString = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbConnection>();

            services.AddScoped<IDbConnection>(_ =>
                new OracleConnection(testConnectionString));
        });
    }
}
