namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Définit le statut de conformité d'un élément diagnostiqué.
/// </summary>
public enum ConformityStatus
{
    /// <summary>
    /// Élément conforme aux normes et réglementations en vigueur.
    /// </summary>
    Compliant = 0,

    /// <summary>
    /// Élément non conforme nécessitant des corrections.
    /// </summary>
    NonCompliant = 1,

    /// <summary>
    /// Critère de conformité non applicable pour cet élément.
    /// </summary>
    NotApplicable = 2
}
