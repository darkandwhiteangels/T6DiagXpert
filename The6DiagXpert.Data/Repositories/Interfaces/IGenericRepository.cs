using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interface générique pour les repositories avec support du soft delete.
    /// </summary>
    /// <typeparam name="T">Type d'entité (doit hériter de BaseEntity)</typeparam>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        #region Lecture (Read)

        /// <summary>
        /// Récupère une entité par son ID (excluant les suppressions logiques).
        /// </summary>
        Task<T?> GetByIdAsync(Guid id, bool includeDeleted = false);

        /// <summary>
        /// Récupère toutes les entités (excluant les suppressions logiques par défaut).
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);

        /// <summary>
        /// Récupère les entités correspondant à une condition.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false);

        /// <summary>
        /// Récupère une seule entité correspondant à une condition.
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false);

        /// <summary>
        /// Vérifie si une entité existe selon une condition.
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false);

        /// <summary>
        /// Compte les entités correspondant à une condition.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false);

        /// <summary>
        /// Récupère des entités avec pagination.
        /// </summary>
        Task<PagedResult<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool includeDeleted = false
        );

        #endregion

        #region Écriture (Create, Update, Delete)

        /// <summary>
        /// Ajoute une nouvelle entité.
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Ajoute plusieurs entités.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Met à jour une entité.
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Met à jour plusieurs entités.
        /// </summary>
        Task UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Suppression logique d'une entité (soft delete).
        /// </summary>
        Task SoftDeleteAsync(T entity);

        /// <summary>
        /// Suppression logique d'une entité par son ID.
        /// </summary>
        Task SoftDeleteAsync(Guid id);

        /// <summary>
        /// Suppression logique de plusieurs entités.
        /// </summary>
        Task SoftDeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Restaure une entité supprimée logiquement.
        /// </summary>
        Task RestoreAsync(Guid id);

        /// <summary>
        /// Suppression physique définitive (à utiliser avec précaution).
        /// </summary>
        Task HardDeleteAsync(T entity);

        #endregion

        #region Requêtes avancées

        /// <summary>
        /// Récupère un IQueryable pour des requêtes complexes.
        /// </summary>
        IQueryable<T> Query(bool includeDeleted = false);

        /// <summary>
        /// Récupère un IQueryable avec une condition.
        /// </summary>
        IQueryable<T> Query(Expression<Func<T, bool>> predicate, bool includeDeleted = false);

        #endregion

        #region Transactions

        /// <summary>
        /// Sauvegarde les changements.
        /// </summary>
        Task<int> SaveChangesAsync();

        #endregion
    }

    /// <summary>
    /// Résultat paginé.
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}