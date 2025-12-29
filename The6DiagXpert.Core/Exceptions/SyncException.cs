namespace The6DiagXpert.Core.Exceptions;

/// <summary>
/// Exception levée lorsqu'une erreur se produit lors de la synchronisation des données.
/// </summary>
public class SyncException : Exception
{
    /// <summary>
    /// Code d'erreur spécifique à la synchronisation.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Identifiant de l'entité concernée par l'erreur de synchronisation.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Type d'entité concernée par l'erreur de synchronisation.
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="SyncException"/>.
    /// </summary>
    public SyncException()
        : base("Une erreur de synchronisation s'est produite.")
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="SyncException"/> avec un message d'erreur spécifique.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    public SyncException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="SyncException"/> avec un message d'erreur et une exception interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    public SyncException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="SyncException"/> avec des informations détaillées.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    /// <param name="entityId">Identifiant de l'entité concernée.</param>
    /// <param name="entityType">Type d'entité concernée.</param>
    public SyncException(string message, string errorCode, string? entityId = null, string? entityType = null)
        : base(message)
    {
        ErrorCode = errorCode;
        EntityId = entityId;
        EntityType = entityType;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="SyncException"/> avec des informations détaillées et une exception interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    /// <param name="entityId">Identifiant de l'entité concernée.</param>
    /// <param name="entityType">Type d'entité concernée.</param>
    public SyncException(string message, string errorCode, Exception innerException, string? entityId = null, string? entityType = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        EntityId = entityId;
        EntityType = entityType;
    }
}
