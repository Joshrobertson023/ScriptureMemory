using Npgsql;
using ScriptureMemory.Server.Endpoints;
using VerseAppNew.Server.Apis;
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
            
        }
        else
        {
            //app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    public static WebApplication UseEndpoints(this WebApplication app)
    {
        app.ConfigureUserEndpoints();
        app.ConfigureVerseOfDayEndpoints();
        app.ConfigureVerseEndpoints();
        app.ConfigureSearchEndpoints();
        app.ConfigureAdminEndpoints();
        app.ConfigureCategoriesEndpoints();
        return app;
    }
}
