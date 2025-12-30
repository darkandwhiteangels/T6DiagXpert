using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.UnitOfWork;

/// <summary>
/// Interface du Unit of Work pour gérer les transactions et coordonner les repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository pour les missions.
    /// </summary>
    IMissionRepository Missions { get; }

    /// <summary>
    /// Repository pour les clients.
    /// </summary>
    IClientRepository Clients { get; }

    /// <summary>
    /// Repository pour les propriétés.
    /// </summary>
    IPropertyRepository Properties { get; }

    /// <summary>
    /// Sauvegarde toutes les modifications dans la base de données.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nombre d'entités modifiées.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Démarre une transaction.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Valide la transaction en cours.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Annule la transaction en cours.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtient un repository générique pour une entité spécifique.
    /// </summary>
    /// <typeparam name="TEntity">Type de l'entité.</typeparam>
    /// <returns>Repository pour l'entité.</returns>
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : Core.Models.Common.BaseEntity;
}
