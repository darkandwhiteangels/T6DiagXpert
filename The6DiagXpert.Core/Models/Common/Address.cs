namespace The6DiagXpert.Core.Models.Common;

/// <summary>
/// Représente une adresse postale complète.
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;

    public string? AdditionalInfo { get; set; }

    /// <summary>(Compat) Ancien champ : Complement.</summary>
    public string? Complement
    {
        get => AdditionalInfo;
        set => AdditionalInfo = value;
    }

    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public string? Department { get; set; }
    public string? Region { get; set; }

    public string Country { get; set; } = "France";
    public string? InseeCode { get; set; }

    public GeoLocation? Location { get; set; }

    public override string ToString() => $"{Street}, {PostalCode} {City}, {Country}";
}
