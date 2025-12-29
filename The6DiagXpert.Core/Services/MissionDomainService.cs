using System;
using System.Collections.Generic;
using The6DiagXpert.Core.Enums;

namespace The6DiagXpert.Core.Services;

public sealed class MissionDomainService : IMissionService
{
    public int GetDiagnosticValidityYears(DiagnosticType diagnosticType)
        => MissionService.GetDiagnosticValidityYears(diagnosticType);

    public int GetDiagnosticValidityMonths(DiagnosticType diagnosticType)
        => MissionService.GetDiagnosticValidityMonths(diagnosticType);

    public DateTime CalculateExpiryDate(DiagnosticType diagnosticType, DateTime diagnosticDate)
        => MissionService.CalculateExpiryDate(diagnosticType, diagnosticDate);

    public bool IsDiagnosticRequired(DiagnosticType diagnosticType, TransactionType transactionType, PropertyUsage propertyUsage)
        => MissionService.IsDiagnosticRequired(diagnosticType, transactionType, propertyUsage);

    public List<DiagnosticType> GetRecommendedDiagnostics(TransactionType transactionType, PropertyUsage propertyUsage, PropertyType propertyType, int? constructionYear = null)
        => MissionService.GetRecommendedDiagnostics(transactionType, propertyUsage, propertyType, constructionYear);

    public int CalculateEstimatedDuration(List<DiagnosticType> diagnosticTypes)
        => MissionService.CalculateEstimatedDuration(diagnosticTypes);

    public int GetEstimatedDiagnosticDuration(DiagnosticType diagnosticType)
        => MissionService.GetEstimatedDiagnosticDuration(diagnosticType);

    public decimal GetStandardPrice(DiagnosticType diagnosticType, decimal? surface = null)
        => MissionService.GetStandardPrice(diagnosticType, surface);

    public decimal CalculateMissionPrice(List<DiagnosticType> diagnosticTypes, decimal? surface = null)
        => MissionService.CalculateMissionPrice(diagnosticTypes, surface);

    public (bool IsValid, string? ErrorMessage) ValidateMissionCreation(Guid clientId, Guid propertyId, List<DiagnosticType> diagnosticTypes)
        => MissionService.ValidateMissionCreation(clientId, propertyId, diagnosticTypes);

    public string GetDiagnosticLabel(DiagnosticType diagnosticType)
        => MissionService.GetDiagnosticLabel(diagnosticType);

    public string GetDiagnosticDescription(DiagnosticType diagnosticType)
        => MissionService.GetDiagnosticDescription(diagnosticType);
}
