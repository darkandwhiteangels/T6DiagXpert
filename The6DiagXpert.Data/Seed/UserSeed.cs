using Microsoft.EntityFrameworkCore;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Data.Seed;

public static class UserSeed
{
    public static async Task EnsureSeededAsync(AppDbContext context, CancellationToken ct = default)
    {
        if (await context.Users.AnyAsync(ct))
            return;

        var company = await context.Companies.FirstOrDefaultAsync(ct);

        var user = new User
        {
            FirebaseUid = "LOCAL-DEMO-UID",
            Email = "demo@the6diagxpert.local",
            EmailVerified = true,
            FirstName = "Demo",
            LastName = "User",
            DisplayName = "Demo User",
            Role = UserRole.CompanyOwner,
            CompanyId = company?.Id ?? Guid.Empty,
            IsActive = true,
            IsLocked = false,
            AcceptedTerms = true,
            GdprConsent = true,
            NotificationsEnabled = true
        };

        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }
}
