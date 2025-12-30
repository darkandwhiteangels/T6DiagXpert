using Microsoft.EntityFrameworkCore.Storage;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Data.Repositories.Implementations;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.UnitOfWork;

/// <summary>
/// Implémentation du Unit of Work pour coordonner les opérations sur les repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    // Repositories
    private IMissionRepository? _missionRepository;
    private IClientRepository? _clientRepository;
    private IPropertyRepository? _propertyRepository;

    // Cache pour repositories génériques
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Repository pour les missions (lazy loading).
    /// </summary>
    public IMissionRepository Missions =>
        _missionRepository ??= new MissionRepository(_context);

    /// <summary>
    /// Repository pour les clients (lazy loading).
    /// </summary>
    public IClientRepository Clients =>
        _clientRepository ??= new ClientRepository(_context);

    /// <summary>
    /// Repository pour les propriétés (lazy loading).
    /// </summary>
    public IPropertyRepository Properties =>
        _propertyRepository ??= new PropertyRepository(_context);

    /// <summary>
    /// Obtient un repository générique avec cache.
    /// </summary>
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity);

        if (_repositories.TryGetValue(type, out var cachedRepository))
        {
            return (IGenericRepository<TEntity>)cachedRepository;
        }

        var repository = new GenericRepository<TEntity>(_context);
        _repositories[type] = repository;

        return repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log l'erreur si nécessaire
            throw new InvalidOperationException("Erreur lors de la sauvegarde des modifications.", ex);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Une transaction est déjà en cours.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Aucune transaction en cours à valider.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Aucune transaction en cours à annuler.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
