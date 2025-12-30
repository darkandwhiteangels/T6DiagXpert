using System.Linq.Expressions;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Data.Repositories.Interfaces;

/// <summary>
/// Repository générique EF Core
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query(bool includeDeleted = false);

    Task<TEntity?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);

    Task<List<TEntity>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);

    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false, CancellationToken ct = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false, CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    /// <summary>
    /// Soft delete standard (IsDeleted + DeletedAtUtc) si tu veux le gérer ici.
    /// (Sinon tu peux utiliser Remove si tu préfères hard delete)
    /// </summary>
    void SoftDelete(TEntity entity);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
