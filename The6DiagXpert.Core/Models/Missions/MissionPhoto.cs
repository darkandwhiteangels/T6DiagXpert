using System;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Core.Models.Missions;

/// <summary>
/// Représente une photo prise lors d'une mission de diagnostic.
/// </summary>
public class MissionPhoto : BaseEntity
{
    /// <summary>
    /// Nom du fichier photo.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Titre/Légende de la photo.
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Description détaillée de la photo.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Chemin de stockage de la photo (URL Firebase Storage).
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// URL de la miniature (thumbnail).
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// Taille du fichier en octets.
    /// </summary>
    [Range(0, long.MaxValue)]
    public long FileSize { get; set; }

    /// <summary>
    /// Largeur de l'image en pixels.
    /// </summary>
    [Range(0, 99999)]
    public int? Width { get; set; }

    /// <summary>
    /// Hauteur de l'image en pixels.
    /// </summary>
    [Range(0, 99999)]
    public int? Height { get; set; }

    /// <summary>
    /// Ordre d'affichage de la photo dans la mission.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Catégorie/Zone de la photo (Extérieur, Salon, Cuisine, Salle de bain, etc.).
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Coordonnées GPS où la photo a été prise (JSON sérialisé).
    /// </summary>
    [MaxLength(200)]
    public string? GeoLocationJson { get; set; }

    /// <summary>
    /// Date et heure de prise de la photo.
    /// </summary>
    [Required]
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indique si la photo est visible par le client.
    /// </summary>
    public bool IsVisibleToClient { get; set; } = true;

    /// <summary>
    /// Indique si la photo est marquée comme favorite/importante.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// ID de l'utilisateur qui a pris la photo.
    /// </summary>
    public Guid? TakenByUserId { get; set; }

    /// <summary>
    /// Navigation : Utilisateur qui a pris la photo.
    /// </summary>
    public virtual User? TakenByUser { get; set; }

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
    /// Obtient les dimensions formatées de la photo.
    /// </summary>
    public string GetDimensions()
    {
        if (Width.HasValue && Height.HasValue)
            return $"{Width}x{Height}px";
        
        return "Dimensions inconnues";
    }

    /// <summary>
    /// Obtient la taille formatée du fichier.
    /// </summary>
    public string GetFormattedFileSize()
    {
        if (FileSize < 1024)
            return $"{FileSize} o";
        else if (FileSize < 1024 * 1024)
            return $"{FileSize / 1024.0:F1} Ko";
        else
            return $"{FileSize / (1024.0 * 1024.0):F1} Mo";
    }

    /// <summary>
    /// Obtient le ratio d'aspect de l'image.
    /// </summary>
    public string GetAspectRatio()
    {
        if (!Width.HasValue || !Height.HasValue || Height.Value == 0)
            return "N/A";

        var ratio = (double)Width.Value / Height.Value;
        
        // Ratios courants
        if (Math.Abs(ratio - 1.0) < 0.01) return "1:1 (Carré)";
        if (Math.Abs(ratio - 4.0/3.0) < 0.01) return "4:3";
        if (Math.Abs(ratio - 16.0/9.0) < 0.01) return "16:9";
        if (Math.Abs(ratio - 3.0/2.0) < 0.01) return "3:2";
        
        return $"{ratio:F2}:1";
    }
}
