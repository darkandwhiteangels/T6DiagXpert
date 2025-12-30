using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Data.UnitOfWork;
using The6DiagXpert.Infrastructure.Firebase;

namespace The6DiagXpert.Infrastructure.Sync;

/// <summary>
/// Moteur de synchronisation bidirectionnelle entre SQLite local et Firestore cloud.
/// </summary>
public class SyncEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly FirestoreService _firestoreService;
    private readonly ConflictResolver _conflictResolver;
    private readonly bool _isEnabled;

    public SyncEngine(
        IUnitOfWork unitOfWork,
        FirestoreService firestoreService,
        ConflictResolver conflictResolver)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _firestoreService = firestoreService ?? throw new ArgumentNullException(nameof(firestoreService));
        _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));
        _isEnabled = _firestoreService.IsEnabled;
    }

    /// <summary>
    /// Vérifie si la synchronisation est activée.
    /// </summary>
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Synchronise une entité locale vers le cloud.
    /// </summary>
    public async Task<SyncResult> SyncToCloudAsync<TEntity>(
        TEntity entity,
        string collectionPath,
        CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        if (!_isEnabled)
            return SyncResult.Disabled();

        if (entity == null)
            return SyncResult.Error("Entité nulle");

        if (string.IsNullOrWhiteSpace(collectionPath))
            return SyncResult.Error("CollectionPath vide");

        try
        {
            // Vérifier si l'entité existe déjà sur le cloud
            var cloudEntity = !string.IsNullOrEmpty(entity.FirebaseId)
                ? await _firestoreService.GetDocumentAsync<TEntity>(
                    collectionPath,
                    entity.FirebaseId,
                    cancellationToken)
                : null;

            if (cloudEntity != null)
            {
                // Résolution de conflit
                var resolution = _conflictResolver.Resolve(entity, cloudEntity);

                if (resolution == ConflictResolution.UseLocal)
                {
                    // Mettre à jour le cloud avec la version locale
                    var success = await _firestoreService.UpdateDocumentAsync(
                        collectionPath,
                        entity.FirebaseId!,
                        entity,
                        cancellationToken);

                    if (success)
                    {
                        entity.LastSyncAtUtc = DateTime.UtcNow;

                        // Repo générique: UpdateAsync (pas Update)
                        await _unitOfWork.Repository<TEntity>().UpdateAsync(entity);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        return SyncResult.Success($"Entité {entity.Id} synchronisée vers le cloud");
                    }

                    return SyncResult.Error("Échec de la mise à jour du document Firestore");
                }

                if (resolution == ConflictResolution.UseRemote)
                {
                    // Ici tu peux choisir de pull la version cloud, mais pour l'instant on remonte un conflit clair.
                    return SyncResult.Conflict("Version cloud plus récente");
                }

                return SyncResult.Conflict("Conflit détecté");
            }

            // Créer sur le cloud
            var firebaseId = await _firestoreService.AddDocumentAsync(
                collectionPath,
                entity,
                cancellationToken);

            if (!string.IsNullOrEmpty(firebaseId))
            {
                entity.FirebaseId = firebaseId;
                entity.LastSyncAtUtc = DateTime.UtcNow;

                await _unitOfWork.Repository<TEntity>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return SyncResult.Success($"Entité {entity.Id} créée sur le cloud avec ID {firebaseId}");
            }

            return SyncResult.Error("Échec de la création du document Firestore");
        }
        catch (Exception ex)
        {
            return SyncResult.Error($"Erreur SyncToCloud: {ex.Message}");
        }
    }

    /// <summary>
    /// Synchronise depuis le cloud vers le local.
    /// </summary>
    public async Task<SyncResult> SyncFromCloudAsync<TEntity>(
        string collectionPath,
        CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        if (!_isEnabled)
            return SyncResult.Disabled();

        if (string.IsNullOrWhiteSpace(collectionPath))
            return SyncResult.Error("CollectionPath vide");

        try
        {
            // Récupérer toutes les entités du cloud
            var cloudEntities = await _firestoreService.GetCollectionAsync<TEntity>(
                collectionPath,
                cancellationToken);

            var syncCount = 0;
            var conflictCount = 0;

            foreach (var cloudEntity in cloudEntities)
            {
                // Repo local: GetByIdAsync(Guid id, bool includeDeleted=false)
                var localEntity = await _unitOfWork.Repository<TEntity>()
                    .GetByIdAsync(cloudEntity.Id);

                if (localEntity != null)
                {
                    var resolution = _conflictResolver.Resolve(localEntity, cloudEntity);

                    if (resolution == ConflictResolution.UseRemote)
                    {
                        CopyProperties(cloudEntity, localEntity);
                        localEntity.LastSyncAtUtc = DateTime.UtcNow;

                        await _unitOfWork.Repository<TEntity>().UpdateAsync(localEntity);
                        syncCount++;
                    }
                    else if (resolution == ConflictResolution.Conflict)
                    {
                        conflictCount++;
                    }
                }
                else
                {
                    cloudEntity.LastSyncAtUtc = DateTime.UtcNow;

                    // Repo local: AddAsync(entity) (pas de CancellationToken)
                    await _unitOfWork.Repository<TEntity>().AddAsync(cloudEntity);
                    syncCount++;
                }
            }

            if (syncCount > 0)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            var message = $"{syncCount} entité(s) synchronisée(s) depuis le cloud";
            if (conflictCount > 0)
                message += $", {conflictCount} conflit(s) détecté(s)";

            return SyncResult.Success(message);
        }
        catch (Exception ex)
        {
            return SyncResult.Error($"Erreur SyncFromCloud: {ex.Message}");
        }
    }

    /// <summary>
    /// Synchronisation bidirectionnelle complète.
    /// </summary>
    public async Task<SyncResult> FullSyncAsync<TEntity>(
        string collectionPath,
        CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        if (!_isEnabled)
            return SyncResult.Disabled();

        try
        {
            // Repo local: GetAllAsync(bool includeDeleted=false)
            var localEntities = await _unitOfWork.Repository<TEntity>().GetAllAsync();
            var toCloudCount = 0;

            foreach (var localEntity in localEntities)
            {
                var result = await SyncToCloudAsync(localEntity, collectionPath, cancellationToken);
                if (result.IsSuccess)
                    toCloudCount++;
            }

            var fromCloudResult = await SyncFromCloudAsync<TEntity>(collectionPath, cancellationToken);

            return SyncResult.Success($"Sync complet: {toCloudCount} vers cloud, {fromCloudResult.Message}");
        }
        catch (Exception ex)
        {
            return SyncResult.Error($"Erreur FullSync: {ex.Message}");
        }
    }

    /// <summary>
    /// Supprime une entité du cloud.
    /// </summary>
    public async Task<SyncResult> DeleteFromCloudAsync(
        string collectionPath,
        string firebaseId,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
            return SyncResult.Disabled();

        if (string.IsNullOrWhiteSpace(collectionPath))
            return SyncResult.Error("CollectionPath vide");

        if (string.IsNullOrWhiteSpace(firebaseId))
            return SyncResult.Error("FirebaseId vide");

        try
        {
            var success = await _firestoreService.DeleteDocumentAsync(collectionPath, firebaseId, cancellationToken);

            return success
                ? SyncResult.Success($"Entité {firebaseId} supprimée du cloud")
                : SyncResult.Error("Échec de la suppression du document Firestore");
        }
        catch (Exception ex)
        {
            return SyncResult.Error($"Erreur DeleteFromCloud: {ex.Message}");
        }
    }

    #region Méthodes privées

    private static void CopyProperties<TEntity>(TEntity source, TEntity destination) where TEntity : BaseEntity
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.CanWrite
                        && p.Name != nameof(BaseEntity.Id)
                        && p.Name != nameof(BaseEntity.CreatedAtUtc)); // on évite d’écraser la création locale

        foreach (var property in properties)
        {
            var value = property.GetValue(source);
            property.SetValue(destination, value);
        }
    }

    #endregion
}

/// <summary>
/// Résultat d'une opération de synchronisation.
/// NOTE: on s'aligne sur Core.Enums.SyncStatus (Synced/Pending/Failed/Conflict).
/// </summary>
public class SyncResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public SyncStatus Status { get; set; }

    public static SyncResult Success(string message) =>
        new() { IsSuccess = true, Message = message, Status = SyncStatus.Synced };

    public static SyncResult Error(string message) =>
        new() { IsSuccess = false, Message = message, Status = SyncStatus.Failed };

    public static SyncResult Conflict(string message) =>
        new() { IsSuccess = false, Message = message, Status = SyncStatus.Conflict };

    public static SyncResult Disabled() =>
        new() { IsSuccess = false, Message = "Synchronisation désactivée", Status = SyncStatus.Pending };
}
