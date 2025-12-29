using System;
using System.Collections.Generic;
using System.Linq;
namespace The6DiagXpert.Core.Exceptions;

/// <summary>
/// Exception levée lorsqu'une erreur de validation se produit.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Code d'erreur spécifique à la validation.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Nom du champ ou de la propriété qui a échoué à la validation.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Valeur qui a échoué à la validation.
    /// </summary>
    public object? InvalidValue { get; set; }

    /// <summary>
    /// Liste des erreurs de validation (si multiples).
    /// </summary>
    public List<string>? ValidationErrors { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ValidationException"/>.
    /// </summary>
    public ValidationException()
        : base("Une erreur de validation s'est produite.")
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ValidationException"/> avec un message d'erreur spécifique.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    public ValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ValidationException"/> avec un message d'erreur et une exception interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ValidationException"/> avec des informations détaillées sur le champ.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="fieldName">Nom du champ qui a échoué à la validation.</param>
    /// <param name="invalidValue">Valeur invalide.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public ValidationException(string message, string fieldName, object? invalidValue = null, string? errorCode = null)
        : base(message)
    {
        FieldName = fieldName;
        InvalidValue = invalidValue;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ValidationException"/> avec plusieurs erreurs de validation.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur globale.</param>
    /// <param name="validationErrors">Liste des erreurs de validation.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public ValidationException(string message, List<string> validationErrors, string? errorCode = null)
        : base(message)
    {
        ValidationErrors = validationErrors;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ValidationException"/> avec des informations complètes.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="fieldName">Nom du champ qui a échoué à la validation.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    /// <param name="invalidValue">Valeur invalide.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public ValidationException(
        string message,
        string fieldName,
        Exception innerException,
        object? invalidValue = null,
        string? errorCode = null)
        : base(message, innerException)
    {
        FieldName = fieldName;
        InvalidValue = invalidValue;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Retourne un message détaillé incluant toutes les erreurs de validation.
    /// </summary>
    /// <returns>Message d'erreur formaté.</returns>
    public string GetDetailedMessage()
    {
        if (ValidationErrors != null && ValidationErrors.Any())
        {
            return $"{Message}{Environment.NewLine}{string.Join(Environment.NewLine, ValidationErrors)}";
        }

        if (!string.IsNullOrEmpty(FieldName))
        {
            return InvalidValue != null
                ? $"{Message} (Champ: {FieldName}, Valeur: {InvalidValue})"
                : $"{Message} (Champ: {FieldName})";
        }

        return Message;
    }
}
