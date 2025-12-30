namespace The6DiagXpert.Infrastructure.Firebase;

/// <summary>
/// Configuration Firebase pour l'application
/// Contient les paramètres de connexion et configuration Firebase
/// </summary>
public class FirebaseConfig
{
    /// <summary>
    /// Clé API Firebase
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Domaine d'authentification
    /// </summary>
    public string AuthDomain { get; set; } = string.Empty;

    /// <summary>
    /// ID du projet Firebase
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// Bucket de stockage
    /// </summary>
    public string StorageBucket { get; set; } = string.Empty;

    /// <summary>
    /// ID de l'expéditeur de messagerie
    /// </summary>
    public string MessagingSenderId { get; set; } = string.Empty;

    /// <summary>
    /// ID de l'application
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// Clé de mesure (Analytics)
    /// </summary>
    public string? MeasurementId { get; set; }

    /// <summary>
    /// URL de la base de données Realtime
    /// </summary>
    public string? DatabaseUrl { get; set; }

    /// <summary>
    /// Timeout pour les requêtes (en secondes)
    /// </summary>
    public int RequestTimeout { get; set; } = 30;

    /// <summary>
    /// Nombre maximum de tentatives de retry
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Activer les logs Firebase
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// Activer le mode hors ligne
    /// </summary>
    public bool EnableOfflineMode { get; set; } = true;

    /// <summary>
    /// Taille du cache hors ligne (en Mo)
    /// </summary>
    public long OfflineCacheSizeMb { get; set; } = 100;

    /// <summary>
    /// Activer la persistance des données
    /// </summary>
    public bool EnablePersistence { get; set; } = true;

    /// <summary>
    /// Activer l'authentification anonyme
    /// </summary>
    public bool AllowAnonymousAuth { get; set; } = false;

    /// <summary>
    /// Durée de validité du token (en minutes)
    /// </summary>
    public int TokenLifetimeMinutes { get; set; } = 60;

    /// <summary>
    /// Activer le rafraîchissement automatique du token
    /// </summary>
    public bool AutoRefreshToken { get; set; } = true;

    /// <summary>
    /// Constructeur par défaut
    /// </summary>
    public FirebaseConfig()
    {
    }

    /// <summary>
    /// Constructeur avec paramètres essentiels
    /// </summary>
    public FirebaseConfig(string apiKey, string projectId, string authDomain, string storageBucket)
    {
        ApiKey = apiKey;
        ProjectId = projectId;
        AuthDomain = authDomain;
        StorageBucket = storageBucket;
    }

    /// <summary>
    /// Valide la configuration
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ApiKey) &&
               !string.IsNullOrWhiteSpace(ProjectId) &&
               !string.IsNullOrWhiteSpace(AuthDomain) &&
               !string.IsNullOrWhiteSpace(StorageBucket);
    }

    /// <summary>
    /// Retourne l'URL complète de l'API Firebase Auth
    /// </summary>
    public string GetAuthApiUrl()
    {
        return $"https://identitytoolkit.googleapis.com/v1/accounts";
    }

    /// <summary>
    /// Retourne l'URL complète de Firestore
    /// </summary>
    public string GetFirestoreUrl()
    {
        return $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
    }

    /// <summary>
    /// Retourne l'URL complète de Firebase Storage
    /// </summary>
    public string GetStorageUrl()
    {
        return $"https://firebasestorage.googleapis.com/v0/b/{StorageBucket}/o";
    }

    /// <summary>
    /// Retourne l'URL complète de Realtime Database
    /// </summary>
    public string GetRealtimeDatabaseUrl()
    {
        return DatabaseUrl ?? $"https://{ProjectId}.firebaseio.com";
    }

    /// <summary>
    /// Clone la configuration
    /// </summary>
    public FirebaseConfig Clone()
    {
        return new FirebaseConfig
        {
            ApiKey = ApiKey,
            AuthDomain = AuthDomain,
            ProjectId = ProjectId,
            StorageBucket = StorageBucket,
            MessagingSenderId = MessagingSenderId,
            AppId = AppId,
            MeasurementId = MeasurementId,
            DatabaseUrl = DatabaseUrl,
            RequestTimeout = RequestTimeout,
            MaxRetryAttempts = MaxRetryAttempts,
            EnableLogging = EnableLogging,
            EnableOfflineMode = EnableOfflineMode,
            OfflineCacheSizeMb = OfflineCacheSizeMb,
            EnablePersistence = EnablePersistence,
            AllowAnonymousAuth = AllowAnonymousAuth,
            TokenLifetimeMinutes = TokenLifetimeMinutes,
            AutoRefreshToken = AutoRefreshToken
        };
    }

    /// <summary>
    /// Crée une configuration par défaut pour le développement
    /// </summary>
    public static FirebaseConfig CreateDefault()
    {
        return new FirebaseConfig
        {
            RequestTimeout = 30,
            MaxRetryAttempts = 3,
            EnableLogging = true,
            EnableOfflineMode = true,
            OfflineCacheSizeMb = 100,
            EnablePersistence = true,
            AllowAnonymousAuth = false,
            TokenLifetimeMinutes = 60,
            AutoRefreshToken = true
        };
    }

    /// <summary>
    /// Charge la configuration depuis un fichier JSON
    /// </summary>
    public static FirebaseConfig LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Le fichier de configuration Firebase n'existe pas : {filePath}");

        var json = File.ReadAllText(filePath);
        var config = System.Text.Json.JsonSerializer.Deserialize<FirebaseConfig>(json);

        if (config == null)
            throw new InvalidOperationException("Impossible de désérialiser la configuration Firebase");

        return config;
    }

    /// <summary>
    /// Sauvegarde la configuration dans un fichier JSON
    /// </summary>
    public void SaveToFile(string filePath)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = System.Text.Json.JsonSerializer.Serialize(this, options);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Charge la configuration depuis les variables d'environnement
    /// </summary>
    public static FirebaseConfig LoadFromEnvironment()
    {
        return new FirebaseConfig
        {
            ApiKey = Environment.GetEnvironmentVariable("FIREBASE_API_KEY") ?? string.Empty,
            AuthDomain = Environment.GetEnvironmentVariable("FIREBASE_AUTH_DOMAIN") ?? string.Empty,
            ProjectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID") ?? string.Empty,
            StorageBucket = Environment.GetEnvironmentVariable("FIREBASE_STORAGE_BUCKET") ?? string.Empty,
            MessagingSenderId = Environment.GetEnvironmentVariable("FIREBASE_MESSAGING_SENDER_ID") ?? string.Empty,
            AppId = Environment.GetEnvironmentVariable("FIREBASE_APP_ID") ?? string.Empty,
            MeasurementId = Environment.GetEnvironmentVariable("FIREBASE_MEASUREMENT_ID"),
            DatabaseUrl = Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL")
        };
    }

    /// <summary>
    /// Masque les informations sensibles pour le logging
    /// </summary>
    public FirebaseConfig Sanitize()
    {
        return new FirebaseConfig
        {
            ApiKey = MaskSensitiveData(ApiKey),
            AuthDomain = AuthDomain,
            ProjectId = ProjectId,
            StorageBucket = StorageBucket,
            MessagingSenderId = MessagingSenderId,
            AppId = MaskSensitiveData(AppId),
            MeasurementId = MeasurementId,
            DatabaseUrl = DatabaseUrl,
            RequestTimeout = RequestTimeout,
            MaxRetryAttempts = MaxRetryAttempts,
            EnableLogging = EnableLogging,
            EnableOfflineMode = EnableOfflineMode,
            OfflineCacheSizeMb = OfflineCacheSizeMb,
            EnablePersistence = EnablePersistence,
            AllowAnonymousAuth = AllowAnonymousAuth,
            TokenLifetimeMinutes = TokenLifetimeMinutes,
            AutoRefreshToken = AutoRefreshToken
        };
    }

    /// <summary>
    /// Masque une donnée sensible
    /// </summary>
    private static string MaskSensitiveData(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        if (data.Length <= 8)
            return "***";

        return $"{data.Substring(0, 4)}...{data.Substring(data.Length - 4)}";
    }

    /// <summary>
    /// Retourne une représentation textuelle (sans données sensibles)
    /// </summary>
    public override string ToString()
    {
        return $"Firebase Config - Project: {ProjectId}, Auth: {AuthDomain}";
    }
}
