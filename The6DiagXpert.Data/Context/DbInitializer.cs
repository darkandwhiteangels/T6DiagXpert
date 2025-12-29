using Microsoft.EntityFrameworkCore;
using The6DiagXpert.Data.Seed;

namespace The6DiagXpert.Data.Context;

/// <summary>
/// Initialisation base locale : migrations + seeds
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, bool applyMigrations = true, bool seed = true)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (applyMigrations)
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        if (seed)
        {
            await SeedAsync(context);
        }
    }

    private static async Task SeedAsync(AppDbContext context)
    {
        // Seed ordre logique : Company puis User (car User a potentiellement CompanyId)
        await CompanySeed.EnsureSeededAsync(context);
        await UserSeed.EnsureSeededAsync(context);
    }
}
