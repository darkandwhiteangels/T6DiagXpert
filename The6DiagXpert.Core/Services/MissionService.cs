using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Core.Services;

/// <summary>
/// Service helper pour la gestion des missions et diagnostics.
/// Contient la logique métier et les règles de calcul.
/// </summary>
public class MissionService
{
    /// <summary>
    /// Calcule la durée de validité standard d'un diagnostic en années.
    /// </summary>
    public static int GetDiagnosticValidityYears(DiagnosticType diagnosticType)
    {
        return diagnosticType switch
        {
            DiagnosticType.DPE => 10,
            DiagnosticType.Amiante => 999, // Illimité si négatif
            DiagnosticType.Plomb => 999, // Illimité si négatif (vente) ou 6 ans (location)
            DiagnosticType.Termites => 0, // 6 mois
            DiagnosticType.Gaz => 3,
            DiagnosticType.Electricite => 3,
            DiagnosticType.ERP => 0, // 6 mois
            DiagnosticType.LoiCarrez => 999, // Illimité
            DiagnosticType.LoiBoutin => 999, // Illimité
            DiagnosticType.AssainissementNC => 3,
            _ => 10
        };
    }

    /// <summary>
    /// Calcule la durée de validité standard d'un diagnostic en mois.
    /// </summary>
    public static int GetDiagnosticValidityMonths(DiagnosticType diagnosticType)
    {
        return diagnosticType switch
        {
            DiagnosticType.Termites => 6,
            DiagnosticType.ERP => 6,
            _ => GetDiagnosticValidityYears(diagnosticType) * 12
        };
    }

    /// <summary>
    /// Calcule la date d'expiration d'un diagnostic.
    /// </summary>
    public static DateTime CalculateExpiryDate(DiagnosticType diagnosticType, DateTime diagnosticDate)
    {
        var months = GetDiagnosticValidityMonths(diagnosticType);
        
        // Si validité illimitée (999 ans), retourner une date très éloignée
        if (months >= 999 * 12)
            return DateTime.MaxValue.AddYears(-1);
        
        return diagnosticDate.AddMonths(months);
    }

    /// <summary>
    /// Vérifie si un diagnostic est obligatoire pour un type de transaction.
    /// </summary>
    public static bool IsDiagnosticRequired(DiagnosticType diagnosticType, TransactionType transactionType, PropertyUsage propertyUsage)
    {
        // DPE obligatoire pour vente et location de biens résidentiels
        if (diagnosticType == DiagnosticType.DPE)
        {
            return propertyUsage == PropertyUsage.Residential &&
                   (transactionType == TransactionType.Sale || transactionType == TransactionType.Rental);
        }

        // Amiante obligatoire pour vente et location de biens construits avant 1997
        if (diagnosticType == DiagnosticType.Amiante)
        {
            return transactionType == TransactionType.Sale || transactionType == TransactionType.Rental;
        }

        // Plomb obligatoire pour vente et location de biens construits avant 1949
        if (diagnosticType == DiagnosticType.Plomb)
        {
            return transactionType == TransactionType.Sale || transactionType == TransactionType.Rental;
        }

        // Termites obligatoire uniquement en zones déclarées
        if (diagnosticType == DiagnosticType.Termites)
        {
            return transactionType == TransactionType.Sale;
        }

        // Gaz obligatoire si installation > 15 ans
        if (diagnosticType == DiagnosticType.Gaz)
        {
            return transactionType == TransactionType.Sale || transactionType == TransactionType.Rental;
        }

        // Électricité obligatoire si installation > 15 ans
        if (diagnosticType == DiagnosticType.Electricite)
        {
            return transactionType == TransactionType.Sale || transactionType == TransactionType.Rental;
        }

        // ERP obligatoire pour vente
        if (diagnosticType == DiagnosticType.ERP)
        {
            return transactionType == TransactionType.Sale;
        }

        // Carrez obligatoire pour vente de lots en copropriété > 8m²
        if (diagnosticType == DiagnosticType.LoiCarrez)
        {
            return transactionType == TransactionType.Sale;
        }

        // Boutin obligatoire pour location
        if (diagnosticType == DiagnosticType.LoiBoutin)
        {
            return transactionType == TransactionType.Rental;
        }

        // Assainissement obligatoire pour vente de maisons individuelles
        if (diagnosticType == DiagnosticType.AssainissementNC)
        {
            return transactionType == TransactionType.Sale &&
                   propertyUsage == PropertyUsage.Residential;
        }

        return false;
    }

    /// <summary>
    /// Obtient la liste des diagnostics recommandés pour une transaction.
    /// </summary>
    public static List<DiagnosticType> GetRecommendedDiagnostics(
        TransactionType transactionType,
        PropertyUsage propertyUsage,
        PropertyType propertyType,
        int? constructionYear = null)
    {
        var diagnostics = new List<DiagnosticType>();

        // DPE pour résidentiel
        if (propertyUsage == PropertyUsage.Residential)
        {
            diagnostics.Add(DiagnosticType.DPE);
        }

        // Amiante pour biens construits avant 1997
        if (constructionYear.HasValue && constructionYear.Value < 1997)
        {
            diagnostics.Add(DiagnosticType.Amiante);
        }

        // Plomb pour biens construits avant 1949
        if (constructionYear.HasValue && constructionYear.Value < 1949)
        {
            diagnostics.Add(DiagnosticType.Plomb);
        }

        // Termites pour ventes
        if (transactionType == TransactionType.Sale)
        {
            diagnostics.Add(DiagnosticType.Termites);
            diagnostics.Add(DiagnosticType.ERP);
        }

        // Gaz et électricité
        diagnostics.Add(DiagnosticType.Gaz);
        diagnostics.Add(DiagnosticType.Electricite);

        // Carrez pour vente en copropriété
        if (transactionType == TransactionType.Sale &&
            (propertyType == PropertyType.Apartment || propertyType == PropertyType.Building))
        {
            diagnostics.Add(DiagnosticType.LoiCarrez);
        }

        // Boutin pour location
        if (transactionType == TransactionType.Rental)
        {
            diagnostics.Add(DiagnosticType.LoiBoutin);
        }

        // Assainissement pour maisons individuelles
        if (transactionType == TransactionType.Sale &&
            propertyType == PropertyType.House)
        {
            diagnostics.Add(DiagnosticType.AssainissementNC);
        }

        return diagnostics.Distinct().ToList();
    }

    /// <summary>
    /// Calcule la durée estimée totale d'une mission en minutes.
    /// </summary>
    public static int CalculateEstimatedDuration(List<DiagnosticType> diagnosticTypes)
    {
        var totalMinutes = 0;

        foreach (var type in diagnosticTypes)
        {
            totalMinutes += GetEstimatedDiagnosticDuration(type);
        }

        // Ajouter 15 minutes de temps de déplacement/installation par mission
        totalMinutes += 15;

        return totalMinutes;
    }

    /// <summary>
    /// Obtient la durée estimée d'un diagnostic en minutes.
    /// </summary>
    public static int GetEstimatedDiagnosticDuration(DiagnosticType diagnosticType)
    {
        return diagnosticType switch
        {
            DiagnosticType.DPE => 90,
            DiagnosticType.Amiante => 120,
            DiagnosticType.Plomb => 60,
            DiagnosticType.Termites => 45,
            DiagnosticType.Gaz => 45,
            DiagnosticType.Electricite => 45,
            DiagnosticType.ERP => 30,
            DiagnosticType.LoiCarrez => 30,
            DiagnosticType.LoiBoutin => 30,
            DiagnosticType.AssainissementNC => 60,
            _ => 60
        };
    }

    /// <summary>
    /// Calcule le tarif standard d'un diagnostic (HT).
    /// </summary>
    public static decimal GetStandardPrice(DiagnosticType diagnosticType, decimal? surface = null)
    {
        var basePrice = diagnosticType switch
        {
            DiagnosticType.DPE => 150m,
            DiagnosticType.Amiante => 200m,
            DiagnosticType.Plomb => 120m,
            DiagnosticType.Termites => 100m,
            DiagnosticType.Gaz => 100m,
            DiagnosticType.Electricite => 100m,
            DiagnosticType.ERP => 80m,
            DiagnosticType.LoiCarrez => 80m,
            DiagnosticType.LoiBoutin => 80m,
            DiagnosticType.AssainissementNC => 120m,
            _ => 100m
        };

        // Majoration pour les grandes surfaces
        if (surface.HasValue && surface.Value > 200)
        {
            basePrice *= 1.2m; // +20% pour > 200m²
        }
        else if (surface.HasValue && surface.Value > 100)
        {
            basePrice *= 1.1m; // +10% pour > 100m²
        }

        return Math.Round(basePrice, 2);
    }

    /// <summary>
    /// Calcule le tarif d'une mission complète (HT).
    /// </summary>
    public static decimal CalculateMissionPrice(List<DiagnosticType> diagnosticTypes, decimal? surface = null)
    {
        decimal totalPrice = 0;

        foreach (var type in diagnosticTypes)
        {
            totalPrice += GetStandardPrice(type, surface);
        }

        // Réduction si plusieurs diagnostics
        if (diagnosticTypes.Count >= 5)
        {
            totalPrice *= 0.85m; // -15% pour 5 diagnostics ou plus
        }
        else if (diagnosticTypes.Count >= 3)
        {
            totalPrice *= 0.90m; // -10% pour 3-4 diagnostics
        }

        return Math.Round(totalPrice, 2);
    }

    /// <summary>
    /// Valide qu'une mission peut être créée.
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateMissionCreation(
        Guid clientId,
        Guid propertyId,
        List<DiagnosticType> diagnosticTypes)
    {
        if (clientId == Guid.Empty)
            return (false, "Le client est obligatoire");

        if (propertyId == Guid.Empty)
            return (false, "Le bien immobilier est obligatoire");

        if (diagnosticTypes == null || !diagnosticTypes.Any())
            return (false, "Au moins un diagnostic doit être sélectionné");

        if (diagnosticTypes.Distinct().Count() != diagnosticTypes.Count)
            return (false, "Les diagnostics ne peuvent pas être dupliqués");

        return (true, null);
    }

    /// <summary>
    /// Obtient un libellé court pour un type de diagnostic.
    /// </summary>
    public static string GetDiagnosticLabel(DiagnosticType diagnosticType)
    {
        return diagnosticType switch
        {
            DiagnosticType.DPE => "DPE",
            DiagnosticType.Amiante => "Amiante",
            DiagnosticType.Plomb => "Plomb",
            DiagnosticType.Termites => "Termites",
            DiagnosticType.Gaz => "Gaz",
            DiagnosticType.Electricite => "Électricité",
            DiagnosticType.ERP => "ERP",
            DiagnosticType.LoiCarrez => "Loi Carrez",
            DiagnosticType.LoiBoutin => "Loi Boutin",
            DiagnosticType.AssainissementNC => "Assainissement",
            _ => diagnosticType.ToString()
        };
    }

    /// <summary>
    /// Obtient une description complète pour un type de diagnostic.
    /// </summary>
    public static string GetDiagnosticDescription(DiagnosticType diagnosticType)
    {
        return diagnosticType switch
        {
            DiagnosticType.DPE => "Diagnostic de Performance Énergétique",
            DiagnosticType.Amiante => "Diagnostic Amiante avant travaux ou démolition",
            DiagnosticType.Plomb => "Constat de Risque d'Exposition au Plomb (CREP)",
            DiagnosticType.Termites => "État relatif à la présence de termites",
            DiagnosticType.Gaz => "État de l'installation intérieure de gaz",
            DiagnosticType.Electricite => "État de l'installation intérieure d'électricité",
            DiagnosticType.ERP => "État des Risques et Pollutions",
            DiagnosticType.LoiCarrez => "Mesurage de la superficie privative (Loi Carrez)",
            DiagnosticType.LoiBoutin => "Mesurage de la superficie habitable (Loi Boutin)",
            DiagnosticType.AssainissementNC => "État de l'installation d'assainissement non collectif",
            _ => "Diagnostic immobilier"
        };
    }
}
