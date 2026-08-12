using Microsoft.AspNetCore.SignalR;
using Npgsql;
using ScriptureMemory.Server.Endpoints;
using ScriptureMemory.Server.Providers;
using ScriptureMemory.Server.SignalR;
// using ScriptureMemory.Server.Endpoints;
// using VerseAppNew.Server.Apis;
using VerseAppNew.Server.Endpoints;

namespace ScriptureMemory.Server.Startup;

public static class Middleware
{
    public static WebApplication UseMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Use middleware exceptions with Problem Details web standard
            app.UseExceptionHandler("/Error"); 
            
            //
            app.UseHsts();
        }
        
        //app.UseHttpsRedirection();
        //app.UseStaticFiles();

        app.UseStatusCodePages();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseHttpLogging();

        app.UseExceptionHandler();
        
        // Add SignalR logger
        var hubContext = app.Services.GetRequiredService<IHubContext<LogHub>>();
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        loggerFactory.AddProvider(new SignalRLoggerProvider(hubContext));

        app.UseCors();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        return app;
    }

    public static WebApplication UseEndpoints(this WebApplication app)
    {
        // app.ConfigureUserEndpoints();
        // app.ConfigureVerseOfDayEndpoints();
        // app.ConfigureVerseEndpoints();
        app.ConfigureSearchEndpoints();
        app.ConfigureAdminEndpoints();
        app.ConfigureBibleEndpoints();
        app.ConfigureLogEndpoints();
        
        return app;
    }
}
