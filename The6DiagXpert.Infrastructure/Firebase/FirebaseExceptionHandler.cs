using System;
using System.Text;

namespace The6DiagXpert.Infrastructure.Firebase;

/// <summary>
/// Centralise la normalisation des erreurs Firebase / HTTP / réseau.
/// Objectif: fournir un message propre + un "code" exploitable côté UI/log.
/// </summary>
public static class FirebaseExceptionHandler
{
    public static FirebaseHandledError Handle(Exception ex, string operation = "Firebase")
    {
        if (ex == null) throw new ArgumentNullException(nameof(ex));

        var message = ex.Message ?? "Erreur inconnue";
        var code = "UNKNOWN";
        var severity = Logging.LogLevel.Error;

        // Heuristiques simples (tu pourras enrichir plus tard)
        var msgUpper = message.ToUpperInvariant();

        if (msgUpper.Contains("INVALID_PASSWORD") || msgUpper.Contains("EMAIL_NOT_FOUND"))
        {
            code = "AUTH_INVALID_CREDENTIALS";
            severity = Logging.LogLevel.Warning;
            message = "Identifiants invalides.";
        }
        else if (msgUpper.Contains("USER_DISABLED"))
        {
            code = "AUTH_USER_DISABLED";
            severity = Logging.LogLevel.Warning;
            message = "Ce compte est désactivé.";
        }
        else if (msgUpper.Contains("EMAIL_EXISTS"))
        {
            code = "AUTH_EMAIL_EXISTS";
            severity = Logging.LogLevel.Warning;
            message = "Cet email est déjà utilisé.";
        }
        else if (msgUpper.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
        {
            code = "AUTH_TOO_MANY_ATTEMPTS";
            severity = Logging.LogLevel.Warning;
            message = "Trop de tentatives. Réessaie plus tard.";
        }
        else if (msgUpper.Contains("NETWORK") || msgUpper.Contains("TIMEOUT") || msgUpper.Contains("NAME RESOLUTION"))
        {
            code = "NETWORK_ERROR";
            severity = Logging.LogLevel.Warning;
            message = "Problème réseau. Vérifie ta connexion internet.";
        }
        else if (msgUpper.Contains("PERMISSION") || msgUpper.Contains("INSUFFICIENT PERMISSIONS"))
        {
            code = "PERMISSION_DENIED";
            severity = Logging.LogLevel.Warning;
            message = "Accès refusé (permissions insuffisantes).";
        }
        else if (msgUpper.Contains("NOT FOUND") || msgUpper.Contains("404"))
        {
            code = "NOT_FOUND";
            severity = Logging.LogLevel.Warning;
            message = "Ressource introuvable.";
        }

        var details = BuildDetails(ex);

        return new FirebaseHandledError
        {
            Operation = operation,
            Code = code,
            UserMessage = message,
            Severity = severity,
            TechnicalDetails = details,
            Exception = ex
        };
    }

    private static string BuildDetails(Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine(ex.GetType().FullName);
        sb.AppendLine(ex.Message);

        if (ex.InnerException != null)
        {
            sb.AppendLine("INNER:");
            sb.AppendLine(ex.InnerException.GetType().FullName);
            sb.AppendLine(ex.InnerException.Message);
        }

        return sb.ToString();
    }
}

/// <summary>
/// Erreur normalisée après traitement.
/// </summary>
public sealed class FirebaseHandledError
{
    public string Operation { get; init; } = "Firebase";
    public string Code { get; init; } = "UNKNOWN";
    public string UserMessage { get; init; } = "Erreur inconnue";

    public The6DiagXpert.Infrastructure.Logging.LogLevel Severity { get; init; }
        = The6DiagXpert.Infrastructure.Logging.LogLevel.Error;

    public string TechnicalDetails { get; init; } = string.Empty;

    public Exception? Exception { get; init; }
}
