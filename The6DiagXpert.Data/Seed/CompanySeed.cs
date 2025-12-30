using Microsoft.EntityFrameworkCore;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Data.Seed;

public static class CompanySeed
{
    public static async Task EnsureSeededAsync(AppDbContext context, CancellationToken ct = default)
    {
        if (await context.Companies.AnyAsync(ct))
            return;

        // Seed minimal (adaptable)
        var company = new Company
        {
            Name = "Demo Company",
            LegalName = "Demo Company SARL",
            // ⚠️ IMPORTANT : ton CompanyConfiguration utilise c.Siret (pas SiretNumber)
            // Donc on remplit Siret si la propriété existe dans ton modèle Core.
            Siret = "00000000000000",
            SubscriptionPlan = "Free",
            IsVerified = false
        };

        await context.Companies.AddAsync(company, ct);
        await context.SaveChangesAsync(ct);
    }
}
