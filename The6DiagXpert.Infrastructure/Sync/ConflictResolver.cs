using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Infrastructure.Sync;

/// <summary>
/// Résolveur de conflits pour la synchronisation entre local et cloud.
/// </summary>
public class ConflictResolver
{
    /// <summary>
    /// Stratégie de résolution par défaut.
    /// </summary>
    public ConflictStrategy DefaultStrategy { get; set; } = ConflictStrategy.LastWriteWins;

    /// <summary>
    /// Résout un conflit entre deux versions d'une même entité.
    /// </summary>
    /// <typeparam name="TEntity">Type de l'entité.</typeparam>
    /// <param name="local">Version locale de l'entité.</param>
    /// <param name="remote">Version distante de l'entité.</param>
    /// <returns>Résolution à appliquer.</returns>
    public ConflictResolution Resolve<TEntity>(TEntity local, TEntity remote) where TEntity : BaseEntity
    {
        if (local == null) throw new ArgumentNullException(nameof(local));
        if (remote == null) throw new ArgumentNullException(nameof(remote));

        // Même version - pas de conflit
        if (local.Version == remote.Version && local.UpdatedAt == remote.UpdatedAt)
        {
            return ConflictResolution.NoConflict;
        }

        // Appliquer la stratégie configurée
        return DefaultStrategy switch
        {
            ConflictStrategy.LastWriteWins => ResolveLastWriteWins(local, remote),
            ConflictStrategy.HighestVersionWins => ResolveHighestVersionWins(local, remote),
            ConflictStrategy.LocalWins => ConflictResolution.UseLocal,
            ConflictStrategy.RemoteWins => ConflictResolution.UseRemote,
            ConflictStrategy.Manual => ConflictResolution.Conflict,
            _ => ResolveLastWriteWins(local, remote)
        };
    }

    /// <summary>
    /// Stratégie : La dernière modification l'emporte.
    /// </summary>
    private ConflictResolution ResolveLastWriteWins<TEntity>(TEntity local, TEntity remote) where TEntity : BaseEntity
    {
        // Comparer les timestamps de dernière modification
        if (local.UpdatedAt > remote.UpdatedAt)
        {
            return ConflictResolution.UseLocal;
        }
        else if (remote.UpdatedAt > local.UpdatedAt)
        {
            return ConflictResolution.UseRemote;
        }
        else
        {
            // Même timestamp - utiliser la version la plus haute
            return local.Version >= remote.Version
                ? ConflictResolution.UseLocal
                : ConflictResolution.UseRemote;
        }
    }

    /// <summary>
    /// Stratégie : La version la plus élevée l'emporte.
    /// </summary>
    private ConflictResolution ResolveHighestVersionWins<TEntity>(TEntity local, TEntity remote) where TEntity : BaseEntity
    {
        if (local.Version > remote.Version)
        {
            return ConflictResolution.UseLocal;
        }
        else if (remote.Version > local.Version)
        {
            return ConflictResolution.UseRemote;
        }
        else
        {
            // Même version - utiliser le timestamp le plus récent
            return local.UpdatedAt >= remote.UpdatedAt
                ? ConflictResolution.UseLocal
                : ConflictResolution.UseRemote;
        }
    }

    /// <summary>
    /// Vérifie si une entité a été modifiée depuis la dernière synchronisation.
    /// </summary>
    public bool HasChangedSinceLastSync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        if (entity.LastSyncAtUtc == null)
            return true;

        return entity.UpdatedAt > entity.LastSyncAtUtc;
    }

    /// <summary>
    /// Détermine l'action de synchronisation nécessaire.
    /// </summary>
    public SyncAction DetermineSyncAction<TEntity>(TEntity? local, TEntity? remote) where TEntity : BaseEntity
    {
        // Cas 1 : Existe seulement en local
        if (local != null && remote == null)
        {
            return SyncAction.UploadToRemote;
        }

        // Cas 2 : Existe seulement en remote
        if (local == null && remote != null)
        {
            return SyncAction.DownloadFromRemote;
        }

        // Cas 3 : Existe des deux côtés
        if (local != null && remote != null)
        {
            // Vérifier si modifié depuis dernière sync
            var localChanged = HasChangedSinceLastSync(local);
            var remoteChanged = remote.LastSyncAtUtc == null || remote.UpdatedAt > remote.LastSyncAtUtc;

            if (localChanged && !remoteChanged)
            {
                return SyncAction.UploadToRemote;
            }
            else if (!localChanged && remoteChanged)
            {
                return SyncAction.DownloadFromRemote;
            }
            else if (localChanged && remoteChanged)
            {
                return SyncAction.ResolveConflict;
            }
            else
            {
                return SyncAction.NoAction;
            }
        }

        return SyncAction.NoAction;
    }

    /// <summary>
    /// Fusionne deux entités en conflit en privilégiant certains champs.
    /// </summary>
    public TEntity Merge<TEntity>(
        TEntity local,
        TEntity remote,
        Func<string, ConflictResolution>? fieldResolver = null) where TEntity : BaseEntity
    {
        if (local == null) throw new ArgumentNullException(nameof(local));
        if (remote == null) throw new ArgumentNullException(nameof(remote));

        // Par défaut, créer une copie de l'entité locale
        var merged = (TEntity)Activator.CreateInstance(typeof(TEntity))!;
        merged.Id = local.Id;

        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.CanWrite && p.Name != nameof(BaseEntity.Id));

        foreach (var property in properties)
        {
            var localValue = property.GetValue(local);
            var remoteValue = property.GetValue(remote);

            // Si les valeurs sont identiques, pas de conflit
            if (Equals(localValue, remoteValue))
            {
                property.SetValue(merged, localValue);
                continue;
            }

            // Utiliser le résolveur de champ personnalisé si fourni
            var resolution = fieldResolver?.Invoke(property.Name) ?? ConflictResolution.UseLocal;

            var valueToUse = resolution switch
            {
                ConflictResolution.UseLocal => localValue,
                ConflictResolution.UseRemote => remoteValue,
                _ => localValue
            };

            property.SetValue(merged, valueToUse);
        }

        // Métadonnées de synchronisation
        merged.UpdatedAt = DateTime.UtcNow;
        merged.Version = Math.Max(local.Version, remote.Version) + 1;
        merged.LastSyncAtUtc = DateTime.UtcNow;

        return merged;
    }

    /// <summary>
    /// Obtient un rapport détaillé des différences entre deux entités.
    /// </summary>
    public ConflictReport GetConflictReport<TEntity>(TEntity local, TEntity remote) where TEntity : BaseEntity
    {
        var report = new ConflictReport
        {
            LocalVersion = local.Version,
            RemoteVersion = remote.Version,
            LocalUpdatedAt = local.UpdatedAt,
            RemoteUpdatedAt = remote.UpdatedAt
        };

        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.CanRead && p.Name != nameof(BaseEntity.Id));

        foreach (var property in properties)
        {
            var localValue = property.GetValue(local);
            var remoteValue = property.GetValue(remote);

            if (!Equals(localValue, remoteValue))
            {
                report.Differences.Add(new FieldDifference
                {
                    FieldName = property.Name,
                    LocalValue = localValue?.ToString() ?? "null",
                    RemoteValue = remoteValue?.ToString() ?? "null"
                });
            }
        }

        return report;
    }
}

/// <summary>
/// Stratégie de résolution de conflit.
/// </summary>
public enum ConflictStrategy
{
    /// <summary>La dernière modification l'emporte.</summary>
    LastWriteWins,

    /// <summary>La version la plus élevée l'emporte.</summary>
    HighestVersionWins,

    /// <summary>La version locale l'emporte toujours.</summary>
    LocalWins,

    /// <summary>La version distante l'emporte toujours.</summary>
    RemoteWins,

    /// <summary>Résolution manuelle requise.</summary>
    Manual
}

/// <summary>
/// Résolution d'un conflit.
/// </summary>
public enum ConflictResolution
{
    /// <summary>Pas de conflit.</summary>
    NoConflict,

    /// <summary>Utiliser la version locale.</summary>
    UseLocal,

    /// <summary>Utiliser la version distante.</summary>
    UseRemote,

    /// <summary>Conflit nécessitant intervention manuelle.</summary>
    Conflict
}

/// <summary>
/// Action de synchronisation à effectuer.
/// </summary>
public enum SyncAction
{
    /// <summary>Aucune action nécessaire.</summary>
    NoAction,

    /// <summary>Uploader vers le serveur distant.</summary>
    UploadToRemote,

    /// <summary>Télécharger depuis le serveur distant.</summary>
    DownloadFromRemote,

    /// <summary>Résoudre un conflit.</summary>
    ResolveConflict
}

/// <summary>
/// Rapport de conflit détaillé.
/// </summary>
public class ConflictReport
{
    public int LocalVersion { get; set; }
    public int RemoteVersion { get; set; }
    public DateTime LocalUpdatedAt { get; set; }
    public DateTime RemoteUpdatedAt { get; set; }
    public List<FieldDifference> Differences { get; set; } = new();

    public bool HasConflicts => Differences.Any();
    public int ConflictCount => Differences.Count;
}

/// <summary>
/// Différence sur un champ spécifique.
/// </summary>
public class FieldDifference
{
    public string FieldName { get; set; } = string.Empty;
    public string LocalValue { get; set; } = string.Empty;
    public string RemoteValue { get; set; } = string.Empty;
}
