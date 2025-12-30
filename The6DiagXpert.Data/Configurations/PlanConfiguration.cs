using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Plans;

namespace The6DiagXpert.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.MissionId).HasDatabaseName("IX_Plans_MissionId");
        builder.HasIndex(p => p.FloorIndex).HasDatabaseName("IX_Plans_FloorIndex");
        builder.HasIndex(p => p.IsDeleted).HasDatabaseName("IX_Plans_IsDeleted");

        builder.Property(p => p.MissionId).IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(p => p.FloorIndex)
            .IsRequired();

        builder.Property(p => p.CanvasWidth).IsRequired(false);
        builder.Property(p => p.CanvasHeight).IsRequired(false);

        builder.Property(p => p.SourceUri)
            .HasMaxLength(500)
            .IsRequired(false);

        // Scale (owned - optionnelle)
        builder.OwnsOne(p => p.Scale, scale =>
        {
            scale.Property(s => s.PixelLength)
                .HasColumnName("Scale_PixelLength")
                .IsRequired();

            scale.Property(s => s.RealLengthMeters)
                .HasColumnName("Scale_RealLengthMeters")
                .IsRequired();

            scale.OwnsOne(s => s.PixelPointA, pos =>
            {
                pos.Property(x => x.X).HasColumnName("Scale_PointA_X");
                pos.Property(x => x.Y).HasColumnName("Scale_PointA_Y");
            });

            scale.OwnsOne(s => s.PixelPointB, pos =>
            {
                pos.Property(x => x.X).HasColumnName("Scale_PointB_X");
                pos.Property(x => x.Y).HasColumnName("Scale_PointB_Y");
            });
        });

        builder.Navigation(p => p.Scale).IsRequired(false);

        // Relations (scope Phase 16)
        builder.HasMany(p => p.Rooms)
            .WithOne()
            .HasForeignKey(r => r.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Objects)
            .WithOne()
            .HasForeignKey(o => o.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Hors scope Phase 16 (évite tables non prévues)
        builder.Ignore(p => p.Layers);
        builder.Ignore(p => p.Annotations);

        // BaseEntity
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.Property(p => p.DeletedAtUtc).IsRequired(false);

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(p => p.FirebaseId).HasMaxLength(100);
        builder.Property(p => p.LastSyncAtUtc).IsRequired(false);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
