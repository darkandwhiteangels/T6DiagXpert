using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.CompanyId).HasDatabaseName("IX_Properties_CompanyId");
        builder.HasIndex(p => p.ClientId).HasDatabaseName("IX_Properties_ClientId");
        builder.HasIndex(p => p.PropertyType).HasDatabaseName("IX_Properties_PropertyType");
        builder.HasIndex(p => p.PropertyUsage).HasDatabaseName("IX_Properties_PropertyUsage");
        builder.HasIndex(p => p.TransactionType).HasDatabaseName("IX_Properties_TransactionType");
        builder.HasIndex(p => p.Reference).HasDatabaseName("IX_Properties_Reference");
        builder.HasIndex(p => p.IsDeleted).HasDatabaseName("IX_Properties_IsDeleted");

        builder.Property(p => p.PropertyType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.PropertyUsage)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.TransactionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Reference)
            .HasMaxLength(100);

        builder.Property(p => p.AddressJson)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.GeoLocationJson)
            .HasMaxLength(500);

        builder.Property(p => p.LivingArea).HasPrecision(10, 2);
        builder.Property(p => p.LandArea).HasPrecision(10, 2);

        builder.Property(p => p.RoomCount).IsRequired(false);
        builder.Property(p => p.BedroomCount).IsRequired(false);
        builder.Property(p => p.BathroomCount).IsRequired(false);

        builder.Property(p => p.ConstructionYear).IsRequired(false);
        builder.Property(p => p.RenovationYear).IsRequired(false);
        builder.Property(p => p.FloorNumber).IsRequired(false);
        builder.Property(p => p.TotalFloors).IsRequired(false);

        builder.Property(p => p.EnergyClass).HasMaxLength(1);
        builder.Property(p => p.GhgClass).HasMaxLength(1);

        builder.Property(p => p.AdditionalInfo).HasMaxLength(2000);
        builder.Property(p => p.Notes).HasMaxLength(2000);

        // BaseEntity (TES NOMS)
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

        // Relations
        builder.HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Client)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Missions)
            .WithOne(m => m.Property)
            .HasForeignKey(m => m.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
