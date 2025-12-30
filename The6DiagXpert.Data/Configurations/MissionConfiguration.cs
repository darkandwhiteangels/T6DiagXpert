using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Configurations;

public class MissionConfiguration : IEntityTypeConfiguration<Mission>
{
    public void Configure(EntityTypeBuilder<Mission> builder)
    {
        builder.ToTable("Missions");

        builder.HasKey(m => m.Id);

        builder.HasIndex(m => m.MissionNumber)
            .IsUnique()
            .HasDatabaseName("IX_Missions_MissionNumber");

        builder.HasIndex(m => m.Status).HasDatabaseName("IX_Missions_Status");
        builder.HasIndex(m => m.MissionDate).HasDatabaseName("IX_Missions_MissionDate");
        builder.HasIndex(m => m.ScheduledDate).HasDatabaseName("IX_Missions_ScheduledDate");

        builder.HasIndex(m => m.CompanyId).HasDatabaseName("IX_Missions_CompanyId");
        builder.HasIndex(m => m.ClientId).HasDatabaseName("IX_Missions_ClientId");
        builder.HasIndex(m => m.PropertyId).HasDatabaseName("IX_Missions_PropertyId");
        builder.HasIndex(m => m.AssignedDiagnosticianId).HasDatabaseName("IX_Missions_AssignedDiagnosticianId");

        builder.HasIndex(m => m.IsDeleted).HasDatabaseName("IX_Missions_IsDeleted");

        builder.Property(m => m.MissionNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.TransactionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.MissionDate).IsRequired();

        builder.Property(m => m.ScheduledDate).IsRequired(false);
        builder.Property(m => m.CompletedDate).IsRequired(false);
        builder.Property(m => m.ExpiryDate).IsRequired(false);

        builder.Property(m => m.Title).HasMaxLength(200);
        builder.Property(m => m.Description).HasMaxLength(2000);
        builder.Property(m => m.InternalNotes).HasMaxLength(2000);

        builder.Property(m => m.AmountHT).HasPrecision(10, 2);
        builder.Property(m => m.VatAmount).HasPrecision(10, 2);
        builder.Property(m => m.AmountTTC).HasPrecision(10, 2);
        builder.Property(m => m.VatRate).HasPrecision(5, 2);

        builder.Property(m => m.EstimatedDuration).IsRequired(false);
        builder.Property(m => m.ActualDuration).IsRequired(false);

        // BaseEntity (TES NOMS)
        builder.Property(m => m.CreatedAtUtc).IsRequired();
        builder.Property(m => m.UpdatedAtUtc).IsRequired();
        builder.Property(m => m.DeletedAtUtc).IsRequired(false);

        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(m => m.FirebaseId).HasMaxLength(100);
        builder.Property(m => m.LastSyncAtUtc).IsRequired(false);

        // Relations
        builder.HasOne(m => m.Client)
            .WithMany(c => c.Missions)
            .HasForeignKey(m => m.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Property)
            .WithMany(p => p.Missions)
            .HasForeignKey(m => m.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Company)
            .WithMany()
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AssignedDiagnostician)
            .WithMany()
            .HasForeignKey(m => m.AssignedDiagnosticianId)
            .OnDelete(DeleteBehavior.Restrict);

        // Documents (MissionDocument a MissionId + Mission)
        builder.HasMany(m => m.Documents)
            .WithOne(d => d.Mission)
            .HasForeignKey(d => d.MissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Photos : je ne peux pas finaliser sans MissionPhoto.cs
        // builder.HasMany(m => m.Photos) ... (on le fait dès que tu colles MissionPhoto)

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
