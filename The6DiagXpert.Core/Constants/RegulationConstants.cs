namespace The6DiagXpert.Core.Constants;

/// <summary>
/// Constantes réglementaires pour les diagnostics immobiliers.
/// Basées sur la réglementation française en vigueur.
/// </summary>
public static class RegulationConstants
{
    /// <summary>
    /// Durée de validité d'un DPE (Diagnostic de Performance Énergétique) en années.
    /// </summary>
    public const int DPE_VALIDITY_YEARS = 10;

    /// <summary>
    /// Durée de validité d'un diagnostic Électricité en années.
    /// </summary>
    public const int ELECTRICITY_VALIDITY_YEARS = 3;

    /// <summary>
    /// Durée de validité d'un diagnostic Gaz en années.
    /// </summary>
    public const int GAS_VALIDITY_YEARS = 3;

    /// <summary>
    /// Durée de validité d'un diagnostic Amiante en années (si absence d'amiante).
    /// </summary>
    public const int ASBESTOS_VALIDITY_YEARS_IF_NEGATIVE = 0; // Illimité si négatif

    /// <summary>
    /// Durée de validité d'un CREP (Constat de Risque d'Exposition au Plomb) en années pour une vente.
    /// </summary>
    public const int LEAD_VALIDITY_YEARS_SALE = 1;

    /// <summary>
    /// Durée de validité d'un CREP pour une location en années (si absence de plomb).
    /// </summary>
    public const int LEAD_VALIDITY_YEARS_RENTAL = 6;

    /// <summary>
    /// Durée de validité d'un diagnostic Termites en mois.
    /// </summary>
    public const int TERMITES_VALIDITY_MONTHS = 6;

    /// <summary>
    /// Durée de validité d'un ERP (État des Risques et Pollutions) en mois.
    /// </summary>
    public const int ERP_VALIDITY_MONTHS = 6;

    /// <summary>
    /// Année de construction minimum pour obligation de diagnostic Amiante.
    /// Immeubles construits avant le 1er juillet 1997.
    /// </summary>
    public const int ASBESTOS_CONSTRUCTION_YEAR_THRESHOLD = 1997;

    /// <summary>
    /// Année de construction minimum pour obligation de diagnostic Plomb.
    /// Immeubles construits avant le 1er janvier 1949.
    /// </summary>
    public const int LEAD_CONSTRUCTION_YEAR_THRESHOLD = 1949;

    /// <summary>
    /// Surface habitable minimum en m² nécessitant un DPE.
    /// </summary>
    public const double DPE_MIN_SURFACE_M2 = 50.0;

    /// <summary>
    /// Seuil de concentration de plomb en mg/cm² considéré comme seuil de risque.
    /// </summary>
    public const double LEAD_RISK_THRESHOLD_MG_CM2 = 1.0;

    /// <summary>
    /// Classes énergétiques DPE (A à G).
    /// </summary>
    public static readonly string[] DPE_ENERGY_CLASSES = new[]
    {
        "A", "B", "C", "D", "E", "F", "G"
    };

    /// <summary>
    /// Classes GES (Gaz à Effet de Serre) pour le DPE (A à G).
    /// </summary>
    public static readonly string[] DPE_GES_CLASSES = new[]
    {
        "A", "B", "C", "D", "E", "F", "G"
    };

    /// <summary>
    /// Types de diagnostics obligatoires pour une vente.
    /// </summary>
    public static readonly string[] DIAGNOSTICS_REQUIRED_FOR_SALE = new[]
    {
        "DPE",
        "Amiante",
        "Plomb",
        "Termites",
        "ERP",
        "Électricité",
        "Gaz",
        "Assainissement"
    };

    /// <summary>
    /// Types de diagnostics obligatoires pour une location.
    /// </summary>
    public static readonly string[] DIAGNOSTICS_REQUIRED_FOR_RENTAL = new[]
    {
        "DPE",
        "Plomb",
        "ERP",
        "Électricité",
        "Gaz"
    };

    /// <summary>
    /// Délai légal de conservation des documents en années.
    /// </summary>
    public const int LEGAL_DOCUMENT_RETENTION_YEARS = 10;
}
