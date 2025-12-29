namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Définit le statut de synchronisation des données entre le client et le serveur.
/// </summary>
public enum SyncStatus
{
    /// <summary>
    /// Données synchronisées avec le serveur.
    /// </summary>
    Synced = 0,

    /// <summary>
    /// Synchronisation en attente.
    /// Les modifications locales n'ont pas encore été envoyées au serveur.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Échec de la synchronisation.
    /// Une erreur s'est produite lors de la tentative de synchronisation.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Conflit de synchronisation.
    /// Les données ont été modifiées à la fois localement et sur le serveur.
    /// </summary>
    Conflict = 3
}
