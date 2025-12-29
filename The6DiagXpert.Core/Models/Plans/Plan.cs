using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Plan rattaché à une mission (import image/PDF, calques, pièces, objets).
/// </summary>
public sealed class Plan : BaseEntity
{
    /// <summary>
    /// Mission associée.
    /// </summary>
    public Guid MissionId { get; set; }

    [Required]
    [MaxLength(160)]
    public string Name { get; set; } = "Plan";

    /// <summary>
    /// Numéro d'étage (0 = RDC, 1 = R+1, -1 = sous-sol).
    /// </summary>
    public int FloorIndex { get; set; } = 0;

    /// <summary>
    /// Largeur/hauteur du support importé (pixels).
    /// </summary>
    public double? CanvasWidth { get; set; }

    public double? CanvasHeight { get; set; }

    /// <summary>
    /// Source importée (chemin local ou identifiant stockage).
    /// </summary>
    [MaxLength(500)]
    public string? SourceUri { get; set; }

    /// <summary>
    /// Échelle de calibration (optionnelle tant que non calibré).
    /// </summary>
    public Scale? Scale { get; set; }

    public ICollection<Layer> Layers { get; set; } = new List<Layer>();

    public ICollection<Room> Rooms { get; set; } = new List<Room>();

    public ICollection<PlanObject> Objects { get; set; } = new List<PlanObject>();

    public ICollection<Annotation> Annotations { get; set; } = new List<Annotation>();

    public Layer CreateDefaultLayerIfMissing(string name = "Default", int order = 0)
    {
        foreach (var l in Layers)
        {
            if (string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase))
                return l;
        }

        var layer = new Layer
        {
            PlanId = Id,
            Name = name,
            Order = order,
            IsVisible = true,
            Opacity = 1.0
        };

        Layers.Add(layer);
        MarkAsModified();
        return layer;
    }
}
