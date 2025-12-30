using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using The6DiagXpert.Core.Models.Identity;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Context;

/// <summary>
/// Contexte de base de données Entity Framework Core
/// Utilisé pour la persistance locale (SQLite) en mode offline
/// </summary>
public partial class AppDbContext : DbContext
{
    #region DbSets

    // ========================================
    // Identity
    // ========================================
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Certification> Certifications { get; set; } = null!;

    // ========================================
    // Missions & Clients
    // ========================================
    public DbSet<Mission> Missions { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<Diagnostic> Diagnostics { get; set; } = null!;

    #endregion

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "The6DiagXpert",
                "the6diagxpert.db"
            );

            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
#endif
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global: éviter les cascades involontaires
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            if (relationship.DeleteBehavior == DeleteBehavior.ClientSetNull)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed optionnel
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is The6DiagXpert.Core.Models.Common.BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (The6DiagXpert.Core.Models.Common.BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                // Id est déjà initialisé par BaseEntity (Guid.NewGuid())
                entity.CreatedAtUtc = DateTime.UtcNow;
                entity.UpdatedAtUtc = DateTime.UtcNow;
                entity.Version = 1;
                entity.IsDeleted = false;
                entity.DeletedAtUtc = null;
            }
            else
            {
                entity.MarkAsModified();
            }
        }
    }

    public static string GetDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "The6DiagXpert",
            "the6diagxpert.db"
        );
    }

    public static bool DatabaseExists() => File.Exists(GetDatabasePath());

    public static void DeleteDatabase()
    {
        var dbPath = GetDatabasePath();
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }
    }

    public static long GetDatabaseSizeMb()
    {
        var dbPath = GetDatabasePath();
        if (!File.Exists(dbPath))
            return 0;

        var fileInfo = new FileInfo(dbPath);
        return fileInfo.Length / (1024 * 1024);
    }

    public async Task EnsureCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }

    public async Task MigrateAsync()
    {
        await Database.MigrateAsync();
    }

    public async Task<bool> HasPendingMigrationsAsync()
    {
        var pendingMigrations = await Database.GetPendingMigrationsAsync();
        return pendingMigrations.Any();
    }

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync()
    {
        return await Database.GetAppliedMigrationsAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await Database.EnsureDeletedAsync();
        await Database.EnsureCreatedAsync();
    }

    public async Task<bool> CanConnectAsync()
    {
        try
        {
            return await Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
    {
        return await Database.BeginTransactionAsync();
    }
}
