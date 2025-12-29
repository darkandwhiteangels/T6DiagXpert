using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Certification.
/// </summary>
public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
{
    public void Configure(EntityTypeBuilder<Certification> builder)
    {
        // Table
        builder.ToTable("Certifications");

        // Clé primaire
        builder.HasKey(c => c.Id);

        // Index
        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_Certifications_UserId");

        builder.HasIndex(c => c.Type)
            .HasDatabaseName("IX_Certifications_Type");

        builder.HasIndex(c => c.ExpirationDate)
            .HasDatabaseName("IX_Certifications_ExpirationDate");

        builder.HasIndex(c => c.IsDeleted)
            .HasDatabaseName("IX_Certifications_IsDeleted");

        // Propriétés
        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(c => c.Number)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.IssuingAuthority)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.IssueDate)
            .IsRequired();

        builder.Property(c => c.ExpirationDate)
            .IsRequired();

        builder.Property(c => c.DocumentPath)
            .HasMaxLength(500);

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        // BaseEntity properties
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();

        builder.Property(c => c.DeletedAtUtc)
            .IsRequired(false);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(c => c.FirebaseId)
            .HasMaxLength(100);

        builder.Property(c => c.LastSyncAtUtc)
            .IsRequired(false);

        // Relations
        builder.HasOne(c => c.User)
            .WithMany(u => u.Certifications)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter pour soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
