using System;
using System.Collections.Generic;
using The6DiagXpert.Core.Enums;

namespace The6DiagXpert.Core.Services;

public interface IMissionService
{
    int GetDiagnosticValidityYears(DiagnosticType diagnosticType);
    int GetDiagnosticValidityMonths(DiagnosticType diagnosticType);
    DateTime CalculateExpiryDate(DiagnosticType diagnosticType, DateTime diagnosticDate);

    bool IsDiagnosticRequired(DiagnosticType diagnosticType, TransactionType transactionType, PropertyUsage propertyUsage);

    List<DiagnosticType> GetRecommendedDiagnostics(
        TransactionType transactionType,
        PropertyUsage propertyUsage,
        PropertyType propertyType,
        int? constructionYear = null);

    int CalculateEstimatedDuration(List<DiagnosticType> diagnosticTypes);
    int GetEstimatedDiagnosticDuration(DiagnosticType diagnosticType);

    decimal GetStandardPrice(DiagnosticType diagnosticType, decimal? surface = null);
    decimal CalculateMissionPrice(List<DiagnosticType> diagnosticTypes, decimal? surface = null);

    (bool IsValid, string? ErrorMessage) ValidateMissionCreation(Guid clientId, Guid propertyId, List<DiagnosticType> diagnosticTypes);

    string GetDiagnosticLabel(DiagnosticType diagnosticType);
    string GetDiagnosticDescription(DiagnosticType diagnosticType);
}
