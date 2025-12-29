namespace The6DiagXpert.Core.Models.Identity;

/// <summary>
/// Définit les différents rôles d'utilisateurs dans le système.
/// Les valeurs numériques déterminent la hiérarchie des permissions.
/// </summary>
public enum UserRole

{
    /// <summary>
    /// Administrateur système avec tous les droits
    /// Gestion complète de la plateforme
    /// </summary>
    SuperAdmin = 10,

    /// <summary>
    /// Administrateur système avec tous les droits
    /// Gestion complète de la plateforme
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Propriétaire / Gérant de société de diagnostic
    /// Accès complet aux données de sa société
    /// </summary>
    CompanyOwner = 2,

    /// <summary>
    /// Manager / Responsable d'agence
    /// Gestion des équipes et attribution des missions
    /// </summary>
    Manager = 3,

    /// <summary>
    /// Diagnostiqueur certifié
    /// Réalisation de tous types de diagnostics immobiliers
    /// </summary>
    Diagnostician = 4,

    /// <summary>
    /// Diagnostiqueur junior / en formation
    /// Accès limité, nécessite supervision
    /// </summary>
    JuniorDiagnostician = 5,

    /// <summary>
    /// Assistant administratif
    /// Gestion des plannings, saisie de données
    /// </summary>
    Assistant = 6,

    /// <summary>
    /// Comptable
    /// Accès aux données financières et facturation
    /// </summary>
    Accountant = 7,

    /// <summary>
    /// Client (lecture seule)
    /// Consultation de ses propres rapports de diagnostic
    /// </summary>
    Client = 8,

    /// <summary>
    /// Observateur en lecture seule.
    /// Peut consulter les données mais ne peut pas les modifier.
    /// </summary>
    Observer = 9
}