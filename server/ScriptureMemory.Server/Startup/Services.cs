using Dapper;
using DataAccess.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using System.Data;
using System.Text;
using VerseAppNew.Server.Services;
using Pgvector;
using Quartz;
using ScriptureMemory.Server.Data.DataAccess;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Services.BackgroundServices;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ScriptureMemory.Server.Providers;

namespace ScriptureMemory.Server.Startup;

public static class Services
{
    static string otlpEndpoint = "https://<your-otlp-endpoint>";
    static string authHeader = "Authorization=Basic <base64-token>";
    
    /// <summary>
    /// Add authentication for Swagger
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
            };

            o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirements = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                    },
                    []
                }
            };

            o.AddSecurityRequirement(securityRequirements);
        });

        return services;
    }

    public static IServiceCollection ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("Syncer");
            q.AddJob<SyncBibleApiService>(o => o.WithIdentity(jobKey));
            q.AddTrigger(o =>
            {
                o.ForJob(jobKey)
                    .WithIdentity("Syncer-Trigger")
                    .WithCronSchedule("0 0 2 * * ?"); // 2am daily
            });
        });
        
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        
        return services;
    }

    /// <summary>
    /// Add authentication & authorization
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(o =>
        {
            o.AddPolicy("Admin", policy => policy.RequireClaim(
                "role", Enums.UserRole.Admin.ToString(), Enums.UserRole.SuperAdmin.ToString()));
            o.AddPolicy("SuperAdmin", policy => policy.RequireClaim(
                "role", Enums.UserRole.SuperAdmin.ToString()));
            o.AddPolicy("UserOnly", policy => policy.RequireClaim(
                "role", Enums.UserRole.User.ToString()));
            o.AddPolicy("UserOrAdmin", policy => policy.RequireClaim(
                "role", 
                Enums.UserRole.User.ToString(), 
                Enums.UserRole.Admin.ToString(), 
                Enums.UserRole.SuperAdmin.ToString()));
        }); 

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        //
        services.AddEndpointsApiExplorer();
        
        //
        services.AddSwaggerGenWithAuth();

        //
        //SqlMapper.AddTypeHandler(new VectorTypeHandler());

        
        // Add custom exception handler that logs exceptions
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        var connectionString = configuration.GetConnectionString("PostgresConnection")
            ?? throw new Exception("Connection string not found in configuration");
        
        // Add DbContext
        services.AddDbContextFactory<ApplicationDbContext>(
            options => options.UseNpgsql(connectionString, o => o.UseVector()),
            ServiceLifetime.Scoped);
        
        // Add postgres data source for integration tests
        // services.AddNpgsqlDataSource(connectionString);
        
        //
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
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        services.AddHttpLogging(options =>
        {
            options.LoggingFields =
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.RequestQuery |
                HttpLoggingFields.RequestHeaders |
                HttpLoggingFields.RequestBody |
                HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.Duration;

            options.RequestHeaders.Add("Authorization");
            options.RequestHeaders.Add("User-Agent");
            options.RequestHeaders.Add("X-Correlation-ID");

            options.CombineLogs = true;
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSignalR();

        services.AddScoped<UserService>();
        services.AddScoped<AdminService>();
        //services.AddScoped<EmailSenderService>();
        // services.AddScoped<ActivityLogger>();
        //services.AddScoped<PasswordResetService>();
        //services.AddScoped<PopulateDatabase>();
        //services.AddScoped<SearchService>();
        // services.AddScoped<NotificationService>();
        //services.AddScoped<VerseService>();
        //services.AddScoped<UserPassageService>();
        //services.AddScoped<CollectionService>();
        services.AddScoped<TokenProvider>();
        //services.AddScoped<VerseOfDayService>();

        //services.AddScoped<VerseManagement>();
        services.AddScoped<BibleApi>();
        services.AddScoped<BibleSyncer>();

        //services.AddScoped<EmbeddingGenerator>();

        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped<IUserData, UserDataEFCore>();
        services.AddScoped<IAdminData, AdminDataEfCore>();
        services.AddScoped<IVerseData, VerseDataEfCore>();
        services.AddScoped<BibleData>();
        // services.AddScoped<UserSettingsData>();
        // //services.AddScoped<CrossReferenceData>();
        // services.AddScoped<ActivityLoggingData>();
        // //services.AddScoped<SearchData>();
        // services.AddScoped<NotificationData>();
        // //services.AddScoped<VerseData>();
        // //services.AddScoped<UserPassageData>();
        // services.AddScoped<CollectionData>();
        //services.AddScoped<PublishedCollectionData>();
        //services.AddScoped<VerseOfDayData>();
        services.AddScoped<ISessionData, SessionDataEfCore>();
        return services;
    }

    public static ILoggingBuilder ConfigureOpenTelemetrySignalRLogging(
        this ILoggingBuilder logger, 
        IConfiguration configuration)
    {
        logger.ClearProviders();

        logger.AddConsole(); // temp
        
        // Disable in development
        // logger.AddOpenTelemetry(o =>
        // {
        //     o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("verse-app"));
        //     o.IncludeFormattedMessage = true;
        //     o.IncludeScopes = true;
        //     o.ParseStateValues = true;
        //     o.AddOtlpExporter(opt =>
        //     {
        //         opt.Endpoint = new Uri($"{configuration["Grafana:Endpoint"]}/v1/logs");
        //         opt.Headers = $"Authorization=Basic {configuration["Grafana:AuthToken"]}";
        //     });
        // });
        
        // Block Grafana / Loki http logs
        logger.AddFilter("System.Net.Http.HttpClient.OtlpLogExporter", LogLevel.None);
        logger.AddFilter("System.Net.Http.HttpClient.OtlpMetricExporter", LogLevel.None);
        logger.AddFilter("System.Net.Http.HttpClient.OtlpTraceExporter", LogLevel.None);

        return logger;
    }

    public static IServiceCollection ConfigureTracingAndMetricsExporting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(o => {
                    o.Endpoint = new Uri($"{configuration["Grafana:Endpoint"]}/v1/traces");
                    o.Headers = $"Authorization=Basic {configuration["Grafana:AuthToken"]}";
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(o => {
                    o.Endpoint = new Uri($"{configuration["Grafana:Endpoint"]}/v1/metrics");
                    o.Headers = $"Authorization=Basic {configuration["Grafana:AuthToken"]}";
                }));

        return services;
    }
}
