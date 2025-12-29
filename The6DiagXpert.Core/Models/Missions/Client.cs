using System.ComponentModel.DataAnnotations;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Common;
using System;
using The6DiagXpert.Core.Models.Identity;
using System.Collections.Generic;

namespace The6DiagXpert.Core.Models.Missions;

/// <summary>
/// Représente un client (particulier ou professionnel) pour lequel des diagnostics sont réalisés.
/// </summary>
public class Client : BaseEntity
{
    /// <summary>
    /// Type de client (Particulier ou Professionnel).
    /// </summary>
    [Required]
    public ClientType ClientType { get; set; }

    /// <summary>
    /// Nom du client (nom de famille pour particulier, raison sociale pour professionnel).
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Prénom du client (uniquement pour particulier).
    /// </summary>
    [MaxLength(200)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Nom complet calculé du client.
    /// </summary>
    public string FullName => ClientType == ClientType.Individual && !string.IsNullOrEmpty(FirstName)
        ? $"{FirstName} {LastName}"
        : LastName;

    /// <summary>
    /// Numéro SIRET (uniquement pour professionnels).
    /// </summary>
    [MaxLength(14)]
    public string? SiretNumber { get; set; }

    /// <summary>
    /// Numéro de TVA intracommunautaire (uniquement pour professionnels).
    /// </summary>
    [MaxLength(20)]
    public string? VatNumber { get; set; }

    /// <summary>
    /// Email principal du client.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Numéro de téléphone principal.
    /// </summary>
    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Numéro de téléphone mobile.
    /// </summary>
    [Phone]
    [MaxLength(20)]
    public string? MobilePhone { get; set; }

    /// <summary>
    /// Adresse postale complète (JSON sérialisé).
    /// </summary>
    [MaxLength(2000)]
    public string? AddressJson { get; set; }

    /// <summary>
    /// Notes ou informations complémentaires sur le client.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Indique si le client est un client VIP / prioritaire.
    /// </summary>
    public bool IsVip { get; set; }

    /// <summary>
    /// Indique si le client accepte de recevoir des communications marketing.
    /// </summary>
    public bool AcceptsMarketing { get; set; }

    /// <summary>
    /// ID de l'entreprise propriétaire de ce client.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Navigation : Entreprise propriétaire.
    /// </summary>
    public virtual Company? Company { get; set; }

    /// <summary>
    /// Navigation : Missions associées à ce client.
    /// </summary>
    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();

    /// <summary>
    /// Navigation : Biens immobiliers associés à ce client.
    /// </summary>
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public PreferredContactMethod PreferredContactMethod { get; set; } = PreferredContactMethod.Email;

    /// <summary>
    /// Vérifie si le client est un professionnel.
    /// </summary>
    public bool IsProfessional() => ClientType == ClientType.Professional;

    /// <summary>
    /// Vérifie si le client est un particulier.
    /// </summary>
    public bool IsIndividual() => ClientType == ClientType.Individual;

    /// <summary>
    /// Obtient le numéro de téléphone prioritaire (mobile en priorité).
    /// </summary>
    public string? GetPrimaryPhone() => !string.IsNullOrWhiteSpace(MobilePhone) ? MobilePhone : Phone;

    /// <summary>
    /// Valide que les informations obligatoires pour un professionnel sont présentes.
    /// </summary>
    public bool ValidateProfessionalInfo()
    {
        if (ClientType != ClientType.Professional)
            return true;

        return !string.IsNullOrWhiteSpace(SiretNumber) || !string.IsNullOrWhiteSpace(VatNumber);
    }
}
