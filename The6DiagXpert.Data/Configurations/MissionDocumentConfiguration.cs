using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Configurations;

public class MissionDocumentConfiguration : IEntityTypeConfiguration<MissionDocument>
{
    public void Configure(EntityTypeBuilder<MissionDocument> builder)
    {
        builder.ToTable("MissionDocuments");

        builder.HasKey(d => d.Id);

        builder.HasIndex(d => d.MissionId).HasDatabaseName("IX_MissionDocuments_MissionId");
        builder.HasIndex(d => d.DocumentType).HasDatabaseName("IX_MissionDocuments_DocumentType");
        builder.HasIndex(d => d.IsDeleted).HasDatabaseName("IX_MissionDocuments_IsDeleted");

        builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(d => d.MimeType).IsRequired().HasMaxLength(100);
        builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
        builder.Property(d => d.DocumentType).IsRequired().HasMaxLength(50);

        builder.Property(d => d.Description).HasMaxLength(500);

        builder.Property(d => d.UploadedAt).IsRequired();

        // BaseEntity
        builder.Property(d => d.CreatedAtUtc).IsRequired();
        builder.Property(d => d.UpdatedAtUtc).IsRequired();
        builder.Property(d => d.DeletedAtUtc).IsRequired(false);

        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(d => d.FirebaseId).HasMaxLength(100);
        builder.Property(d => d.LastSyncAtUtc).IsRequired(false);

        // Relations
        builder.HasOne(d => d.Mission)
            .WithMany(m => m.Documents)
            .HasForeignKey(d => d.MissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.UploadedByUser)
            .WithMany()
            .HasForeignKey(d => d.UploadedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
