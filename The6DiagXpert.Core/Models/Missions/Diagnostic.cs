using System;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Core.Models.Missions;

/// <summary>
/// Représente un diagnostic individuel (DPE, Amiante, Plomb, etc.) dans une mission.
/// </summary>
public class Diagnostic : BaseEntity
{
    /// <summary>
    /// Numéro unique du diagnostic (généré automatiquement).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DiagnosticNumber { get; set; } = string.Empty;

    /// <summary>
    /// Type de diagnostic.
    /// </summary>
    [Required]
    public DiagnosticType DiagnosticType { get; set; }

    /// <summary>
    /// Statut actuel du diagnostic.
    /// </summary>
    [Required]
    public MissionStatus Status { get; set; }

    /// <summary>
    /// Date de réalisation du diagnostic.
    /// </summary>
    public DateTime? DiagnosticDate { get; set; }

    /// <summary>
    /// Date de validité / expiration du diagnostic.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Durée de réalisation en minutes.
    /// </summary>
    [Range(0, 9999)]
    public int? Duration { get; set; }

    /// <summary>
    /// Résultat principal du diagnostic (ex: "Positif", "Négatif", "Conforme", classe énergétique).
    /// </summary>
    [MaxLength(100)]
    public string? Result { get; set; }

    /// <summary>
    /// Notes et observations du diagnostiqueur.
    /// </summary>
    [MaxLength(4000)]
    public string? Observations { get; set; }

    /// <summary>
    /// Recommandations suite au diagnostic.
    /// </summary>
    [MaxLength(4000)]
    public string? Recommendations { get; set; }

    /// <summary>
    /// Données techniques du diagnostic (JSON sérialisé).
    /// </summary>
    public string? TechnicalDataJson { get; set; }

    /// <summary>
    /// URL ou chemin du rapport PDF généré.
    /// </summary>
    [MaxLength(500)]
    public string? ReportPath { get; set; }

    /// <summary>
    /// Date de génération du rapport.
    /// </summary>
    public DateTime? ReportGeneratedDate { get; set; }

    /// <summary>
    /// Montant HT du diagnostic.
    /// </summary>
    [Range(0, 99999.99)]
    public decimal? AmountHT { get; set; }

    /// <summary>
    /// Montant TTC du diagnostic.
    /// </summary>
    [Range(0, 99999.99)]
    public decimal? AmountTTC { get; set; }

    /// <summary>
    /// Taux de TVA appliqué (en %).
    /// </summary>
    [Range(0, 100)]
    public decimal? VatRate { get; set; }

    /// <summary>
    /// Indique si le diagnostic révèle des anomalies ou problèmes.
    /// </summary>
    public bool HasAnomalies { get; set; }

    /// <summary>
    /// Indique si le diagnostic est conforme à la réglementation.
    /// </summary>
    public bool IsCompliant { get; set; } = true;

    /// <summary>
    /// ID de la mission parente.
    /// </summary>
    [Required]
    public Guid MissionId { get; set; }

    /// <summary>
    /// Navigation : Mission parente.
    /// </summary>
    public virtual Mission? Mission { get; set; }

    /// <summary>
    /// ID du diagnostiqueur qui a réalisé le diagnostic.
    /// </summary>
    public Guid? DiagnosticianId { get; set; }

    /// <summary>
    /// Navigation : Diagnostiqueur.
    /// </summary>
    public virtual User? Diagnostician { get; set; }

    /// <summary>
    /// Génère un numéro de diagnostic unique.
    /// </summary>
    public static string GenerateDiagnosticNumber(DiagnosticType type, string missionNumber)
    {
        var typeCode = type.ToString().Substring(0, Math.Min(3, type.ToString().Length)).ToUpper();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(100, 999);
        return $"{typeCode}-{timestamp}-{random}";
    }

    /// <summary>
    /// Vérifie si le diagnostic est expiré.
    /// </summary>
    public bool IsExpired()
    {
        return ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.Today;
    }

    /// <summary>
    /// Vérifie si le diagnostic expire bientôt (dans les 30 jours).
    /// </summary>
    public bool IsExpiringSoon(int daysThreshold = 30)
    {
        if (!ExpiryDate.HasValue)
            return false;

        var daysUntilExpiry = (ExpiryDate.Value.Date - DateTime.Today).Days;
        return daysUntilExpiry > 0 && daysUntilExpiry <= daysThreshold;
    }

    /// <summary>
    /// Calcule la durée de validité restante en jours.
    /// </summary>
    public int? GetRemainingValidityDays()
    {
        if (!ExpiryDate.HasValue)
            return null;

        var days = (ExpiryDate.Value.Date - DateTime.Today).Days;
        return days > 0 ? days : 0;
    }

    /// <summary>
    /// Vérifie si le diagnostic peut être modifié.
    /// </summary>
    public bool CanBeModified()
    {
        return Status != MissionStatus.Completed &&
               Status != MissionStatus.Cancelled &&
               Status != MissionStatus.Archived &&
               !IsExpired();
    }

    /// <summary>
    /// Marque le diagnostic comme complété.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = MissionStatus.Completed;
        DiagnosticDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Calcule le montant TTC à partir du HT et du taux de TVA.
    /// </summary>
    public void CalculateAmountTTC()
    {
        if (AmountHT.HasValue && VatRate.HasValue)
        {
            AmountTTC = AmountHT.Value * (1 + VatRate.Value / 100);
        }
    }

    /// <summary>
    /// Obtient la durée de validité standard pour ce type de diagnostic (en années).
    /// </summary>
    public int GetStandardValidityPeriod()
    {
        return DiagnosticType switch
        {
            DiagnosticType.DPE => 10,
            DiagnosticType.Amiante => 999, // Illimité si négatif
            DiagnosticType.Plomb => 999, // Illimité si négatif (vente) ou 6 ans (location)
            DiagnosticType.Termites => 6 / 12, // 6 mois
            DiagnosticType.Gaz => 3,
            DiagnosticType.Electricite => 3,
            DiagnosticType.ERP => 6 / 12, // 6 mois
            DiagnosticType.LoiCarrez => 999, // Illimité
            DiagnosticType.LoiBoutin => 999, // Illimité
            DiagnosticType.AssainissementNC => 3,
            _ => 10
        };
    }
}
