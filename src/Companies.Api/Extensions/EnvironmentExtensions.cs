using Companies.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Companies.Api.Extensions;

public static class EnvironmentExtensions
{
    public static WebApplication MapEnvironmentConfigurations(this WebApplication app, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            UseDevelopmentSettings(app);
        }
        else if (environment.IsProduction())
        {
            UseProductionSettings(app);
        }

        return app;
    }

    public static WebApplication UseDevelopmentSettings(WebApplication app)
    {
        // swagger

        app.UseApiVersionedSwagger();

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CompaniesContext>();

        // aplica migrações do EF Core antes do seed
        context.Database.Migrate();

        // seed sample data
        var databaseSeed = new CompaniesDatabaseSeed(context);
        databaseSeed.SeedData();

        return app;
    }

    public static WebApplication UseProductionSettings(WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }
}
