using J2N.Collections.Generic.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Testcontainers.Oracle;

namespace ScriptureMemory.IntegrationTests;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private OracleContainer? _dbContainer;
    private string? _connectionString;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        _dbContainer = new OracleBuilder()
            .WithImage("gvenzl/oracle-xe")
            .WithPassword("Oracle123!")
            .Build();

        _dbContainer.StartAsync().GetAwaiter().GetResult();

        var host = _dbContainer.Hostname;
        var port = _dbContainer.GetMappedPublicPort(1521);

        _connectionString =
            $"User Id=system;Password=Oracle123!;Data Source={host}:{port}/XE";

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString
            });
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        _ = CreateClient();

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
            var connString = _connectionString!;
        });
        builder.UseEnvironment("Development");
        builder.Configure(app =>
        {
            app.UseDeveloperExceptionPage();
        });
        builder.CaptureStartupErrors(true);
        builder.UseSetting("detailedErrors", "true");
    }

    public async Task CleanDatabaseAsync()
    {
        var connString = _connectionString!;

        using var connection = new OracleConnection(connString);
        await connection.OpenAsync();

        var commands = new[]
        {
            "DROP TABLE NOTIFICATIONS",
            "DROP TABLE ACTIVITY_LOGS",
            "DROP TABLE USER_PREFERENCES",
            "DROP TABLE USERS",
            "DROP TABLE VERSES"
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

        var connString = _connectionString!;

        using var connection = new OracleConnection(connString);
        await connection.OpenAsync();
        
        var commands = new[]
        {
            """
            CREATE TABLE USERS (
                ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                "USERNAME" VARCHAR2(4000),
                "FIRST_NAME" VARCHAR2(4000),
                "LAST_NAME" VARCHAR2(4000),
                "EMAIL" VARCHAR2(4000),
                "AUTH_TOKEN" VARCHAR2(4000),
                "STATUS" NUMBER,
                "HASHED_PASSWORD" VARCHAR2(4000),
                "DATERE_GISTERED" DATE,
                "LAST_SEEN" DATE,
                "profile_DESCRIPTION" VARCHAR2(4000),
                "VERSES_MEMORIZED" NUMBER DEFAULT 0,
                "POINTS" NUMBER DEFAULT 0,
                "PROFILE_PICTURE_URL" VARCHAR2(500)
            )
            """,
            """
                        
              CREATE TABLE "USER_PREFERENCES" 
                (
                "USER_ID" NUMBER PRIMARY KEY,
                "USERNAME" VARCHAR2(100),

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
                "TYPE_OUT_REFERENCE" NUMBER,

                CONSTRAINT "FK_USER_PREFERENCES_USERS"
                    FOREIGN KEY ("USER_ID")
                    REFERENCES "USERS"("ID")
                    ON DELETE CASCADE
                );
            """,
            """
                        
              CREATE TABLE "ACTIVITY_LOGS" 
               (	
               "ID" NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, 
                USER_ID NUMBER REFERENCES USERS(ID),
            	"ACTION_TYPE" NUMBER, 
            	"ENTITY_TYPE" NUMBER, 
            	"ENTITY_ID" NUMBER, 
            	"CONTEXT_DESCRIPTION" VARCHAR2(1000), 
            	"METADATA_JSON" VARCHAR2(2000), 
            	"SEVERITY_LEVEL" NUMBER DEFAULT 0 NOT NULL, 
            	"IS_ADMIN_ACTION" NUMBER DEFAULT 0 NOT NULL, 
            	"CREATED_AT" DATE DEFAULT SYSDATE NOT NULL,
            )
            """,
            """
                        
              CREATE TABLE "NOTIFICATIONS" 
               (	
               "ID" NUMBER GENERATED ALWAYS AS IDENTITY, 
            	"MESSAGE" VARCHAR2(1000) NOT NULL, 
            	"CREATEDDATE" DATE NOT NULL, 
            	"ISREAD" NUMBER(1,0) DEFAULT 0, 
            	"EXPIRATION_DATE" DATE, 
            	"NOTIFICATIONTYPE" NUMBER, 
            	"SENDER_ID" NUMBER REFERENCES USERS(ID),
            	"RECEIVER_ID" NUMBER REFERENCES USERS(ID), 
            	 PRIMARY KEY ("ID")
            )
            """,
            """
            CREATE TABLE VERSES
            (
            VERSE_ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            VERSE_REFERENCE VARCHAR2(100),
            USERS_SAVED_VERSE NUMBER DEFAULT 0,
            USERS_MEMORIZED_VERSE NUMBER DEFAULT 0,
            TEXT VARCHAR2(2000)
            )
            """
        };
        
        foreach (var cmdText in commands)
        {
            using var command = new OracleCommand(cmdText, connection);
            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (OracleException ex) when (ex.Number == 955)
            {

            }
        }
        
        await connection.CloseAsync();
    }
}
