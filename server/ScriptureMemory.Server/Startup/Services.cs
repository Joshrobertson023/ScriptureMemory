using DataAccess.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using System.Data;
using System.Text;
using VerseAppNew.Server.Bogus;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.Server.Startup;

public static class Services
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = 
            };
        });
    }

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
        services.AddSwaggerGenWithAuth();

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
        services.AddScoped<AdminService>();
        services.AddScoped<EmailSenderService>();
        services.AddScoped<ActivityLogger>();
        services.AddScoped<PasswordResetService>();
        services.AddScoped<PopulateDatabase>();
        services.AddScoped<SearchService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<VerseService>();
        //services.AddScoped<UserPassageService>();
        services.AddScoped<CollectionService>();
        services.AddScoped<TokenProvider>();

        services.AddScoped<VerseManagement>();

        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped<UserData>();
        services.AddScoped<AdminData>();
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
