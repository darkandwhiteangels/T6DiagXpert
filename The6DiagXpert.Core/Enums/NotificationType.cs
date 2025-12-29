namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Définit les différents types de notifications système.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Notification informative.
    /// Message d'information général sans criticité particulière.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Avertissement.
    /// Situation nécessitant attention mais non critique.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Erreur.
    /// Problème nécessitant une action immédiate.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Succès.
    /// Confirmation de la réussite d'une opération.
    /// </summary>
    Success = 3
}
