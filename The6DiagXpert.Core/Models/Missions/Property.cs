using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Common;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Core.Models.Missions;

/// <summary>
/// Représente un bien immobilier faisant l'objet de diagnostics.
/// </summary>
public class Property : BaseEntity
{
    [Required]
    public PropertyType PropertyType { get; set; }

    [Required]
    public PropertyUsage PropertyUsage { get; set; }

    [Required]
    public TransactionType TransactionType { get; set; }

    [MaxLength(100)]
    public string? Reference { get; set; }

    [Required]
    [MaxLength(2000)]
    public string AddressJson { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? GeoLocationJson { get; set; }

    [Range(0, 999999.99)]
    public decimal? LivingArea { get; set; }

    [Range(0, 9999999.99)]
    public decimal? LandArea { get; set; }

    [Range(0, 999)]
    public int? RoomCount { get; set; }

    [Range(0, 999)]
    public int? BedroomCount { get; set; }

    [Range(0, 99)]
    public int? BathroomCount { get; set; }

    [Range(1000, 9999)]
    public int? ConstructionYear { get; set; }

    [Range(1000, 9999)]
    public int? RenovationYear { get; set; }

    [Range(-5, 999)]
    public int? FloorNumber { get; set; }

    [Range(0, 999)]
    public int? TotalFloors { get; set; }

    public bool HasElevator { get; set; }
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }

    [MaxLength(1)]
    public string? EnergyClass { get; set; }

    [MaxLength(1)]
    public string? GhgClass { get; set; }

    [MaxLength(2000)]
    public string? AdditionalInfo { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// ID de l'entreprise propriétaire de ce bien.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    public virtual Company? Company { get; set; }

    /// <summary>
    /// ID du client propriétaire/occupant (lié à ce bien).
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();

    public bool IsCollective() =>
        PropertyType == PropertyType.Apartment ||
        PropertyType == PropertyType.Building;

    public bool IsNewConstruction(int currentYear)
    {
        if (!ConstructionYear.HasValue)
            return false;

        return (currentYear - ConstructionYear.Value) <= 5;
    }

    public string GetShortDescription()
    {
        var parts = new List<string>();

        if (RoomCount.HasValue)
            parts.Add($"{RoomCount}P");

        if (LivingArea.HasValue)
            parts.Add($"{LivingArea:F0}m²");

        return parts.Any() ? string.Join(" - ", parts) : PropertyType.ToString();
    }

    public int? GetPropertyAge(int currentYear)
    {
        if (!ConstructionYear.HasValue)
            return null;

        return currentYear - ConstructionYear.Value;
    }

    public bool RequiresDpe()
    {
        return PropertyUsage == PropertyUsage.Residential &&
               (TransactionType == TransactionType.Sale || TransactionType == TransactionType.Rental);
    }
}
