using System;
using System.Collections.Generic;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Identity;

/// <summary>
/// Représente une entreprise de diagnostic immobilier.
/// </summary>
public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string LegalName { get; set; } = string.Empty;

    public string SiretNumber { get; set; } = string.Empty;

    /// <summary>(Compat) Ancien champ : Siret.</summary>
    public string Siret
    {
        get => SiretNumber;
        set => SiretNumber = value;
    }

    public string? VatNumber { get; set; }
    public string? ApeCode { get; set; }
    public string? LegalForm { get; set; }
    public string? RcsNumber { get; set; }

    public decimal? ShareCapital { get; set; }

    public DateTime? FoundedDate { get; set; }

    // Assurance
    public string? InsurancePolicyNumber { get; set; }
    public string? InsuranceCompany { get; set; }
    public decimal? InsuranceCoverage { get; set; }

    /// <summary>
    /// Date d'expiration d'assurance (compat DTO) - stockée dans Settings.
    /// </summary>
    public DateTime? InsuranceExpiryDate
    {
        get => Settings?.InsuranceExpiryDate;
        set
        {
            Settings ??= new CompanySettings();
            Settings.InsuranceExpiryDate = value;
        }
    }

    public string SubscriptionPlan { get; set; } = "Free";
    public DateTime? SubscriptionExpiryDate { get; set; }

    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }

    public int MaxUsers { get; set; } = 5;
    public int MaxMissionsPerMonth { get; set; } = 50;

    public long MaxStorageInMb { get; set; } = 1024;

    public string? OwnerId { get; set; }

    public Address? HeadquartersAddress { get; set; }
    public Address? BillingAddress { get; set; }
    public Contact? ContactInfo { get; set; }

    /// <summary>(Compat) Ancien champ : Address.</summary>
    public Address? Address
    {
        get => HeadquartersAddress;
        set => HeadquartersAddress = value;
    }

    /// <summary>(Compat) Ancien champ : LogoUrl.</summary>
    public string? LogoUrl
    {
        get => Settings?.LogoUrl;
        set
        {
            Settings ??= new CompanySettings();
            Settings.LogoUrl = value;
        }
    }

    public CompanySettings Settings { get; set; } = new();

    public List<User> Users { get; set; } = new();

    public bool IsValidSiret()
    {
        if (string.IsNullOrWhiteSpace(SiretNumber))
            return false;

        var cleaned = SiretNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length == 14 && long.TryParse(cleaned, out _);
    }
}
