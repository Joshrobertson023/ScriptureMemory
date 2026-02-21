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
            .WithPassword("Oracle123!")
            .Build();
        
        await _dbContainer.StartAsync();

        await InitializeDatabaseTablesAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_dbContainer is null)
            return;

        try
        {
            await CleanDatabaseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cleaning test database: {ex}");
        }

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbConnection>();

            var host = _dbContainer.Hostname;
            var port = _dbContainer.GetMappedPublicPort(1521);
            var connString = $"User Id=system;Password=Oracle123!;Data Source={host}:{port}/XE";

            services.AddScoped<IDbConnection>(_ => new OracleConnection(connString));
        });
        builder.UseEnvironment("Development");
        builder.CaptureStartupErrors(true);
        builder.UseSetting("detailedErrors", "true");
    }

    public async Task CleanDatabaseAsync()
    {
        var host = _dbContainer!.Hostname;
        var port = _dbContainer.GetMappedPublicPort(1521);
        var connString = $"User Id=system;Password=Oracle123!;Data Source={host}:{port}/XE";

        using var connection = new OracleConnection(connString);
        await connection.OpenAsync();

        var commands = new[]
        {
            "DROP TABLE NOTIFICATIONS CASCADE CONSTRAINTS",
            "DROP TABLE ACTIVITY_LOGS CASCADE CONSTRAINTS",
            "DROP TABLE USER_PREFERENCES CASCADE CONSTRAINTS",
            "DROP TABLE USERS CASCADE CONSTRAINTS"
        };

        foreach (var cmdText in commands)
        {
            using var command = new OracleCommand(cmdText, connection);
            await command.ExecuteNonQueryAsync();
        }

        await connection.CloseAsync();
    }

    private async Task InitializeDatabaseTablesAsync()
    {

        var host = _dbContainer.Hostname;
        var port = _dbContainer.GetMappedPublicPort(1521);
        var connString = $"User Id=system;Password=Oracle123!;Data Source={host}:{port}/XE";

        using var connection = new OracleConnection(connString);
        await connection.OpenAsync();
        
        var commands = new[]
        {
            """
            CREATE TABLE "USERS" (
                "ID" NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                "USERNAME" VARCHAR2(100) UNIQUE NOT NULL,
                "FIRST_NAME" VARCHAR2(4000),
                "LAST_NAME" VARCHAR2(4000),
                "EMAIL" VARCHAR2(4000),
                "AUTH_TOKEN" VARCHAR2(4000),
                "STATUS" NUMBER,
                "HASHED_PASSWORD" VARCHAR2(4000),
                "DATE_REGISTERED" DATE,
                "LAST_SEEN" DATE,
                "PROFILE_DESCRIPTION" VARCHAR2(4000),
                "VERSES_MEMORIZED" NUMBER DEFAULT 0,
                "POINTS" NUMBER DEFAULT 0,
                "PROFILE_PICTURE_URL" VARCHAR2(500)
            )
            """,
            """
                        
              CREATE TABLE "USER_PREFERENCES" 
               (	
                "USER_ID" NUMBER PRIMARY KEY REFERENCES USERS(ID),
            	"THEME" NUMBER, 
            	"VERSION" NUMBER, 
            	"COLLECTIONS_SORT" NUMBER, 
            	"SUBSCRIBED_VOD" NUMBER, 
            	"PUSH_NOTIFICATIONS_ENABLED" NUMBER, 
            	"NOTIFY_MEMORIZED_VERSE" NUMBER, 
            	"NOTIFY_PUBLISHED_COLLECTION" NUMBER, 
            	"NOTIFY_COLLECTION_SAVED" NUMBER, 
            	"NOTIFY_NOTE_LIKED" NUMBER, 
            	"FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED" NUMBER, 
            	"STREAK_REMINDERS_ENABLED" NUMBER, 
            	"APP_BADGES_ENABLED" NUMBER, 
            	"PRACTICE_TAB_BADGES_ENABLED" NUMBER, 
            	"TYPE_OUT_REFERENCE" NUMBER
            )
            """,
            """
                        
              CREATE TABLE "ACTIVITY_LOGS" 
               (	
               "ID" NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, 
                "USER_ID" NUMBER REFERENCES USERS(ID),
            	"ACTION_TYPE" VARCHAR2(100), 
            	"ENTITY_TYPE" VARCHAR2(100), 
            	"ENTITY_ID" NUMBER, 
            	"CONTEXT_DESCRIPTION" VARCHAR2(1000), 
            	"METADATA_JSON" VARCHAR2(2000), 
            	"SEVERITY_LEVEL" NUMBER DEFAULT 0 NOT NULL, 
            	"IS_ADMIN_ACTION" NUMBER DEFAULT 0 NOT NULL, 
            	"CREATED_AT" DATE DEFAULT SYSDATE NOT NULL
            )
            """,
            """
                        
              CREATE TABLE "NOTIFICATIONS" 
               (	
               "ID" NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, 
            	"MESSAGE" VARCHAR2(1000) NOT NULL, 
            	"CREATEDDATE" DATE NOT NULL, 
            	"ISREAD" NUMBER(1,0) DEFAULT 0, 
            	"EXPIRATION_DATE" DATE, 
            	"NOTIFICATIONTYPE" NUMBER, 
            	"SENDER_ID" NUMBER REFERENCES USERS(ID), 
            	"RECEIVER_ID" NUMBER REFERENCES USERS(ID)
            )
            """
        };
        
        foreach (var cmdText in commands)
        {
            using var command = new OracleCommand(cmdText, connection);
            await command.ExecuteNonQueryAsync();
        }
        
        await connection.CloseAsync();
    }
}
