using VerseAppNew.Server.Apis;
using VerseAppNew.Server.Endpoints;

namespace ScriptureMemory.Server.Startup;

public static class Middleware
{
    public static WebApplication UseMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();
        return app;
    }

    public static WebApplication UseEndpoints(this WebApplication app)
    {
        app.ConfigureUserEndpoints();
        app.ConfigureVerseOfDayEndpoints();
        //app.ConfigureVerseEndpoints();
        //app.ConfigureVerseOfDayEndpoints();
        //app.ConfigureUserPassageEndpoints();
        //app.ConfigureCollectionEndpoints();
        //app.ConfigurePracticeLogEndpoints();
        //app.ConfigurePracticeSessionEndpoints();
        //app.ConfigureNotificationEndpoints();
        app.ConfigureAdminEndpoints();
        //app.ConfigureRelationshipEndpoints();
        //app.ConfigurePopularSearchEndpoints();
        //app.ConfigureReportEndpoints();
        //app.ConfigureCategoryEndpoints();
        //app.ConfigureActivityEndpoints();
        //app.ConfigurePushTokenEndpoints();
        //app.ConfigureHighlightEndpoints();
        //app.ConfigureVerseNoteEndpoints();
        //app.ConfigureBanEndpoints();
        return app;
    }
}
