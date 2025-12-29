using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Company
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table
        builder.ToTable("Companies");

        // Clé primaire
        builder.HasKey(c => c.Id);

        // IMPORTANT : éviter le double mapping (Siret est un alias compat)
        builder.Ignore(c => c.Siret);

        // Index
        builder.HasIndex(c => c.SiretNumber).IsUnique();
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => c.CreatedAtUtc);
        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => c.IsVerified);

        // Index composites
        builder.HasIndex(c => new { c.IsVerified, c.IsDeleted });
        builder.HasIndex(c => new { c.SubscriptionPlan, c.SubscriptionExpiryDate });

        // Propriétés requises
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.LegalName)
            .IsRequired()
            .HasMaxLength(200);

        // Mapper le vrai champ et conserver le nom historique de colonne "Siret"
        builder.Property(c => c.SiretNumber)
            .HasColumnName("Siret")
            .IsRequired()
            .HasMaxLength(14);

        // Propriétés optionnelles
        builder.Property(c => c.VatNumber)
            .HasMaxLength(20);

        builder.Property(c => c.ApeCode)
            .HasMaxLength(10);

        builder.Property(c => c.LegalForm)
            .HasMaxLength(50);

        builder.Property(c => c.RcsNumber)
            .HasMaxLength(50);

        builder.Property(c => c.InsurancePolicyNumber)
            .HasMaxLength(50);

        builder.Property(c => c.InsuranceCompany)
            .HasMaxLength(200);

        builder.Property(c => c.SubscriptionPlan)
            .HasMaxLength(50)
            .HasDefaultValue("Free");

        // Valeurs par défaut
        builder.Property(c => c.IsVerified)
            .HasDefaultValue(false);

        builder.Property(c => c.MaxUsers)
            .HasDefaultValue(5);

        builder.Property(c => c.MaxMissionsPerMonth)
            .HasDefaultValue(50);

        builder.Property(c => c.MaxStorageInMb)
            .HasDefaultValue(1024); // 1 GB

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        // Propriétés décimales
        builder.Property(c => c.ShareCapital)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.InsuranceCoverage)
            .HasColumnType("decimal(18,2)");

        // Relations
        // IMPORTANT : OwnerId est string? tandis que User.Id est Guid -> FK impossible sans conversion.
        // On garde OwnerId comme champ/index, mais on ne crée pas de relation EF.

        // Propriétés complexes (owned entities) - Adresse siège social
        builder.OwnsOne(c => c.HeadquartersAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).HasColumnName("HQ_Street");
            address.Property(a => a.AdditionalInfo).HasMaxLength(200).HasColumnName("HQ_AdditionalInfo");
            address.Property(a => a.PostalCode).HasMaxLength(10).HasColumnName("HQ_PostalCode");
            address.Property(a => a.City).HasMaxLength(100).HasColumnName("HQ_City");
            address.Property(a => a.Department).HasMaxLength(100).HasColumnName("HQ_Department");
            address.Property(a => a.Region).HasMaxLength(100).HasColumnName("HQ_Region");
            address.Property(a => a.Country).HasMaxLength(100).HasColumnName("HQ_Country");
            address.Property(a => a.InseeCode).HasMaxLength(10).HasColumnName("HQ_InseeCode");

            // GeoLocation imbriquée
            address.OwnsOne(a => a.Location, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("HQ_Latitude");
                location.Property(l => l.Longitude).HasColumnName("HQ_Longitude");
                location.Property(l => l.Altitude).HasColumnName("HQ_Altitude");
                location.Property(l => l.Accuracy).HasColumnName("HQ_Accuracy");
                location.Property(l => l.CapturedAt).HasColumnName("HQ_CapturedAt");
                location.Property(l => l.Source).HasMaxLength(50).HasColumnName("HQ_Source");
            });
        });

        // Propriétés complexes (owned entities) - Adresse facturation
        builder.OwnsOne(c => c.BillingAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).HasColumnName("Billing_Street");
            address.Property(a => a.AdditionalInfo).HasMaxLength(200).HasColumnName("Billing_AdditionalInfo");
            address.Property(a => a.PostalCode).HasMaxLength(10).HasColumnName("Billing_PostalCode");
            address.Property(a => a.City).HasMaxLength(100).HasColumnName("Billing_City");
            address.Property(a => a.Department).HasMaxLength(100).HasColumnName("Billing_Department");
            address.Property(a => a.Region).HasMaxLength(100).HasColumnName("Billing_Region");
            address.Property(a => a.Country).HasMaxLength(100).HasColumnName("Billing_Country");
            address.Property(a => a.InseeCode).HasMaxLength(10).HasColumnName("Billing_InseeCode");

            // GeoLocation imbriquée
            address.OwnsOne(a => a.Location, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("Billing_Latitude");
                location.Property(l => l.Longitude).HasColumnName("Billing_Longitude");
                location.Property(l => l.Altitude).HasColumnName("Billing_Altitude");
                location.Property(l => l.Accuracy).HasColumnName("Billing_Accuracy");
                location.Property(l => l.CapturedAt).HasColumnName("Billing_CapturedAt");
                location.Property(l => l.Source).HasMaxLength(50).HasColumnName("Billing_Source");
            });
        });

        // Propriétés complexes (owned entities) - Contact
        builder.OwnsOne(c => c.ContactInfo, contact =>
        {
            contact.Property(ct => ct.PhoneNumber).HasMaxLength(20).HasColumnName("Contact_PhoneNumber");
            contact.Property(ct => ct.SecondaryPhoneNumber).HasMaxLength(20).HasColumnName("Contact_SecondaryPhoneNumber");
            contact.Property(ct => ct.Email).HasMaxLength(255).HasColumnName("Contact_Email");
            contact.Property(ct => ct.SecondaryEmail).HasMaxLength(255).HasColumnName("Contact_SecondaryEmail");
            contact.Property(ct => ct.Website).HasMaxLength(200).HasColumnName("Contact_Website");
            contact.Property(ct => ct.FaxNumber).HasMaxLength(20).HasColumnName("Contact_FaxNumber");
            contact.Property(ct => ct.PreferredContactMethod).HasMaxLength(50).HasColumnName("Contact_PreferredContactMethod");
            contact.Property(ct => ct.AvailabilityHours).HasMaxLength(200).HasColumnName("Contact_AvailabilityHours");
            contact.Property(ct => ct.Notes).HasMaxLength(1000).HasColumnName("Contact_Notes");
        });

        // Propriétés complexes (owned entities) - Settings
        builder.OwnsOne(c => c.Settings, settings =>
        {
            settings.Property(s => s.LogoUrl).HasMaxLength(500).HasColumnName("Settings_LogoUrl");
            settings.Property(s => s.PrimaryColor).HasMaxLength(20).HasColumnName("Settings_PrimaryColor");
            settings.Property(s => s.SecondaryColor).HasMaxLength(20).HasColumnName("Settings_SecondaryColor");
            settings.Property(s => s.Slogan).HasMaxLength(200).HasColumnName("Settings_Slogan");
            settings.Property(s => s.VatNumber).HasMaxLength(20).HasColumnName("Settings_VatNumber");
            settings.Property(s => s.SiretNumber).HasMaxLength(14).HasColumnName("Settings_SiretNumber");
            settings.Property(s => s.ApeCode).HasMaxLength(10).HasColumnName("Settings_ApeCode");
            settings.Property(s => s.ShareCapital).HasColumnType("decimal(18,2)").HasColumnName("Settings_ShareCapital");
            settings.Property(s => s.LegalForm).HasMaxLength(50).HasColumnName("Settings_LegalForm");
            settings.Property(s => s.RcsNumber).HasMaxLength(50).HasColumnName("Settings_RcsNumber");
            settings.Property(s => s.InsuranceNumber).HasMaxLength(50).HasColumnName("Settings_InsuranceNumber");
            settings.Property(s => s.InsuranceCompany).HasMaxLength(200).HasColumnName("Settings_InsuranceCompany");
            settings.Property(s => s.InsuranceExpiryDate).HasColumnName("Settings_InsuranceExpiryDate");
            settings.Property(s => s.DefaultPaymentTermDays).HasColumnName("Settings_DefaultPaymentTermDays");
            settings.Property(s => s.Currency).HasMaxLength(10).HasColumnName("Settings_Currency");
            settings.Property(s => s.DefaultVatRate).HasColumnType("decimal(5,2)").HasColumnName("Settings_DefaultVatRate");
            settings.Property(s => s.InvoicePrefix).HasMaxLength(20).HasColumnName("Settings_InvoicePrefix");
            settings.Property(s => s.QuotePrefix).HasMaxLength(20).HasColumnName("Settings_QuotePrefix");
            settings.Property(s => s.MissionPrefix).HasMaxLength(20).HasColumnName("Settings_MissionPrefix");
            settings.Property(s => s.DefaultMissionDuration).HasColumnName("Settings_DefaultMissionDuration");
            settings.Property(s => s.DefaultServiceRadius).HasColumnName("Settings_DefaultServiceRadius");
            settings.Property(s => s.TimeZone).HasMaxLength(50).HasColumnName("Settings_TimeZone");
            settings.Property(s => s.DefaultLanguage).HasMaxLength(10).HasColumnName("Settings_DefaultLanguage");
            settings.Property(s => s.DateFormat).HasMaxLength(20).HasColumnName("Settings_DateFormat");

            // WorkingDays stockés en JSON
            settings.Property(s => s.WorkingDays)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<DayOfWeek>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<DayOfWeek>()
                )
                .HasColumnName("Settings_WorkingDays");
        });

        // Query filters globaux
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
