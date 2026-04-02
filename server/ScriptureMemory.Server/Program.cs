using Microsoft.AspNetCore.Authentication.JwtBearer;
using ScriptureMemory.Server.Startup;
using ScriptureMemory.Server.Tools;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddServices()
    .AddDataAccess();

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Admin", policy => policy.RequireClaim("role", Enums.UserRole.Admin.ToString(), Enums.UserRole.SuperAdmin.ToString()));
    o.AddPolicy("SuperAdmin", policy => policy.RequireClaim("role", Enums.UserRole.SuperAdmin.ToString()));
    o.AddPolicy("UserOnly", policy => policy.RequireClaim("role", Enums.UserRole.User.ToString()));
    o.AddPolicy("UserOrAdmin", policy => policy.RequireClaim("role", Enums.UserRole.User.ToString(), Enums.UserRole.Admin.ToString(), Enums.UserRole.SuperAdmin.ToString()));
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

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware()
   .UseEndpoints();

//using var scope = app.Services.CreateScope();
//{
//    var service = scope.ServiceProvider.GetRequiredService<VerseManagement>();
//    await service.UploadVersesToPostgres();
//}

app.Run();

public partial class Program() { }
