namespace The6DiagXpert.Core.Constants;

/// <summary>
/// Constantes définissant les règles de validation pour les données de l'application.
/// </summary>
public static class ValidationRules
{
    /// <summary>
    /// Longueur minimale d'un mot de passe.
    /// </summary>
    public const int MIN_PASSWORD_LENGTH = 8;

    /// <summary>
    /// Longueur maximale d'un mot de passe.
    /// </summary>
    public const int MAX_PASSWORD_LENGTH = 128;

    /// <summary>
    /// Longueur minimale du nom d'une entreprise.
    /// </summary>
    public const int MIN_COMPANY_NAME_LENGTH = 2;

    /// <summary>
    /// Longueur maximale du nom d'une entreprise.
    /// </summary>
    public const int MAX_COMPANY_NAME_LENGTH = 100;

    /// <summary>
    /// Longueur minimale d'un nom ou prénom.
    /// </summary>
    public const int MIN_NAME_LENGTH = 2;

    /// <summary>
    /// Longueur maximale d'un nom ou prénom.
    /// </summary>
    public const int MAX_NAME_LENGTH = 50;

    /// <summary>
    /// Longueur exacte d'un numéro SIRET (sans espaces).
    /// </summary>
    public const int SIRET_LENGTH = 14;

    /// <summary>
    /// Longueur exacte d'un numéro SIREN (sans espaces).
    /// </summary>
    public const int SIREN_LENGTH = 9;

    /// <summary>
    /// Longueur minimale d'une adresse.
    /// </summary>
    public const int MIN_ADDRESS_LENGTH = 5;

    /// <summary>
    /// Longueur maximale d'une adresse.
    /// </summary>
    public const int MAX_ADDRESS_LENGTH = 200;

    /// <summary>
    /// Longueur minimale d'un code postal français.
    /// </summary>
    public const int POSTAL_CODE_LENGTH = 5;

    /// <summary>
    /// Longueur minimale d'un nom de ville.
    /// </summary>
    public const int MIN_CITY_LENGTH = 2;

    /// <summary>
    /// Longueur maximale d'un nom de ville.
    /// </summary>
    public const int MAX_CITY_LENGTH = 100;

    /// <summary>
    /// Longueur maximale d'un numéro de téléphone.
    /// </summary>
    public const int MAX_PHONE_LENGTH = 20;

    /// <summary>
    /// Longueur maximale d'une adresse email.
    /// </summary>
    public const int MAX_EMAIL_LENGTH = 255;

    /// <summary>
    /// Longueur maximale d'un commentaire ou description.
    /// </summary>
    public const int MAX_COMMENT_LENGTH = 2000;

    /// <summary>
    /// Longueur maximale d'un titre ou libellé.
    /// </summary>
    public const int MAX_TITLE_LENGTH = 200;

    /// <summary>
    /// Valeur minimale pour une surface habitable en m².
    /// </summary>
    public const double MIN_SURFACE_M2 = 1.0;

    /// <summary>
    /// Valeur maximale pour une surface habitable en m².
    /// </summary>
    public const double MAX_SURFACE_M2 = 100000.0;

    /// <summary>
    /// Valeur minimale pour une surface de terrain en m².
    /// </summary>
    public const double MIN_LAND_SURFACE_M2 = 1.0;

    /// <summary>
    /// Valeur maximale pour une surface de terrain en m².
    /// </summary>
    public const double MAX_LAND_SURFACE_M2 = 10000000.0;

    /// <summary>
    /// Année minimale de construction valide.
    /// </summary>
    public const int MIN_CONSTRUCTION_YEAR = 1800;

    /// <summary>
    /// Nombre minimum de pièces pour un logement.
    /// </summary>
    public const int MIN_ROOMS_COUNT = 1;

    /// <summary>
    /// Nombre maximum de pièces pour un logement.
    /// </summary>
    public const int MAX_ROOMS_COUNT = 50;

    /// <summary>
    /// Expression régulière pour validation d'un email.
    /// </summary>
    public const string EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    /// <summary>
    /// Expression régulière pour validation d'un numéro de téléphone français.
    /// Accepte les formats avec ou sans espaces/tirets.
    /// </summary>
    public const string FRENCH_PHONE_REGEX = @"^(?:(?:\+|00)33|0)\s*[1-9](?:[\s.-]*\d{2}){4}$";

    /// <summary>
    /// Expression régulière pour validation d'un code postal français.
    /// </summary>
    public const string FRENCH_POSTAL_CODE_REGEX = @"^[0-9]{5}$";

    /// <summary>
    /// Expression régulière pour validation d'un SIRET.
    /// </summary>
    public const string SIRET_REGEX = @"^\d{14}$";

    /// <summary>
    /// Durée minimale d'une mission en heures.
    /// </summary>
    public const int MIN_MISSION_DURATION_HOURS = 1;

    /// <summary>
    /// Durée maximale d'une mission en heures.
    /// </summary>
    public const int MAX_MISSION_DURATION_HOURS = 168; // 1 semaine

    /// <summary>
    /// Montant minimum d'une facture en euros.
    /// </summary>
    public const decimal MIN_INVOICE_AMOUNT = 0.01m;

    /// <summary>
    /// Montant maximum d'une facture en euros.
    /// </summary>
    public const decimal MAX_INVOICE_AMOUNT = 999999.99m;
}