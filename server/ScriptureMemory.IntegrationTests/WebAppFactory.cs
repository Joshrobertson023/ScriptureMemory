using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Testcontainers.Oracle;

namespace ScriptureMemory.IntegrationTests;

/// <summary>
/// Shared test fixture that spins up an Oracle XE container, creates the schema,
/// and tears everything down after all tests in the collection have run.
/// </summary>
public sealed class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    // -------------------------------------------------------------------------
    // Constants
    // -------------------------------------------------------------------------

    private const string OracleImage = "gvenzl/oracle-xe";
    private const string AdminPassword = "Oracle123!";
    private const string AppUser = "APPUSER";
    private const string AppPassword = "App123!";
    private const string ServiceName = "XEPDB1";
    private const int OraclePort = 1521;

    // -------------------------------------------------------------------------
    // State
    // -------------------------------------------------------------------------

    private OracleContainer? _container;

    /// <summary>Connection string for <see cref="AppUser"/>. Available after <see cref="InitializeAsync"/>.</summary>
    public string ConnectionString { get; private set; } = string.Empty;

    // -------------------------------------------------------------------------
    // IAsyncLifetime – start / stop container
    // -------------------------------------------------------------------------

    /// <summary>Starts the Oracle container, provisions the schema, then wires up the HTTP client.</summary>
    public async Task InitializeAsync()
    {
        _container = new OracleBuilder()
            .WithImage(OracleImage)
            .WithPassword(AdminPassword)
            .Build();

        await _container.StartAsync();

        ConnectionString = BuildConnectionString(AppUser, AppPassword);

        // CreateClient() triggers CreateHost(), which needs ConnectionString to already be set.
        _ = CreateClient();

        await ProvisionSchemaAsync();
    }

    /// <summary>Drops all tables and stops the container.</summary>
    public new async Task DisposeAsync()
    {
        if (_container is null)
            return;

        try
        {
            await DropSchemaAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[IntegrationTestWebAppFactory] Error dropping schema: {ex.Message}");
        }
        finally
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    // -------------------------------------------------------------------------
    // WebApplicationFactory overrides
    // -------------------------------------------------------------------------

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Development")
            .UseSetting("detailedErrors", "true")
            .CaptureStartupErrors(true)
            .ConfigureTestServices(services =>
            {
                // Replace the production connection string so all injected
                // IDbConnection / IDbConnectionFactory instances hit the test DB.
                services.Configure<Microsoft.Extensions.Options.ConfigureNamedOptions<object>>(
                    _ => { });
            })
            .ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = ConnectionString
                });
            });
    }

    // -------------------------------------------------------------------------
    // Schema management
    // -------------------------------------------------------------------------

    /// <summary>Creates the APPUSER and all application tables.</summary>
    private async Task ProvisionSchemaAsync()
    {
        await CreateAppUserAsync();
        await CreateTablesAsync();
    }

    private async Task CreateAppUserAsync()
    {
        var adminConnString = BuildConnectionString("system", AdminPassword);

        await using var conn = new OracleConnection(adminConnString);
        await conn.OpenAsync();

        try
        {
            // Password must be double-quoted in DDL if it contains special characters (e.g. !)
            await conn.ExecuteAsync($"CREATE USER {AppUser} IDENTIFIED BY \"{AppPassword}\"");
        }
        catch (OracleException ex) when (ex.Number == 1920)
        {
            Console.WriteLine($"[IntegrationTestWebAppFactory] User {AppUser} already exists, continuing.");
        }

        var grants = new[]
        {
        $"GRANT CREATE SESSION TO {AppUser}",
        $"GRANT CREATE TABLE TO {AppUser}",
        $"GRANT CREATE SEQUENCE TO {AppUser}",
        $"GRANT UNLIMITED TABLESPACE TO {AppUser}"
    };

        foreach (var grant in grants)
            await conn.ExecuteAsync(grant);
    }

    private async Task CreateTablesAsync()
    {
        await using var conn = new OracleConnection(ConnectionString);
        await conn.OpenAsync();

        foreach (var ddl in TableDefinitions.CreateStatements)
        {
            await using var cmd = new OracleCommand(ddl, conn);
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (OracleException ex) when (ex.Number == 955) // ORA-00955: name already used
            {
                Console.WriteLine($"[IntegrationTestWebAppFactory] Table already exists, skipping.");
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"[IntegrationTestWebAppFactory] DDL failed (ORA-{ex.Number}): {ex.Message}");
                Console.WriteLine($"Statement: {ddl.Trim()[..Math.Min(120, ddl.Trim().Length)]}...");
                throw;
            }
        }
    }

    /// <summary>Drops all application tables in reverse FK order.</summary>
    public async Task DropSchemaAsync()
    {
        await using var conn = new OracleConnection(ConnectionString);
        await conn.OpenAsync();

        foreach (var ddl in TableDefinitions.DropStatements)
        {
            try
            {
                await conn.ExecuteAsync(ddl);
            }
            catch (OracleException ex) when (ex.Number == 942) // ORA-00942: table or view does not exist
            {
                // Nothing to drop – that's fine.
            }
        }
    }

    public async Task CreateSchemaAsync()
    {
        await CreateTablesAsync();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private string BuildConnectionString(string userId, string password)
    {
        var host = _container!.Hostname;
        var port = _container.GetMappedPublicPort(OraclePort);
        return $"User Id={userId};Password={password};Data Source={host}:{port}/{ServiceName}";
    }
}

// =============================================================================
// DDL definitions – kept separate so they are easy to update independently.
// =============================================================================

internal static class TableDefinitions
{
    internal static readonly string[] DropStatements =
    [
        "DROP TABLE NOTIFICATIONS  CASCADE CONSTRAINTS",
        "DROP TABLE ACTIVITY_LOGS  CASCADE CONSTRAINTS",
        "DROP TABLE USER_PREFERENCES CASCADE CONSTRAINTS",
        "DROP TABLE VERSES          CASCADE CONSTRAINTS",
        "DROP TABLE USERS           CASCADE CONSTRAINTS",
        "DROP TABLE USER_PASSAGES   CASCADE CONSTRAINTS",
        "DROP TABLE COLLECTIONS     CASCADE CONSTRAINTS",
        "DROP TABLE PUBLISHED_COLLECTIONS CASCADE CONSTRAINTS",
        "DROP TABLE SAVED_COLLECTIONS CASCADE CONSTRAINTS"
    ];

    internal static readonly string[] CreateStatements =
    [
        """
        CREATE TABLE USERS (
            ID                  NUMBER          GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            USERNAME            VARCHAR2(4000),
            FIRST_NAME          VARCHAR2(4000),
            LAST_NAME           VARCHAR2(4000),
            EMAIL               VARCHAR2(4000),
            AUTH_TOKEN          VARCHAR2(4000),
            USER_STATUS         NUMBER,
            HASHED_PASSWORD     VARCHAR2(4000),
            DATE_REGISTERED     DATE,
            LAST_SEEN           DATE,
            PROFILE_DESCRIPTION VARCHAR2(4000),
            VERSES_MEMORIZED    NUMBER DEFAULT 0,
            POINTS              NUMBER DEFAULT 0,
            PROFILE_PICTURE_URL VARCHAR2(500)
        )
        """,

        """
        CREATE TABLE USER_PREFERENCES (
            USER_ID                                 NUMBER  PRIMARY KEY REFERENCES USERS(ID) ON DELETE CASCADE,
            THEME                                   NUMBER,
            BIBLE_VERSION                           NUMBER,
            COLLECTIONS_SORT                        NUMBER,
            SUBSCRIBED_VOD                          NUMBER,
            PUSH_NOTIFICATIONS_ENABLED              NUMBER,
            NOTIFY_MEMORIZED_VERSE                  NUMBER,
            NOTIFY_PUBLISHED_COLLECTION             NUMBER,
            NOTIFY_COLLECTION_SAVED                 NUMBER,
            NOTIFY_NOTE_LIKED                       NUMBER,
            FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED  NUMBER,
            STREAK_REMINDERS_ENABLED                NUMBER,
            APP_BADGES_ENABLED                      NUMBER,
            PRACTICE_TAB_BADGES_ENABLED             NUMBER,
            TYPE_OUT_REFERENCE                      NUMBER
        )
        """,

        """
        CREATE TABLE ACTIVITY_LOGS (
            ID                  NUMBER  GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            USER_ID             NUMBER  REFERENCES USERS(ID) ON DELETE CASCADE,
            ACTION_TYPE         NUMBER,
            ENTITY_TYPE         NUMBER,
            ENTITY_ID           NUMBER,
            CONTEXT_DESCRIPTION VARCHAR2(1000),
            METADATA_JSON       VARCHAR2(2000),
            SEVERITY_LEVEL      NUMBER  DEFAULT 0 NOT NULL,
            IS_ADMIN_ACTION     NUMBER  DEFAULT 0 NOT NULL,
            CREATED_AT          DATE    DEFAULT SYSDATE NOT NULL
        )
        """,

        """
        CREATE TABLE NOTIFICATIONS (
            ID               NUMBER           GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            MESSAGE          VARCHAR2(1000)   NOT NULL,
            CREATEDDATE      DATE             NOT NULL,
            ISREAD           NUMBER(1,0)      DEFAULT 0,
            EXPIRATION_DATE  DATE,
            NOTIFICATIONTYPE NUMBER,
            SENDER_ID        NUMBER           REFERENCES USERS(ID) ON DELETE CASCADE,
            RECEIVER_ID      NUMBER           REFERENCES USERS(ID) ON DELETE CASCADE
        )
        """,

        """
        CREATE TABLE VERSES (
            VERSE_ID         NUMBER  GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            VERSE_REFERENCE  VARCHAR2(100),
            USERS_SAVED_VERSE NUMBER DEFAULT 0,
            USERS_MEMORIZED  NUMBER DEFAULT 0,
            VERSE_TEXT       VARCHAR2(2000)
        )
        """,

        """
        CREATE TABLE PUBLISHED_COLLECTIONS (
            PUBLISHED_ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            COLLECTION_DESCRIPTION VARCHAR2(2000),
            NUM_SAVES NUMBER,
            TITLE VARCHAR2(100),
            DATE_PUBLISHED DATE,
            AUTHOR_ID NUMBER REFERENCES USERS(ID) ON DELETE SET NULL,
            APPROVED_STATUS NUMBER,
            ORDER_POSITION NUMBER
        )
        """,

        """
         CREATE TABLE COLLECTIONS (
            COLLECTION_ID    NUMBER  GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            USER_ID          NUMBER  REFERENCES USERS(ID) ON DELETE CASCADE,
            TITLE            VARCHAR2(4000),
            VISIBILITY       NUMBER,
            DATE_CREATED     DATE,
            ORDER_POSITION   NUMBER,
            IS_FAVORITES     NUMBER(1,0) DEFAULT 0,
            DESCRIPTION      VARCHAR2(1000),
            PUBLISHED_ID     NUMBER  REFERENCES PUBLISHED_COLLECTIONS(PUBLISHED_ID),
            AUTHOR_ID        NUMBER,
            AUTHOR           VARCHAR2(4000),
            PROGRESS_PERCENT NUMBER,
            NUM_PASSAGES     NUMBER
         )
         """,

         """
         CREATE TABLE USER_PASSAGES (
            ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            REFERENCE VARCHAR2(100),
            DUE_DATE DATE,
            LAST_PRACTICED DATE,
            TIMES_MEMORIZED NUMBER,
            DATE_SAVED DATE,
            COLLECTION_ID NUMBER REFERENCES COLLECTIONS(COLLECTION_ID) ON DELETE CASCADE,
            ORDER_POSITION NUMBER,
            NOTIFY_MEMORIZED NUMBER,
            USER_ID NUMBER REFERENCES USERS(ID) ON DELETE CASCADE,
            PROGRESS_PERCENT NUMBER
         )
        """,

        """
        CREATE TABLE SAVED_COLLECTIONS (
            USER_ID NUMBER REFERENCES USERS(ID),
            PUBLISHED_ID NUMBER REFERENCES PUBLISHED_COLLECTIONS(PUBLISHED_ID),
            DATE_SAVED DATE,
            ORDER_POSITION NUMBER,
            VISIBILITY NUMBER
        )
        """
    ];
}