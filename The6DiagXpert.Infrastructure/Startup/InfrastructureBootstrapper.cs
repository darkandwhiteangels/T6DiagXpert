using System;
using The6DiagXpert.Infrastructure.Firebase;
using The6DiagXpert.Infrastructure.Logging;

namespace The6DiagXpert.Infrastructure.Startup;

/// <summary>
/// Permet d'instancier l'Infrastructure sans DI (avant Phase 7).
/// - Logger fichier
/// - Accès aux services Firebase existants
/// </summary>
public sealed class InfrastructureBootstrapper
{
    public FirebaseConfig Config { get; }
    public FileLogger Logger { get; }

    public FirebaseAuthService Auth { get; }
    public FirestoreService Firestore { get; }
    public FirebaseStorageService Storage { get; }

    public InfrastructureBootstrapper(
        FirebaseConfig config,
        string? logsDirectory = null,
        bool writeJsonLines = false)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));

        // Logs local (par défaut: ./Logs à côté de l'exe)
        Logger = new FileLogger(
            logDirectory: string.IsNullOrWhiteSpace(logsDirectory) ? string.Empty : logsDirectory,
            fileName: "the6diagxpert.log",
            writeJsonLines: writeJsonLines);

        // ✅ Conformes à tes constructeurs
        Auth = new FirebaseAuthService(Config);

        // Ici on utilise les infos de config si elles existent, sinon null (ton service gère)
        Firestore = new FirestoreService(GetProjectIdOrNull(Config));
        Storage = new FirebaseStorageService(GetBucketNameOrNull(Config));
    }

    private static string? GetProjectIdOrNull(FirebaseConfig config)
    {
        // On essaye d’être tolérant: si ta FirebaseConfig a ProjectId / projectId, etc.
        var t = config.GetType();

        var prop =
            t.GetProperty("ProjectId")
            ?? t.GetProperty("projectId")
            ?? t.GetProperty("FirebaseProjectId")
            ?? t.GetProperty("FirebaseProjectID");

        return prop?.GetValue(config) as string;
    }

    private static string? GetBucketNameOrNull(FirebaseConfig config)
    {
        var t = config.GetType();

        var prop =
            t.GetProperty("BucketName")
            ?? t.GetProperty("bucketName")
            ?? t.GetProperty("StorageBucket")
            ?? t.GetProperty("FirebaseStorageBucket");

        return prop?.GetValue(config) as string;
    }
}
