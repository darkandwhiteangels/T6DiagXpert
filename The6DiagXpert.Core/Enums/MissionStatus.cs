namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Énumération des statuts possibles d'une mission de diagnostic
/// Représente le cycle de vie complet d'une mission
/// </summary>
public enum MissionStatus
{
    /// <summary>
    /// Mission créée mais pas encore assignée à un diagnostiqueur
    /// État initial lors de la création
    /// </summary>
    Created = 1,

    /// <summary>
    /// Mission assignée à un diagnostiqueur
    /// En attente d'intervention sur site
    /// </summary>
    Assigned = 2,

    /// <summary>
    /// Mission planifiée avec date/heure de rendez-vous
    /// Diagnostiqueur informé
    /// </summary>
    Scheduled = 3,

    /// <summary>
    /// Mission en cours de réalisation
    /// Diagnostiqueur sur site
    /// </summary>
    InProgress = 4,

    /// <summary>
    /// Intervention terrain terminée
    /// En attente de génération du rapport
    /// </summary>
    FieldworkCompleted = 5,

    /// <summary>
    /// Rapport en cours de rédaction/finalisation
    /// Vérifications et corrections
    /// </summary>
    ReportInProgress = 6,

    /// <summary>
    /// Rapport terminé en attente de validation
    /// Contrôle qualité avant envoi client
    /// </summary>
    PendingValidation = 7,

    /// <summary>
    /// Rapport validé et envoyé au client
    /// Mission techniquement terminée
    /// </summary>
    Completed = 8,

    /// <summary>
    /// Mission facturée au client
    /// En attente de paiement
    /// </summary>
    Invoiced = 9,

    /// <summary>
    /// Paiement reçu, mission totalement close
    /// Peut être archivée
    /// </summary>
    Paid = 10,

    /// <summary>
    /// Mission annulée (client, météo, etc.)
    /// Conservée pour historique
    /// </summary>
    Cancelled = 11,

    /// <summary>
    /// Mission archivée
    /// Conservation légale (10 ans pour diagnostics)
    /// </summary>
    Archived = 12,

    /// <summary>
    /// Mission suspendue temporairement
    /// En attente d'informations complémentaires
    /// </summary>
    OnHold = 13
}
