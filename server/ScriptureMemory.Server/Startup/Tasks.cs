namespace ScriptureMemory.Server.Startup;

public static class Tasks
{
    public static async Task<WebApplication> AskToRunStartupTasks(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var tasks = new List<(string TaskName, Func<Task> Task)>
        {
            ("Sync database with API.Bible", () => SyncDatabaseWithApiBible(scope))
        };
        
        Console.WriteLine("\n\nSome startup tasks have been found to run: ");
        for (int i = 0; i < tasks.Count; i++)
            Console.WriteLine($"  {i + 1}) {tasks[i].TaskName}");
        
        Console.WriteLine("\n\nWould you like to run the startup tasks? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y")
            return app;

        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"\n\nRunning task #{i + 1}) {tasks[i].TaskName}...");
            await tasks[i].Task();
        }
        
        Console.WriteLine("\n\nAll startup tasks finished.");
        
        return app;
    }

    public static async Task SyncDatabaseWithApiBible(IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<BibleApi>();
        await service.SyncDatabaseWithApiBible(); 
    }
}