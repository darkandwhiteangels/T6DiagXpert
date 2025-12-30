using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.Repositories.Implementations
{
    /// <summary>
    /// Implémentation générique du repository avec support du soft delete.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        #region Helpers

        protected virtual IQueryable<T> BuildQuery(bool includeDeleted)
        {
            return includeDeleted ? _dbSet.IgnoreQueryFilters() : _dbSet.AsQueryable();
        }

        #endregion

        #region Lecture

        public virtual IQueryable<T> Query(bool includeDeleted = false)
        {
            return BuildQuery(includeDeleted);
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return BuildQuery(includeDeleted).Where(predicate);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, bool includeDeleted = false)
        {
            return await BuildQuery(includeDeleted).FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false)
        {
            return await BuildQuery(includeDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await BuildQuery(includeDeleted).Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await BuildQuery(includeDeleted).FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await BuildQuery(includeDeleted).AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false)
        {
            var query = BuildQuery(includeDeleted);
            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync();
        }

        public virtual async Task<PagedResult<T>> GetPagedAsync(
    int pageNumber,
    int pageSize,
    Expression<Func<T, bool>>? filter = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    bool includeDeleted = false)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var query = BuildQuery(includeDeleted);

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }


        #endregion

        #region Écriture

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            entity.Version = 1;

            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var now = DateTime.UtcNow;
            foreach (var entity in entities)
            {
                entity.CreatedAtUtc = now;
                entity.UpdatedAtUtc = now;
                entity.Version = 1;
            }

            await _dbSet.AddRangeAsync(entities);
        }

        public virtual Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.MarkAsModified();
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                entity.MarkAsModified();
            }

            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual Task SoftDeleteAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.MarkAsDeleted();
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task SoftDeleteRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
            }

            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task SoftDeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id, includeDeleted: false);
            if (entity != null)
            {
                await SoftDeleteAsync(entity);
            }
        }

        public virtual async Task RestoreAsync(Guid id)
        {
            var entity = await GetByIdAsync(id, includeDeleted: true);
            if (entity == null)
                throw new InvalidOperationException($"Entity with ID {id} not found");

            entity.Restore();
            _dbSet.Update(entity);
        }

        public virtual Task HardDeleteAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        #endregion

        #region Transactions

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}
