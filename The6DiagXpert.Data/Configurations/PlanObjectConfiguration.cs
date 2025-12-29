using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Plans;

namespace The6DiagXpert.Data.Configurations;

public class PlanObjectConfiguration : IEntityTypeConfiguration<PlanObject>
{
    public void Configure(EntityTypeBuilder<PlanObject> builder)
    {
        builder.ToTable("PlanObjects");

        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.PlanId).HasDatabaseName("IX_PlanObjects_PlanId");
        builder.HasIndex(o => o.LayerId).HasDatabaseName("IX_PlanObjects_LayerId");
        builder.HasIndex(o => o.RoomId).HasDatabaseName("IX_PlanObjects_RoomId");
        builder.HasIndex(o => o.Type).HasDatabaseName("IX_PlanObjects_Type");
        builder.HasIndex(o => o.Status).HasDatabaseName("IX_PlanObjects_Status");
        builder.HasIndex(o => o.IsDeleted).HasDatabaseName("IX_PlanObjects_IsDeleted");

        builder.Property(o => o.PlanId).IsRequired();

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(80);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(o => o.Label)
            .HasMaxLength(120)
            .IsRequired(false);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(o => o.RotationDegrees).IsRequired();
        builder.Property(o => o.VisualScale).IsRequired();

        // Position (owned)
        builder.OwnsOne(o => o.Position, pos =>
        {
            pos.Property(p => p.X).HasColumnName("Pos_X").IsRequired();
            pos.Property(p => p.Y).HasColumnName("Pos_Y").IsRequired();
        });

        builder.Navigation(o => o.Position).IsRequired();

        // BaseEntity
        builder.Property(o => o.CreatedAtUtc).IsRequired();
        builder.Property(o => o.UpdatedAtUtc).IsRequired();
        builder.Property(o => o.DeletedAtUtc).IsRequired(false);

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(o => o.FirebaseId).HasMaxLength(100);
        builder.Property(o => o.LastSyncAtUtc).IsRequired(false);

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
