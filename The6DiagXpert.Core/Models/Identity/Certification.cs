using System;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Identity;

/// <summary>
/// Représente une certification professionnelle d'un utilisateur (diagnostiqueur).
/// </summary>
public class Certification : BaseEntity
{
    /// <summary>
    /// Type de certification (aligné sur tes enums existants).
    /// </summary>
    public DiagnosticType Type { get; set; }

    /// <summary>
    /// Numéro de certification.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Organisme certificateur.
    /// </summary>
    public string IssuingAuthority { get; set; } = string.Empty;

    /// <summary>
    /// Date de délivrance.
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Date d'expiration.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Chemin/URL du document (optionnel).
    /// </summary>
    public string? DocumentPath { get; set; }

    /// <summary>
    /// Notes internes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// FK utilisateur.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation vers l'utilisateur.
    /// </summary>
    public User? User { get; set; }

    public bool IsValid()
        => ExpirationDate >= DateTime.UtcNow && !IsDeleted;

    public bool IsExpiringSoon(int days = 90)
        => !IsDeleted && ExpirationDate <= DateTime.UtcNow.AddDays(days);

    public int GetDaysUntilExpiration()
        => (int)(ExpirationDate - DateTime.UtcNow).TotalDays;

    public override string ToString()
    {
        var status = IsValid() ? "Valide" : "Expirée";
        return $"{Type} - {Number} ({status})";
    }
}
