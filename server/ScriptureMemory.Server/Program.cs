using Dapper;
using DataAccess.Data;
using DataAccess.DataInterfaces;
using System;
using VerseAppNew.Server.Apis;
using VerseAppNew.Server.Bogus;
using VerseAppNew.Server.Endpoints;
using VerseAppNew.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IVerseService, VerseService>();
builder.Services.AddScoped<PopulateDatabase>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Data Access
builder.Services.AddScoped<IUserData, UserData>();
builder.Services.AddScoped<IUserSettingsData, UserSettingsData>();
builder.Services.AddScoped<IActivityLoggingData, ActivityLoggingData>();
builder.Services.AddScoped<ISearchData, DataAccess.Data.SearchData>();
builder.Services.AddScoped<INotificationData, NotificationData>();
//builder.Services.AddScoped<IVerseData, VerseData>();
//builder.Services.AddScoped<IPaidData, PaidData>();
//builder.Services.AddScoped<ICategoryData, CategoryData>();
//builder.Services.AddScoped<IUserPassageData, UserPassageData>();
//builder.Services.AddScoped<ICollectionData, CollectionData>();
//builder.Services.AddScoped<IPracticeLogData, PracticeLogData>();
//builder.Services.AddScoped<IPracticeSessionData, PracticeSessionData>();
//builder.Services.AddScoped<IVerseOfDayData, VerseOfDayData>();
//builder.Services.AddScoped<IVerseOfDaySuggestionData, VerseOfDaySuggestionData>();
//builder.Services.AddScoped<IRelationshipData, RelationshipData>();
//builder.Services.AddScoped<IReportData, ReportData>();
//builder.Services.AddScoped<IPublishedCollectionData, PublishedCollectionData>();
//builder.Services.AddScoped<IActivityData, ActivityData>();
//builder.Services.AddScoped<IBannerData, BannerData>();
//builder.Services.AddScoped<IAdminData, AdminData>();
//builder.Services.AddScoped<IPushTokenData, PushTokenData>();
//builder.Services.AddScoped<IHighlightData, HighlightData>();
//builder.Services.AddScoped<IVerseNoteData, VerseNoteData>();
//builder.Services.AddScoped<INoteLikeData, NoteLikeData>();
//builder.Services.AddScoped<IBanData, BanData>();


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


var app = builder.Build();

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
