using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Pièce définie sur le plan via un contour (polygone en pixels).
/// </summary>
public sealed class Room : BaseEntity
{
    public Guid PlanId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = "Room";

    public RoomType Type { get; set; } = RoomType.Unknown;

    /// <summary>
    /// Contour de la pièce (polygone) en coordonnées plan (pixels).
    /// </summary>
    public List<Position> Polygon { get; set; } = new();

    /// <summary>
    /// Surface calculée (m²) si une échelle est définie.
    /// </summary>
    public double? AreaSquareMeters { get; set; }

    public void SetPolygon(IEnumerable<Position> points)
    {
        Polygon = points is List<Position> list ? list : new List<Position>(points);
        MarkAsModified();
    }
}
