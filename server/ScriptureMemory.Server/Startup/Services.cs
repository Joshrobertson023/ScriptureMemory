using DataAccess.Data;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using System.Data;
using VerseAppNew.Server.Bogus;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.Server.Startup;

public static class Services
{
    public static IServiceCollection AddDatabaseConnections(this IServiceCollection services)
    {
        services.AddKeyedScoped<IDbConnection>("Oracle", (sp, _) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new OracleConnection(
                config.GetConnectionString("DefaultConnection")
            );
        });

        services.AddKeyedScoped<IDbConnection>("Postgres", (sp, _) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new NpgsqlConnection(
                config.GetConnectionString("PostgresConnection")
            );
        });

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpClient("ExpoPush", client =>
        {
            client.BaseAddress = new Uri("https://exp.host");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<UserService>();
        services.AddScoped<EmailSenderService>();
        services.AddScoped<ActivityLogger>();
        services.AddScoped<PasswordResetService>();
        services.AddScoped<PopulateDatabase>();
        services.AddScoped<SearchService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<VerseService>();
        //services.AddScoped<UserPassageService>();
        services.AddScoped<CollectionService>();

        services.AddScoped<VerseManagement>();

        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped<UserData>();
        services.AddScoped<UserSettingsData>();
        services.AddScoped<ActivityLoggingData>();
        services.AddScoped<SearchData>();
        services.AddScoped<NotificationData>();
        services.AddScoped<VerseData>();
        services.AddScoped<UserPassageData>();
        services.AddScoped<CollectionData>();
        services.AddScoped<PublishedCollectionData>();
        return services;
    }
}
