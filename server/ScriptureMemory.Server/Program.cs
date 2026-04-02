using Microsoft.AspNetCore.Authentication.JwtBearer;
using ScriptureMemory.Server.Startup;
using ScriptureMemory.Server.Tools;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddDatabaseConnections()
    .AddServices()
    .AddDataAccess();

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Admins", policy => policy.RequireClaim("role", Enums.UserRoles.Admin, Enums.UserRoles.SuperAdmin));
    o.AddPolicy("SuperAdmins", policy => policy.RequireClaim("role", Enums.UserRoles.SuperAdmin));
    o.AddPolicy("UsersOnly", policy => policy.RequireClaim("role", Enums.UserRoles.User));
    o.AddPolicy("UsersAndAdmins", policy => policy.RequireClaim("role", Enums.UserRoles.User, Enums.UserRoles.Admin, Enums.UserRoles.SuperAdmin));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseMiddleware()
   .UseEndpoints();
app.UseAuthentication();
app.UseAuthorization();

//using var scope = app.Services.CreateScope();
//{
//    var service = scope.ServiceProvider.GetRequiredService<VerseManagement>();
//    await service.UploadVersesToPostgres();
//}

app.Run();

public partial class Program() { }
