namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Statut logique d'un objet dans le plan (utile pour soft-delete / masquage / brouillon).
/// </summary>
public enum ObjectStatus
{
    Active = 0,
    Hidden = 1,
    Deleted = 2,
    Draft = 3
}
