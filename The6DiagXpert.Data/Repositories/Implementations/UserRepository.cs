using Microsoft.EntityFrameworkCore;
using The6DiagXpert.Core.Models.Identity;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.Repositories.Implementations;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, bool includeDeleted = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        var normalized = email.Trim().ToLowerInvariant();

        return await Query(includeDeleted)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, ct);
    }

    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid, bool includeDeleted = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid)) return null;

        var uid = firebaseUid.Trim();

        return await Query(includeDeleted)
            .FirstOrDefaultAsync(u => u.FirebaseUid == uid, ct);
    }

    public async Task<List<User>> GetByCompanyIdAsync(Guid companyId, bool includeDeleted = false, CancellationToken ct = default)
    {
        if (companyId == Guid.Empty) return new List<User>();

        return await Query(includeDeleted)
            .Where(u => u.CompanyId == companyId)
            .ToListAsync(ct);
    }
}
