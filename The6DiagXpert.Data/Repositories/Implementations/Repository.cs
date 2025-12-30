using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.Repositories.Implementations;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> Set;

    public Repository(AppDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Set = Context.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> Query(bool includeDeleted = false)
    {
        return includeDeleted
            ? Set.IgnoreQueryFilters()
            : Set.AsQueryable();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        return await Query(includeDeleted)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public virtual async Task<List<TEntity>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
    {
        return await Query(includeDeleted)
            .ToListAsync(ct);
    }

    public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return await Query(includeDeleted)
            .Where(predicate)
            .ToListAsync(ct);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return await Query(includeDeleted)
            .AnyAsync(predicate, ct);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await Set.AddAsync(entity, ct);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await Set.AddRangeAsync(entities, ct);
    }

    public virtual void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Remove(entity);
    }

    public virtual void SoftDelete(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.MarkAsModified();

        Set.Update(entity);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return Context.SaveChangesAsync(ct);
    }
}
