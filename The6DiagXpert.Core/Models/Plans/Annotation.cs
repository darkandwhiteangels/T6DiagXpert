using System;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Annotation libre posée sur le plan (texte, repère, note).
/// </summary>
public sealed class Annotation : BaseEntity
{
    public Guid PlanId { get; set; }

    public Guid? LayerId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public Position Position { get; set; } = new Position(0, 0);

    /// <summary>
    /// Couleur d'UI (hex).
    /// </summary>
    [MaxLength(12)]
    public string? ColorHex { get; set; }

    /// <summary>
    /// Taille de police (UI).
    /// </summary>
    public double FontSize { get; set; } = 12.0;

    public bool IsPinned { get; set; } = false;
}
