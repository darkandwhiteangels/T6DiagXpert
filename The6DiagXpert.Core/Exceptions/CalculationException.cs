namespace The6DiagXpert.Core.Exceptions;

/// <summary>
/// Exception levée lorsqu'une erreur se produit lors d'un calcul (DPE, surfaces, etc.).
/// </summary>
public class CalculationException : Exception
{
    /// <summary>
    /// Code d'erreur spécifique au calcul.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Type de calcul qui a échoué.
    /// </summary>
    public string? CalculationType { get; set; }

    /// <summary>
    /// Paramètres utilisés lors du calcul (pour le débogage).
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CalculationException"/>.
    /// </summary>
    public CalculationException()
        : base("Une erreur de calcul s'est produite.")
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CalculationException"/> avec un message d'erreur spécifique.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    public CalculationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CalculationException"/> avec un message d'erreur et une exception interne.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    public CalculationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CalculationException"/> avec des informations détaillées.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="calculationType">Type de calcul qui a échoué.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public CalculationException(string message, string calculationType, string? errorCode = null)
        : base(message)
    {
        CalculationType = calculationType;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CalculationException"/> avec des informations détaillées et des paramètres.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="calculationType">Type de calcul qui a échoué.</param>
    /// <param name="parameters">Paramètres utilisés lors du calcul.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public CalculationException(string message, string calculationType, Dictionary<string, object> parameters, string? errorCode = null)
        : base(message)
    {
        CalculationType = calculationType;
        Parameters = parameters;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CalculationException"/> avec des informations complètes.
    /// </summary>
    /// <param name="message">Message décrivant l'erreur.</param>
    /// <param name="calculationType">Type de calcul qui a échoué.</param>
    /// <param name="innerException">Exception à l'origine de cette exception.</param>
    /// <param name="parameters">Paramètres utilisés lors du calcul.</param>
    /// <param name="errorCode">Code d'erreur spécifique.</param>
    public CalculationException(
        string message,
        string calculationType,
        Exception innerException,
        Dictionary<string, object>? parameters = null,
        string? errorCode = null)
        : base(message, innerException)
    {
        CalculationType = calculationType;
        Parameters = parameters;
        ErrorCode = errorCode;
    }
}
