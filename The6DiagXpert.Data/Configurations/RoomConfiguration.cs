using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Plans;

namespace The6DiagXpert.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.PlanId).HasDatabaseName("IX_Rooms_PlanId");
        builder.HasIndex(r => r.Type).HasDatabaseName("IX_Rooms_Type");
        builder.HasIndex(r => r.IsDeleted).HasDatabaseName("IX_Rooms_IsDeleted");

        builder.Property(r => r.PlanId).IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(r => r.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(60);

        builder.Property(r => r.AreaSquareMeters)
            .IsRequired(false);

        // Polygon stocké en JSON (comme CompanyConfiguration.WorkingDays)
        builder.Property(r => r.Polygon)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<The6DiagXpert.Core.Models.Plans.Position>>(
                         v,
                         (System.Text.Json.JsonSerializerOptions?)null
                     ) ?? new List<The6DiagXpert.Core.Models.Plans.Position>()
            )
            .HasColumnName("PolygonJson");

        // BaseEntity
        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.UpdatedAtUtc).IsRequired();
        builder.Property(r => r.DeletedAtUtc).IsRequired(false);

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(r => r.FirebaseId).HasMaxLength(100);
        builder.Property(r => r.LastSyncAtUtc).IsRequired(false);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
