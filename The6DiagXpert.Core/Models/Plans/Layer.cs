using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Calque du plan (ex: Fond, Électricité, Gaz, Eau, Annotations, etc.).
/// </summary>
public sealed class Layer : BaseEntity
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = "Layer";

    /// <summary>
    /// Ordre d'affichage croissant (0 = dessous).
    /// </summary>
    public int Order { get; set; } = 0;

    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Opacité (0..1).
    /// </summary>
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// Couleur d'UI (hex: #RRGGBB ou #AARRGGBB).
    /// </summary>
    [MaxLength(12)]
    public string? ColorHex { get; set; }

    /// <summary>
    /// FK plan.
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Navigation.
    /// </summary>
    public ICollection<PlanObject> Objects { get; set; } = new List<PlanObject>();

    public ICollection<Annotation> Annotations { get; set; } = new List<Annotation>();
}
