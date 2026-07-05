using Npgsql;
using ScriptureMemory.Server.Endpoints;
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
        // app.ConfigureSearchEndpoints();
        app.ConfigureAdminEndpoints();
        app.ConfigureBibleEndpoints();
        
        return app;
    }
}
