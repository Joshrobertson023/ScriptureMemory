using Dapper;
using DataAccess.Data;
using DataAccess.DataInterfaces;
using ScriptureMemory.Server.Services;
using System;
using VerseAppNew.Server.Apis;
using VerseAppNew.Server.Bogus;
using VerseAppNew.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<PopulateDatabase>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IVerseService, VerseService>();
builder.Services.AddScoped<IUserPassageService, UserPassageService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();

// Data Access
builder.Services.AddScoped<IUserData, UserData>();
builder.Services.AddScoped<IUserSettingsData, UserSettingsData>();
builder.Services.AddScoped<IActivityLoggingData, ActivityLoggingData>();
builder.Services.AddScoped<ISearchData, DataAccess.Data.SearchData>();
builder.Services.AddScoped<INotificationData, NotificationData>();
builder.Services.AddScoped<IVerseData, VerseData>();
builder.Services.AddScoped<IUserPassageData, UserPassageData>();
builder.Services.AddScoped<ICollectionData, CollectionData>();


builder.Services.AddHttpClient("ExpoPush", client =>
{
    client.BaseAddress = new Uri("https://exp.host");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseDefaultFiles();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.ConfigureUserEndpoints();
//app.ConfigureVerseEndpoints();
//app.ConfigureVerseOfDayEndpoints();
//app.ConfigureUserPassageEndpoints();
//app.ConfigureCollectionEndpoints();
//app.ConfigurePracticeLogEndpoints();
//app.ConfigurePracticeSessionEndpoints();
//app.ConfigureNotificationEndpoints();
//app.ConfigureAdminEndpoints();
//app.ConfigureRelationshipEndpoints();
//app.ConfigurePopularSearchEndpoints();
//app.ConfigureReportEndpoints();
//app.ConfigureCategoryEndpoints();
//app.ConfigureActivityEndpoints();
//app.ConfigurePushTokenEndpoints();
//app.ConfigureHighlightEndpoints();
//app.ConfigureVerseNoteEndpoints();
//app.ConfigureBanEndpoints();


using var scope = app.Services.CreateScope();
{
    var populateService = scope.ServiceProvider.GetRequiredService<PopulateDatabase>();
    await populateService.Populate();
}



app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");



public partial class Program() { }
