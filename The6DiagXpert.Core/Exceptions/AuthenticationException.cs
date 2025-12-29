namespace The6DiagXpert.Core.Exceptions;

/// <summary>
/// Exception levée lorsqu'une erreur d'authentification ou d'autorisation se produit.
/// </summary>
public class AuthenticationException : Exception
{
    /// <summary>
    /// Code d'erreur spécifique à l'authentification.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur concerné (si applicable).
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Adresse email de l'utilisateur concerné (si applicable).
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Type d'erreur d'authentification.
    /// </summary>
    public AuthenticationErrorType ErrorType { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthenticationException"/>.
    /// </summary>
    public AuthenticationException()
        : base("Une erreur d'authentification s'est produite.")
    {
        ErrorType = AuthenticationErrorType.Unknown;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthenticationException"/> avec un message d'erreur spécifique.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    public AuthenticationException(string message)
        : base(message)
    {
        ErrorType = AuthenticationErrorType.Unknown;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthenticationException"/> avec un message d'erreur et une exception interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorType = AuthenticationErrorType.Unknown;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthenticationException"/> avec un type d'erreur spécifique.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="errorType">Type d'erreur d'authentification.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public AuthenticationException(string message, AuthenticationErrorType errorType, string? errorCode = null)
        : base(message)
    {
        ErrorType = errorType;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthenticationException"/> avec des informations utilisateur.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="errorType">Type d'erreur d'authentification.</param>
    /// <param name="userEmail">Email de l'utilisateur concerné.</param>
    /// <param name="userId">Identifiant de l'utilisateur concerné.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public AuthenticationException(
        string message,
        AuthenticationErrorType errorType,
        string? userEmail = null,
        string? userId = null,
        string? errorCode = null)
        : base(message)
    {
        ErrorType = errorType;
        UserEmail = userEmail;
        UserId = userId;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthenticationException"/> avec des informations complètes.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="errorType">Type d'erreur d'authentification.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    /// <param name="userEmail">Email de l'utilisateur concerné.</param>
    /// <param name="userId">Identifiant de l'utilisateur concerné.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public AuthenticationException(
        string message,
        AuthenticationErrorType errorType,
        Exception innerException,
        string? userEmail = null,
        string? userId = null,
        string? errorCode = null)
        : base(message, innerException)
    {
        ErrorType = errorType;
        UserEmail = userEmail;
        UserId = userId;
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Définit les types d'erreurs d'authentification.
/// </summary>
public enum AuthenticationErrorType
{
    /// <summary>
    /// Type d'erreur inconnu.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Identifiants invalides (email/mot de passe incorrects).
    /// </summary>
    InvalidCredentials = 1,

    /// <summary>
    /// Compte utilisateur désactivé.
    /// </summary>
    AccountDisabled = 2,

    /// <summary>
    /// Compte utilisateur verrouillé (trop de tentatives).
    /// </summary>
    AccountLocked = 3,

    /// <summary>
    /// Token d'authentification expiré.
    /// </summary>
    TokenExpired = 4,

    /// <summary>
    /// Token d'authentification invalide.
    /// </summary>
    InvalidToken = 5,

    /// <summary>
    /// Permissions insuffisantes pour l'action demandée.
    /// </summary>
    InsufficientPermissions = 6,

    /// <summary>
    /// Session expirée.
    /// </summary>
    SessionExpired = 7,

    /// <summary>
    /// Authentification multi-facteurs requise.
    /// </summary>
    MfaRequired = 8,

    /// <summary>
    /// Code de vérification invalide.
    /// </summary>
    InvalidVerificationCode = 9
}
