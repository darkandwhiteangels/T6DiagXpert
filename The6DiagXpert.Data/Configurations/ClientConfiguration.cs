using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.CompanyId).HasDatabaseName("IX_Clients_CompanyId");
        builder.HasIndex(c => c.Email).HasDatabaseName("IX_Clients_Email");
        builder.HasIndex(c => c.SiretNumber).HasDatabaseName("IX_Clients_Siret");
        builder.HasIndex(c => c.ClientType).HasDatabaseName("IX_Clients_ClientType");
        builder.HasIndex(c => c.IsDeleted).HasDatabaseName("IX_Clients_IsDeleted");
        builder.HasIndex(c => new { c.LastName, c.FirstName }).HasDatabaseName("IX_Clients_LastName_FirstName");

        builder.Property(c => c.ClientType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.FirstName)
            .IsRequired(false)
            .HasMaxLength(200);

        // Propriété calculée => pas en DB
        builder.Ignore(c => c.FullName);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.MobilePhone).HasMaxLength(20);

        builder.Property(c => c.SiretNumber).HasMaxLength(14);
        builder.Property(c => c.VatNumber).HasMaxLength(20);

        builder.Property(c => c.AddressJson).HasMaxLength(2000);
        builder.Property(c => c.Notes).HasMaxLength(2000);

        builder.Property(c => c.PreferredContactMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // BaseEntity
        builder.Property(c => c.CreatedAtUtc).IsRequired();
        builder.Property(c => c.UpdatedAtUtc).IsRequired();
        builder.Property(c => c.DeletedAtUtc).IsRequired(false);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(c => c.FirebaseId).HasMaxLength(100);
        builder.Property(c => c.LastSyncAtUtc).IsRequired(false);

        // Relations
        builder.HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Missions)
            .WithOne(m => m.Client)
            .HasForeignKey(m => m.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Properties)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
