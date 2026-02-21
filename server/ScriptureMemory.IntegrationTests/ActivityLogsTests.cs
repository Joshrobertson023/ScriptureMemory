using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemoryLibrary;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests;

public class ActivityLogsTests : BaseIntegrationTest
{
    public ActivityLogsTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task ActivityLogsTest_CreateAndRetrieveActivityLogs()
    {
        var testUser = new CreateUserRequest
        {
            Username = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com",
            Password = "password12355555"
        };

        await userService.CreateUserFromRequest(testUser);
        var user = await userContext.GetUserFromUsername(testUser.Username);
        Assert.NotNull(user);

        int userId = user.Id;
        Assert.True(userId > 0);


        var activityLog1 = new ActivityLog(
            userId: userId,
            actionType: ActionType.Create,
            entityType: EntityType.Verse,
            entityId: 1,
            contextDescription: "User created a new verse memorization",
            jsonMetadata: new { bookName = "John", chapter = 3, verse = 16 },
            severityLevel: SeverityLevel.Info
        );

        var activityLog2 = new ActivityLog(
            userId: userId,
            actionType: ActionType.Update,
            entityType: EntityType.Verse,
            entityId: 1,
            contextDescription: "User updated verse progress",
            jsonMetadata: new { progressPercentage = 75 },
            severityLevel: SeverityLevel.Info
        );

        var activityLog3 = new ActivityLog(
            userId: userId,
            actionType: ActionType.Delete,
            entityType: EntityType.Verse,
            entityId: 2,
            contextDescription: "User deleted a verse",
            jsonMetadata: null,
            severityLevel: SeverityLevel.Warning
        );

        var adminActionLog = new ActivityLog(
            userId: userId,
            actionType: ActionType.Update,
            entityType: EntityType.User,
            entityId: userId,
            contextDescription: "Admin updated user settings",
            jsonMetadata: new { changedField = "email" },
            severityLevel: SeverityLevel.Warning,
            isAdminAction: true
        );

        await activityLogContext.Create(activityLog1);
        await activityLogContext.Create(activityLog2);
        await activityLogContext.Create(activityLog3);
        await activityLogContext.Create(adminActionLog);


        var userLogs = await activityLogContext.GetByUser(userId, page: 1, pageSize: 50);
        
        Assert.NotNull(userLogs);
        Assert.Equal(userId, userLogs.Items[0].UserId);
        Assert.Equal(1, userLogs.Page);
        Assert.Equal(50, userLogs.PageSize);


        var verseLogs = await activityLogContext.GetByEntity(EntityType.Verse, entityId: 1);
        
        Assert.NotNull(verseLogs);
        Assert.All(verseLogs.Items, log => Assert.Equal(EntityType.Verse, log.EntityType));
        Assert.All(verseLogs.Items, log => Assert.Equal(1, log.EntityId));


        var createActionLogs = await activityLogContext.GetByActionType(ActionType.Create);
        
        Assert.NotNull(createActionLogs);
        Assert.Equal(ActionType.Create, createActionLogs.Items[0].ActionType);
        Assert.Equal(EntityType.Verse, createActionLogs.Items[0].EntityType);


        var adminLogs = await activityLogContext.GetAdminActions();
        
        Assert.NotNull(adminLogs);
        Assert.True(adminLogs.Items[0].IsAdminAction);
        Assert.Equal(ActionType.Update, adminLogs.Items[0].ActionType);
        Assert.Equal(EntityType.User, adminLogs.Items[0].EntityType);


        var now = DateTime.UtcNow;
        var yesterday = now.AddDays(-1);
        var tomorrow = now.AddDays(1);
        
        var dateRangeLogs = await activityLogContext.GetByDateRange(yesterday, tomorrow);
        
        Assert.NotNull(dateRangeLogs);
        Assert.True(dateRangeLogs.Items.Count >= 4, "Should have at least the 4 logs we created");


        var logDetails = userLogs.Items.Last();
        Assert.NotNull(logDetails.CreatedAt);
        Assert.True(logDetails.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(userId, logDetails.UserId);
        Assert.Equal(ActionType.Create, logDetails.ActionType);
        Assert.Equal(EntityType.Verse, logDetails.EntityType);
        Assert.Equal(1, logDetails.EntityId);
        Assert.NotNull(logDetails.ContextDescription);
        Assert.NotNull(logDetails.JsonMetadata);
        Assert.Equal(SeverityLevel.Info, logDetails.SeverityLevel);
        Assert.False(logDetails.IsAdminAction);
    }

    //[Fact]
    //public async Task ActivityLogsTest_PaginationWorks()
    //{
    //    var testUser = new CreateUserRequest
    //    {
    //        Username = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}",
    //        FirstName = "Test",
    //        LastName = "User",
    //        Email = "testuser@example.com",
    //        Password = "password12355555"
    //    };

    //    await userService.CreateUserFromRequest(testUser);
    //    var user = await userContext.GetUserFromUsername(testUser.Username);
    //    Assert.NotNull(user);

    //    int userId = user.Id;
    //    Assert.True(userId > 0);


    //    for (int i = 0; i < 5; i++)
    //    {
    //        var log = new ActivityLog(
    //            userId: userId,
    //            actionType: ActionType.Create,
    //            entityType: EntityType.Verse,
    //            entityId: i,
    //            contextDescription: $"Log entry {i}",
    //            jsonMetadata: null,
    //            severityLevel: SeverityLevel.Info
    //        );
    //        await activityLogContext.Create(log);
    //    }

    //    var page1 = await activityLogContext.GetByUser(userId, page: 1, pageSize: 2);
    //    Assert.Equal(1, page1.Page);
    //    Assert.Equal(2, page1.PageSize);

    //    var page2 = await activityLogContext.GetByUser(userId, page: 2, pageSize: 2);
    //    Assert.Equal(2, page2.Page);
    //    Assert.Equal(2, page2.PageSize);

    //    var page3 = await activityLogContext.GetByUser(userId, page: 3, pageSize: 2);
    //    Assert.Equal(3, page3.Page);
    //    Assert.Equal(2, page3.PageSize);

    //    Assert.DoesNotContain(page1.Items[0].Id, page2.Items.Select(x => x.Id));
    //    Assert.DoesNotContain(page2.Items[0].Id, page3.Items.Select(x => x.Id));
    //}
}
