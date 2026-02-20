using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemoryLibrary;
using VerseAppNew.Server.Services;
using static ScriptureMemoryLibrary.Enums;

namespace VerseAppNew.Server.Bogus;

public sealed class PopulateDatabase
{
    private readonly IUserService userService;
    private readonly IUserData userContext;
    private readonly INotificationService notificationService;
    private readonly ISearchService searchService;
    private readonly ILogger<PopulateDatabase> logger;

    public PopulateDatabase(
        IUserService userService,
        IUserData userContext,
        INotificationService notificationService,
        ISearchService searchService,
        ILogger<PopulateDatabase> logger)
    {
        this.userService = userService;
        this.userContext = userContext;
        this.notificationService = notificationService;
        this.searchService = searchService;
        this.logger = logger;
    }

    public async Task Populate()
    {
        //await PopulateUsers();
    }


    public async Task PopulateUsers(int count = 100)
    {
        logger.LogInformation($"Starting PopulateUsers() with {count} count");

        var userGenerator = new UserGenerator();
        var generatedUsers = userGenerator.Generate(count);
        foreach (var user in generatedUsers)
        {
            CreateUserRequest request = new CreateUserRequest
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.HashedPassword?.Length >= Data.MIN_PASSWORD_LENGTH ? user.HashedPassword : user.HashedPassword+user.HashedPassword+user.HashedPassword
            };
            await userService.CreateUserFromRequest(request);
        }

        logger.LogInformation($"Finished PopulateUsers() with {count} count");
    }

    public async Task PopulateSearch(int count = 10000, int uniqueTerms = 100)
    {
        logger.LogInformation($"Starting PopulateSearch() with {count} count");

        var random = new Random();
        var searchGenerator = new SearchGenerator();
        var generatedSearches = searchGenerator.Generate(uniqueTerms);

        var users = await userContext.GetUsers(100);

        for (int i = 0; i < count; i++)
        {
            var randomSearch = generatedSearches[random.Next(generatedSearches.Count)];
            SearchRequest request = new(
                users[random.Next(users.Count)].Username,
                randomSearch.SearchTerm,
                randomSearch.SearchType
            );

            switch (request.SearchType)
            {
                case SearchType.Verse:
                    await searchService.SearchVerses(request);
                    break;
                default:
                    break;
            }
        }

        logger.LogInformation($"Finished PopulateSearch() with {count} count");
    }
}
