using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Core.Models.Missions;

/// <summary>
/// Représente un document lié à une mission (rapport, contrat, annexe, etc.).
/// </summary>
public class MissionDocument : BaseEntity
{
    /// <summary>
    /// Nom du fichier.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Nom d'affichage du document.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Type MIME du fichier.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Taille du fichier en octets.
    /// </summary>
    [Range(0, long.MaxValue)]
    public long FileSize { get; set; }

    /// <summary>
    /// Chemin de stockage du fichier (URL Firebase Storage ou chemin local).
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Type de document (Rapport, Contrat, Annexe, Photo, Autre).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = "Autre";

    /// <summary>
    /// Description du document.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Indique si le document est visible par le client.
    /// </summary>
    public bool IsVisibleToClient { get; set; } = true;

    /// <summary>
    /// Date de téléversement du document.
    /// </summary>
    [Required]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID de l'utilisateur qui a téléversé le document.
    /// </summary>
    public Guid? UploadedByUserId { get; set; }

    /// <summary>
    /// Navigation : Utilisateur qui a téléversé.
    /// </summary>
    public virtual User? UploadedByUser { get; set; }

    /// <summary>
    /// ID de la mission associée.
    /// </summary>
    [Required]
    public Guid MissionId { get; set; }

    /// <summary>
    /// Navigation : Mission associée.
    /// </summary>
    public virtual Mission? Mission { get; set; }

    /// <summary>
    /// Obtient l'extension du fichier.
    /// </summary>
    public string GetFileExtension()
    {
        return Path.GetExtension(FileName)?.ToLowerInvariant() ?? string.Empty;
    }

    /// <summary>
    /// Vérifie si le document est une image.
    /// </summary>
    public bool IsImage()
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        return imageExtensions.Contains(GetFileExtension());
    }

    /// <summary>
    /// Vérifie si le document est un PDF.
    /// </summary>
    public bool IsPdf()
    {
        return GetFileExtension() == ".pdf";
    }

    /// <summary>
    /// Obtient une taille formatée lisible (Ko, Mo, Go).
    /// </summary>
    public string GetFormattedFileSize()
    {
        if (FileSize < 1024)
            return $"{FileSize} o";
        else if (FileSize < 1024 * 1024)
            return $"{FileSize / 1024.0:F1} Ko";
        else if (FileSize < 1024 * 1024 * 1024)
            return $"{FileSize / (1024.0 * 1024.0):F1} Mo";
        else
            return $"{FileSize / (1024.0 * 1024.0 * 1024.0):F1} Go";
    }
}
