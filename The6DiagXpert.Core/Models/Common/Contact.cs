using System.ComponentModel.DataAnnotations;

namespace The6DiagXpert.Core.Models.Common;

/// <summary>
/// Représente les informations de contact d'une personne ou d'une organisation.
/// </summary>
public class Contact
{
    [EmailAddress(ErrorMessage = "Le format de l'email n'est pas valide.")]
    public string? Email { get; set; }

    /// <summary>(Compat) Ancien champ : Phone.</summary>
    public string? Phone
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }

    /// <summary>(Compat) Ancien champ : Mobile.</summary>
    public string? Mobile
    {
        get => SecondaryPhoneNumber;
        set => SecondaryPhoneNumber = value;
    }

    public string? PhoneNumber { get; set; }
    public string? SecondaryPhoneNumber { get; set; }

    [EmailAddress]
    public string? SecondaryEmail { get; set; }

    public string? Website { get; set; }
    public string? FaxNumber { get; set; }

    public string? PreferredContactMethod { get; set; }
    public string? AvailabilityHours { get; set; }
    public string? Notes { get; set; }
}
