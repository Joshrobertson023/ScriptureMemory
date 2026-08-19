using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.Server.Startup;

/// <summary>
/// A series of startup tasks that run in the console before the app starts
/// </summary>
public static class Tasks
{
    public static async Task<WebApplication> AskToRunOptionalStartupTasks(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var tasks = new List<(string TaskName, Func<Task> Task)>
        {
            ("Upload verses from csv", () => UploadVersesFromCsv(scope)),
            ("Recreate vector index in database", () => CreateIndex(scope)),
            ("Add Version column to all verses", () => UpdateVersionInVerses(scope))
        };

        if (tasks.Count == 0)
            return app;
            
        Console.WriteLine("\n\nSome startup tasks have been found to run: ");
        for (int i = 0; i < tasks.Count; i++)
            Console.WriteLine($"  {i + 1}) {tasks[i].TaskName}");
        
        Console.WriteLine("\n\nEnter the number you would like to run, 'a' to run all, or press ENTER to skip: ");
        string? input = Console.ReadLine();
        
        if (string.IsNullOrEmpty(input))
            return app;
        
        if (input.Trim().ToLower() == "a")
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"\n\nRunning task #{i + 1}) {tasks[i].TaskName}...");
                await tasks[i].Task();
            }
        }
        else
        {
            int numToRun = int.Parse(input);
            await tasks[numToRun - 1].Task();
        }
        
        Console.WriteLine("\n\nStartup tasks finished.");
        
        return app;
    }

    public static async Task UploadVersesFromCsv(IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<VerseManagement>();
        await service.UploadVersesToPostgres();
    }

    public static async Task CreateIndex(IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<VerseDataDapper>();
        await service.CreateVectorIndex();
    }

    public static async Task UpdateVersionInVerses(IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<VerseDataDapper>();
        await service.AddVersionToAllVerses();
    }
}