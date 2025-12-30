using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Identity;

namespace The6DiagXpert.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Index
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.FirebaseUid).IsUnique();
        builder.HasIndex(u => u.CompanyId);
        builder.HasIndex(u => u.CreatedAtUtc);
        builder.HasIndex(u => u.IsDeleted);

        builder.HasIndex(u => new { u.CompanyId, u.IsDeleted });
        builder.HasIndex(u => new { u.Email, u.IsDeleted });

        // Champs requis
        builder.Property(u => u.FirebaseUid)
            .HasMaxLength(128);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.DisplayName)
            .HasMaxLength(100);

        builder.Property(u => u.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.ProfessionalCardNumber)
            .HasMaxLength(50);

        builder.Property(u => u.PreferredLanguage)
            .HasMaxLength(10)
            .HasDefaultValue("fr-FR");

        builder.Property(u => u.TimeZone)
            .HasMaxLength(50)
            .HasDefaultValue("Europe/Paris");

        builder.Property(u => u.Theme)
            .HasMaxLength(20)
            .HasDefaultValue("Light");

        // Valeurs par défaut
        builder.Property(u => u.EmailVerified).HasDefaultValue(false);
        builder.Property(u => u.PhoneVerified).HasDefaultValue(false);
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.IsLocked).HasDefaultValue(false);
        builder.Property(u => u.LoginCount).HasDefaultValue(0);
        builder.Property(u => u.NotificationsEnabled).HasDefaultValue(true);
        builder.Property(u => u.AcceptedTerms).HasDefaultValue(false);
        builder.Property(u => u.GdprConsent).HasDefaultValue(false);

        // Enum
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        // BaseEntity (TES PROPRIÉTÉS)
        builder.Property(u => u.CreatedAtUtc).IsRequired();
        builder.Property(u => u.UpdatedAtUtc).IsRequired();
        builder.Property(u => u.DeletedAtUtc).IsRequired(false);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.Version)
            .IsRequired()
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        builder.Property(u => u.FirebaseId)
            .HasMaxLength(100);

        builder.Property(u => u.LastSyncAtUtc)
            .IsRequired(false);

        // Relations
        builder.HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Certifications)
            .WithOne()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
