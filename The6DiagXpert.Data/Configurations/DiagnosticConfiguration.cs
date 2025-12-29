using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Diagnostic.
/// </summary>
public class DiagnosticConfiguration : IEntityTypeConfiguration<Diagnostic>
{
    public void Configure(EntityTypeBuilder<Diagnostic> builder)
    {
        // Table
        builder.ToTable("Diagnostics");

        // Clé primaire
        builder.HasKey(d => d.Id);

        // Index
        builder.HasIndex(d => d.DiagnosticNumber)
            .IsUnique()
            .HasDatabaseName("IX_Diagnostics_DiagnosticNumber");

        builder.HasIndex(d => d.DiagnosticType)
            .HasDatabaseName("IX_Diagnostics_DiagnosticType");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_Diagnostics_Status");

        builder.HasIndex(d => d.DiagnosticDate)
            .HasDatabaseName("IX_Diagnostics_DiagnosticDate");

        builder.HasIndex(d => d.ExpiryDate)
            .HasDatabaseName("IX_Diagnostics_ExpiryDate");

        builder.HasIndex(d => d.MissionId)
            .HasDatabaseName("IX_Diagnostics_MissionId");

        builder.HasIndex(d => d.DiagnosticianId)
            .HasDatabaseName("IX_Diagnostics_DiagnosticianId");

        builder.HasIndex(d => d.IsDeleted)
            .HasDatabaseName("IX_Diagnostics_IsDeleted");

        builder.HasIndex(d => new { d.DiagnosticType, d.Status })
            .HasDatabaseName("IX_Diagnostics_Type_Status");

        builder.HasIndex(d => new { d.MissionId, d.DiagnosticType })
            .HasDatabaseName("IX_Diagnostics_MissionId_Type");

        // Propriétés requises
        builder.Property(d => d.DiagnosticNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Numéro unique du diagnostic");

        builder.Property(d => d.DiagnosticType)
            .IsRequired()
            .HasComment("Type de diagnostic");

        builder.Property(d => d.Status)
            .IsRequired()
            .HasComment("Statut du diagnostic");

        builder.Property(d => d.DiagnosticDate)
            .HasComment("Date de réalisation");

        builder.Property(d => d.ExpiryDate)
            .HasComment("Date d'expiration");

        builder.Property(d => d.Duration)
            .HasComment("Durée de réalisation (minutes)");

        builder.Property(d => d.Result)
            .HasMaxLength(100)
            .HasComment("Résultat principal");

        builder.Property(d => d.Observations)
            .HasMaxLength(4000)
            .HasComment("Observations du diagnostiqueur");

        builder.Property(d => d.Recommendations)
            .HasMaxLength(4000)
            .HasComment("Recommandations");

        builder.Property(d => d.TechnicalDataJson)
            .HasColumnType("TEXT")
            .HasComment("Données techniques (JSON)");

        builder.Property(d => d.ReportPath)
            .HasMaxLength(500)
            .HasComment("Chemin du rapport PDF");

        builder.Property(d => d.ReportGeneratedDate)
            .HasComment("Date de génération du rapport");

        // Montants
        builder.Property(d => d.AmountHT)
            .HasPrecision(10, 2)
            .HasComment("Montant HT");

        builder.Property(d => d.AmountTTC)
            .HasPrecision(10, 2)
            .HasComment("Montant TTC");

        builder.Property(d => d.VatRate)
            .HasPrecision(5, 2)
            .HasComment("Taux de TVA (%)");

        // Indicateurs
        builder.Property(d => d.HasAnomalies)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Présence d'anomalies");

        builder.Property(d => d.IsCompliant)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Conformité réglementaire");

        // Propriétés BaseEntity
        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasComment("Date de création");

        builder.Property(d => d.UpdatedAt)
            .HasComment("Date de dernière modification");

        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Suppression logique");

        builder.Property(d => d.DeletedAt)
            .HasComment("Date de suppression");

        builder.Property(d => d.Version)
            .IsRowVersion()
            .HasComment("Version pour la concurrence optimiste");

        // Relations
        builder.Property(d => d.MissionId)
            .IsRequired()
            .HasComment("ID de la mission parente");

        builder.HasOne(d => d.Mission)
            .WithMany(m => m.Diagnostics)
            .HasForeignKey(d => d.MissionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Diagnostics_Missions");

        builder.Property(d => d.DiagnosticianId)
            .HasComment("ID du diagnostiqueur");

        builder.HasOne(d => d.Diagnostician)
            .WithMany()
            .HasForeignKey(d => d.DiagnosticianId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Diagnostics_Users_Diagnostician");

        // Filtre de requête pour la suppression logique
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
