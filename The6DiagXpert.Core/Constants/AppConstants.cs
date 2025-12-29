namespace The6DiagXpert.Core.Constants;

/// <summary>
/// Constantes générales de l'application The6DiagXpert.
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Nom de l'application.
    /// </summary>
    public const string APP_NAME = "The6DiagXpert";

    /// <summary>
    /// Version actuelle de l'application.
    /// </summary>
    public const string VERSION = "1.0.0";

    /// <summary>
    /// Langue par défaut de l'application.
    /// </summary>
    public const string DEFAULT_LANGUAGE = "fr-FR";

    /// <summary>
    /// Taille maximale des photos en mégaoctets.
    /// </summary>
    public const int MAX_PHOTO_SIZE_MB = 10;

    /// <summary>
    /// Taille maximale des documents en mégaoctets.
    /// </summary>
    public const int MAX_DOCUMENT_SIZE_MB = 50;

    /// <summary>
    /// Formats d'image supportés.
    /// </summary>
    public static readonly string[] SUPPORTED_IMAGE_FORMATS = new[]
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    };

    /// <summary>
    /// Formats de document supportés.
    /// </summary>
    public static readonly string[] SUPPORTED_DOCUMENT_FORMATS = new[]
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"
    };

    /// <summary>
    /// Formats de plan supportés.
    /// </summary>
    public static readonly string[] SUPPORTED_PLAN_FORMATS = new[]
    {
        ".dwg", ".dxf", ".pdf"
    };

    /// <summary>
    /// Nombre maximum de tentatives de synchronisation.
    /// </summary>
    public const int MAX_SYNC_RETRIES = 3;

    /// <summary>
    /// Délai en secondes avant nouvelle tentative de synchronisation.
    /// </summary>
    public const int SYNC_RETRY_DELAY_SECONDS = 30;

    /// <summary>
    /// Durée de validité du cache en minutes.
    /// </summary>
    public const int CACHE_DURATION_MINUTES = 60;

    /// <summary>
    /// Nombre d'éléments par page dans les listes paginées.
    /// </summary>
    public const int DEFAULT_PAGE_SIZE = 20;

    /// <summary>
    /// Nombre maximum d'éléments par page.
    /// </summary>
    public const int MAX_PAGE_SIZE = 100;

    /// <summary>
    /// Durée de session en minutes.
    /// </summary>
    public const int SESSION_TIMEOUT_MINUTES = 480;

    /// <summary>
    /// Préfixe pour les clés de cache.
    /// </summary>
    public const string CACHE_KEY_PREFIX = "The6DiagXpert:";

    /// <summary>
    /// Format de date par défaut.
    /// </summary>
    public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";

    /// <summary>
    /// Format de date et heure par défaut.
    /// </summary>
    public const string DEFAULT_DATETIME_FORMAT = "dd/MM/yyyy HH:mm";

    /// <summary>
    /// Timezone par défaut (France métropolitaine).
    /// </summary>
    public const string DEFAULT_TIMEZONE = "Europe/Paris";
}