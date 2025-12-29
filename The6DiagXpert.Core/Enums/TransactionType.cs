namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Types de transactions immobilières.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Vente du bien.
    /// </summary>
    Sale = 1,

    /// <summary>
    /// Location du bien.
    /// </summary>
    Rental = 2,

    /// <summary>
    /// Vente en viager.
    /// </summary>
    Viager = 3,

    /// <summary>
    /// Construction neuve.
    /// </summary>
    NewConstruction = 4,

    /// <summary>
    /// Vente en état futur d'achèvement (VEFA).
    /// </summary>
    OffPlan = 5,

    /// <summary>
    /// Autre type de transaction.
    /// </summary>
    Other = 99
}
