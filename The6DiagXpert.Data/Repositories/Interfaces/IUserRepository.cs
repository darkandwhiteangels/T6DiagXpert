using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Data.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, bool includeDeleted = false, CancellationToken ct = default);

    Task<User?> GetByFirebaseUidAsync(string firebaseUid, bool includeDeleted = false, CancellationToken ct = default);

    Task<List<User>> GetByCompanyIdAsync(Guid companyId, bool includeDeleted = false, CancellationToken ct = default);
}
