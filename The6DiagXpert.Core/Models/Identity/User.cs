using System;
using System.Collections.Generic;
using System.Linq;
using The6DiagXpert.Core.Models.Common;

namespace The6DiagXpert.Core.Models.Identity;

/// <summary>
/// Représente un utilisateur du système The6DiagXpert.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// UID Firebase Auth (si utilisé)
    /// </summary>
    public string? FirebaseUid { get; set; }

    /// <summary>
    /// Adresse email de l'utilisateur (utilisée pour l'authentification).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Email vérifié (si Firebase / provider le permet)
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Prénom de l'utilisateur.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nom de famille de l'utilisateur.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nom d'affichage (prénom + nom, ou personnalisé)
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// URL de la photo de profil (optionnelle).
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Numéro de téléphone (optionnel).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Téléphone vérifié (si provider le permet)
    /// </summary>
    public bool PhoneVerified { get; set; }

    /// <summary>
    /// Date de naissance (optionnelle)
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Identifiant de l'entreprise à laquelle appartient l'utilisateur.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Rôle de l'utilisateur dans le système.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Observer;

    /// <summary>
    /// Liste des certifications professionnelles de l'utilisateur.
    /// </summary>
    public List<Certification> Certifications { get; set; } = new();

    /// <summary>
    /// Numéro de carte professionnelle (optionnel)
    /// </summary>
    public string? ProfessionalCardNumber { get; set; }

    /// <summary>
    /// Expiration de la carte professionnelle (optionnel)
    /// </summary>
    public DateTime? ProfessionalCardExpiryDate { get; set; }

    /// <summary>
    /// Langue préférée (ex: fr-FR)
    /// </summary>
    public string PreferredLanguage { get; set; } = "fr-FR";

    /// <summary>
    /// Fuseau horaire (ex: Europe/Paris)
    /// </summary>
    public string TimeZone { get; set; } = "Europe/Paris";

    /// <summary>
    /// Thème UI (ex: Light/Dark)
    /// </summary>
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// Indique si le compte utilisateur est actif.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indique si le compte est verrouillé.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Acceptation des CGU
    /// </summary>
    public bool AcceptedTerms { get; set; }

    /// <summary>
    /// Consentement RGPD
    /// </summary>
    public bool GdprConsent { get; set; }

    /// <summary>
    /// Notifications activées
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Date et heure de la dernière connexion (optionnelle).
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Nombre total de connexions
    /// </summary>
    public int LoginCount { get; set; }

    /// <summary>
    /// Entreprise à laquelle appartient l'utilisateur (navigation).
    /// </summary>
    public Company? Company { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}".Trim();

    public string FullName => $"{FirstName} {LastName}".Trim();


    public string GetInitials()
    {
        var firstInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString() : "";
        var lastInitial = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString() : "";
        return $"{firstInitial}{lastInitial}".ToUpper();
    }

    public bool HasValidCertifications() => Certifications.Any(c => c.IsValid());

    public List<Certification> GetExpiringSoonCertifications() =>
        Certifications.Where(c => c.IsExpiringSoon()).ToList();

    public bool IsAdministrator() => Role == UserRole.Admin || Role == UserRole.SuperAdmin;

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        LoginCount++;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
