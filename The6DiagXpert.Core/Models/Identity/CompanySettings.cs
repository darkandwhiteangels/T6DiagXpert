using System;
using System.Collections.Generic;

namespace The6DiagXpert.Core.Models.Identity;

/// <summary>
/// Représente les paramètres et préférences d'une entreprise.
/// </summary>
public class CompanySettings
{
    // Branding
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? Slogan { get; set; }

    // Identifiants légaux
    public string? VatNumber { get; set; }
    public string? SiretNumber { get; set; }
    public string? ApeCode { get; set; }
    public decimal? ShareCapital { get; set; }
    public string? LegalForm { get; set; }
    public string? RcsNumber { get; set; }

    // Assurance
    public string? InsuranceNumber { get; set; }
    public string? InsuranceCompany { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }

    // Facturation
    public int? DefaultPaymentTermDays { get; set; }
    public string? Currency { get; set; }
    public decimal? DefaultVatRate { get; set; }
    public string? InvoicePrefix { get; set; }
    public string? QuotePrefix { get; set; }
    public string? MissionPrefix { get; set; }

    // Opérations
    public int? DefaultMissionDuration { get; set; } // minutes
    public int? DefaultServiceRadius { get; set; }   // km

    // Localisation/format
    public string? TimeZone { get; set; }
    public string? DefaultLanguage { get; set; }
    public string? DateFormat { get; set; }

    // Planning
    public List<DayOfWeek> WorkingDays { get; set; } = new()
    {
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
    };

    public TimeSpan WorkDayStartTime { get; set; } = new(8, 0, 0);
    public TimeSpan WorkDayEndTime { get; set; } = new(18, 0, 0);

    // Champs déjà existants (on garde)
    public string? DefaultReportTemplate { get; set; }
    public string? EmailSignature { get; set; }
    public bool AutoSyncEnabled { get; set; } = true;
    public int SyncFrequencyMinutes { get; set; } = 30;

    public CompanySettings() { }

    public CompanySettings(
        string defaultReportTemplate,
        string emailSignature,
        string invoicePrefix,
        bool autoSyncEnabled,
        int syncFrequencyMinutes)
    {
        DefaultReportTemplate = defaultReportTemplate;
        EmailSignature = emailSignature;
        InvoicePrefix = invoicePrefix;
        AutoSyncEnabled = autoSyncEnabled;
        SyncFrequencyMinutes = syncFrequencyMinutes;
    }
}
