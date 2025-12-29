using System;
using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Objet/symbole positionné sur un plan (prise, interrupteur, radiateur, etc.).
/// </summary>
public sealed class PlanObject : BaseEntity
{
    public Guid PlanId { get; set; }

    public Guid? LayerId { get; set; }

    public Guid? RoomId { get; set; }

    public PlanObjectType Type { get; set; } = PlanObjectType.Unknown;

    public ObjectStatus Status { get; set; } = ObjectStatus.Active;

    [MaxLength(120)]
    public string? Label { get; set; }

    /// <summary>
    /// Position en pixels.
    /// </summary>
    public Position Position { get; set; } = new Position(0, 0);

    /// <summary>
    /// Rotation (degrés).
    /// </summary>
    public double RotationDegrees { get; set; } = 0.0;

    /// <summary>
    /// Échelle visuelle (UI) (1.0 = taille normale).
    /// </summary>
    public double VisualScale { get; set; } = 1.0;

    /// <summary>
    /// Données libres (MVP) : ex: "16A", "IP44", "mur nord", etc.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    public void MoveTo(Position newPosition)
    {
        Position = newPosition;
        MarkAsModified();
    }
}
