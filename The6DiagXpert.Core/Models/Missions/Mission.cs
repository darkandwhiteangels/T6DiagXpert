using System.ComponentModel.DataAnnotations;
using System.Linq;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Core.Enums;
using System;
using The6DiagXpert.Core.Models.Identity;
using System.Collections.Generic;

namespace The6DiagXpert.Core.Models.Missions;

/// <summary>
/// Représente une mission de diagnostic immobilier.
/// Une mission regroupe tous les diagnostics à réaliser pour un bien donné.
/// </summary>
public class Mission : BaseEntity
{
    /// <summary>
    /// Numéro unique de la mission (généré automatiquement).
    /// Format: M-YYYYMMDD-XXXX
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string MissionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Statut actuel de la mission.
    /// </summary>
    [Required]
    public MissionStatus Status { get; set; } = MissionStatus.OnHold;

    /// <summary>
    /// Type de transaction immobilière associée.
    /// </summary>
    [Required]
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Date de création de la mission.
    /// </summary>
    [Required]
    public DateTime MissionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure prévues pour la réalisation des diagnostics.
    /// </summary>
    public DateTime? ScheduledDate { get; set; }

    /// <summary>
    /// Date et heure effectives de réalisation.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Date limite de validité de la mission.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Titre / Objet de la mission.
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Description détaillée de la mission.
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Notes internes sur la mission (non visible client).
    /// </summary>
    [MaxLength(2000)]
    public string? InternalNotes { get; set; }

    /// <summary>
    /// Montant total HT de la mission.
    /// </summary>
    [Range(0, 999999.99)]
    public decimal? AmountHT { get; set; }

    /// <summary>
    /// Montant de la TVA.
    /// </summary>
    [Range(0, 999999.99)]
    public decimal? VatAmount { get; set; }

    /// <summary>
    /// Montant total TTC de la mission.
    /// </summary>
    [Range(0, 999999.99)]
    public decimal? AmountTTC { get; set; }

    /// <summary>
    /// Taux de TVA appliqué (en %).
    /// </summary>
    [Range(0, 100)]
    public decimal VatRate { get; set; } = 20;

    /// <summary>
    /// Durée estimée de la mission en minutes.
    /// </summary>
    [Range(0, 9999)]
    public int? EstimatedDuration { get; set; }

    /// <summary>
    /// Durée réelle de la mission en minutes.
    /// </summary>
    [Range(0, 9999)]
    public int? ActualDuration { get; set; }

    /// <summary>
    /// ID du client pour qui la mission est réalisée.
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    /// <summary>
    /// Navigation : Client de la mission.
    /// </summary>
    public virtual Client? Client { get; set; }

    /// <summary>
    /// ID du bien immobilier diagnostiqué.
    /// </summary>
    [Required]
    public Guid PropertyId { get; set; }

    /// <summary>
    /// Navigation : Bien immobilier.
    /// </summary>
    public virtual Property? Property { get; set; }

    /// <summary>
    /// ID du diagnostiqueur assigné à la mission.
    /// </summary>
    public Guid? AssignedDiagnosticianId { get; set; }

    /// <summary>
    /// Navigation : Diagnostiqueur assigné.
    /// </summary>
    public virtual User? AssignedDiagnostician { get; set; }

    /// <summary>
    /// ID de l'entreprise propriétaire de la mission.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Navigation : Entreprise propriétaire.
    /// </summary>
    public virtual Company? Company { get; set; }

    /// <summary>
    /// Navigation : Diagnostics liés à la mission.
    /// </summary>
    public virtual ICollection<Diagnostic> Diagnostics { get; set; } = new List<Diagnostic>();


    /// <summary>
    /// Navigation : Documents liés à la mission.
    /// </summary>
    public virtual ICollection<MissionDocument> Documents { get; set; } = new List<MissionDocument>();

    /// <summary>
    /// Navigation : Photos de la mission.
    /// </summary>
    public virtual ICollection<MissionPhoto> Photos { get; set; } = new List<MissionPhoto>();

    /// <summary>
    /// Génère un numéro de mission unique.
    /// </summary>
    public static string GenerateMissionNumber(string prefix = "M")
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"{prefix}-{timestamp}-{random}";
    }

    /// <summary>
    /// Vérifie si la mission est en retard.
    /// </summary>
    public bool IsOverdue()
    {
        return ScheduledDate.HasValue &&
               ScheduledDate.Value < DateTime.UtcNow &&
               Status != MissionStatus.Completed &&
               Status != MissionStatus.Cancelled;
    }

    /// <summary>
    /// Vérifie si la mission est planifiée pour aujourd'hui.
    /// </summary>
    public bool IsScheduledToday()
    {
        return ScheduledDate.HasValue &&
               ScheduledDate.Value.Date == DateTime.Today;
    }

    /// <summary>
    /// Vérifie si la mission peut être modifiée.
    /// </summary>
    public bool CanBeModified()
    {
        return Status != MissionStatus.Completed &&
               Status != MissionStatus.Cancelled &&
               Status != MissionStatus.Archived;
    }

    /// <summary>
    /// Calcule les montants TTC et TVA à partir du HT.
    /// </summary>
    public void CalculateAmounts()
    {
        if (AmountHT.HasValue)
        {
            VatAmount = Math.Round(AmountHT.Value * VatRate / 100, 2);
            AmountTTC = AmountHT.Value + VatAmount;
        }
    }

    /// <summary>
    /// Marque la mission comme complétée.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = MissionStatus.Completed;
        CompletedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtient le nombre de jours avant la date planifiée.
    /// </summary>
    public int? GetDaysUntilScheduled()
    {
        if (!ScheduledDate.HasValue)
            return null;

        return (ScheduledDate.Value.Date - DateTime.Today).Days;
    }
}
