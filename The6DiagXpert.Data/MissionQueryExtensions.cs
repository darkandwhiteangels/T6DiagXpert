using Microsoft.EntityFrameworkCore;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data;

/// <summary>
/// Extensions LINQ pour faciliter les requêtes sur les missions.
/// </summary>
public static class MissionQueryExtensions
{
    /// <summary>
    /// Filtre les missions par entreprise.
    /// </summary>
    public static IQueryable<Mission> ForCompany(this IQueryable<Mission> query, Guid companyId)
    {
        return query.Where(m => m.CompanyId == companyId);
    }

    /// <summary>
    /// Filtre les missions par statut.
    /// </summary>
    public static IQueryable<Mission> WithStatus(this IQueryable<Mission> query, MissionStatus status)
    {
        return query.Where(m => m.Status == status);
    }

    /// <summary>
    /// Filtre les missions par statuts multiples.
    /// </summary>
    public static IQueryable<Mission> WithStatuses(this IQueryable<Mission> query, params MissionStatus[] statuses)
    {
        return query.Where(m => statuses.Contains(m.Status));
    }

    /// <summary>
    /// Filtre les missions actives (non annulées, non archivées).
    /// </summary>
    public static IQueryable<Mission> Active(this IQueryable<Mission> query)
    {
        return query.Where(m => m.Status != MissionStatus.Cancelled && m.Status != MissionStatus.Archived);
    }

    /// <summary>
    /// Filtre les missions planifiées pour une période donnée.
    /// </summary>
    public static IQueryable<Mission> ScheduledBetween(this IQueryable<Mission> query, DateTime startDate, DateTime endDate)
    {
        return query.Where(m => m.ScheduledDate.HasValue &&
                               m.ScheduledDate.Value.Date >= startDate.Date &&
                               m.ScheduledDate.Value.Date <= endDate.Date);
    }

    /// <summary>
    /// Filtre les missions planifiées aujourd'hui.
    /// </summary>
    public static IQueryable<Mission> ScheduledToday(this IQueryable<Mission> query)
    {
        var today = DateTime.Today;
        return query.Where(m => m.ScheduledDate.HasValue && m.ScheduledDate.Value.Date == today);
    }

    /// <summary>
    /// Filtre les missions en retard.
    /// </summary>
    public static IQueryable<Mission> Overdue(this IQueryable<Mission> query)
    {
        var today = DateTime.Today;
        return query.Where(m => m.ScheduledDate.HasValue &&
                               m.ScheduledDate.Value.Date < today &&
                               m.Status != MissionStatus.Completed &&
                               m.Status != MissionStatus.Cancelled &&
                               m.Status != MissionStatus.Archived);
    }

    /// <summary>
    /// Filtre les missions par client.
    /// </summary>
    public static IQueryable<Mission> ForClient(this IQueryable<Mission> query, Guid clientId)
    {
        return query.Where(m => m.ClientId == clientId);
    }

    /// <summary>
    /// Filtre les missions par bien immobilier.
    /// </summary>
    public static IQueryable<Mission> ForProperty(this IQueryable<Mission> query, Guid propertyId)
    {
        return query.Where(m => m.PropertyId == propertyId);
    }

    /// <summary>
    /// Filtre les missions assignées à un diagnostiqueur.
    /// </summary>
    public static IQueryable<Mission> AssignedTo(this IQueryable<Mission> query, Guid diagnosticianId)
    {
        return query.Where(m => m.AssignedDiagnosticianId == diagnosticianId);
    }

    /// <summary>
    /// Filtre les missions non assignées.
    /// </summary>
    public static IQueryable<Mission> Unassigned(this IQueryable<Mission> query)
    {
        return query.Where(m => !m.AssignedDiagnosticianId.HasValue);
    }

    /// <summary>
    /// Inclut les relations de navigation standard.
    /// </summary>
    public static IQueryable<Mission> IncludeStandard(this IQueryable<Mission> query)
    {
        return query
            .Include(m => m.Client)
            .Include(m => m.Property)
            .Include(m => m.AssignedDiagnostician)
            .Include(m => m.Diagnostics);
    }

    /// <summary>
    /// Tri par date de mission (décroissant par défaut).
    /// </summary>
    public static IQueryable<Mission> OrderByMissionDate(this IQueryable<Mission> query, bool ascending = false)
    {
        return ascending
            ? query.OrderBy(m => m.MissionDate)
            : query.OrderByDescending(m => m.MissionDate);
    }

    /// <summary>
    /// Tri par date planifiée (croissant par défaut).
    /// </summary>
    public static IQueryable<Mission> OrderByScheduledDate(this IQueryable<Mission> query, bool descending = false)
    {
        return descending
            ? query.OrderByDescending(m => m.ScheduledDate)
            : query.OrderBy(m => m.ScheduledDate);
    }

    /// <summary>
    /// Recherche par numéro de mission.
    /// </summary>
    public static IQueryable<Mission> SearchByNumber(this IQueryable<Mission> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        return query.Where(m => m.MissionNumber.Contains(searchTerm));
    }

    /// <summary>
    /// Recherche par nom de client.
    /// </summary>
    public static IQueryable<Mission> SearchByClientName(this IQueryable<Mission> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.ToLower();

        return query.Where(m =>
            m.Client != null &&
            (
                (m.Client.LastName != null && m.Client.LastName.ToLower().Contains(searchTerm)) ||
                (m.Client.FirstName != null && m.Client.FirstName.ToLower().Contains(searchTerm))
            )
        );
    }

    /// <summary>
    /// Recherche globale (numéro, client).
    /// </summary>
    public static IQueryable<Mission> Search(this IQueryable<Mission> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.ToLower();

        return query.Where(m =>
            (m.MissionNumber != null && m.MissionNumber.ToLower().Contains(searchTerm)) ||
            (m.Client != null &&
                (
                    (m.Client.LastName != null && m.Client.LastName.ToLower().Contains(searchTerm)) ||
                    (m.Client.FirstName != null && m.Client.FirstName.ToLower().Contains(searchTerm))
                )
            )
        );
    }

}

/// <summary>
/// Extensions LINQ pour faciliter les requêtes sur les diagnostics.
/// </summary>
public static class DiagnosticQueryExtensions
{
    /// <summary>
    /// Filtre les diagnostics par type.
    /// </summary>
    public static IQueryable<Diagnostic> OfType(this IQueryable<Diagnostic> query, DiagnosticType type)
    {
        return query.Where(d => d.DiagnosticType == type);
    }

    /// <summary>
    /// Filtre les diagnostics par types multiples.
    /// </summary>
    public static IQueryable<Diagnostic> OfTypes(this IQueryable<Diagnostic> query, params DiagnosticType[] types)
    {
        return query.Where(d => types.Contains(d.DiagnosticType));
    }

    /// <summary>
    /// Filtre les diagnostics par statut.
    /// </summary>
    public static IQueryable<Diagnostic> WithStatus(this IQueryable<Diagnostic> query, MissionStatus status)
    {
        return query.Where(d => d.Status == status);
    }

    /// <summary>
    /// Filtre les diagnostics expirés.
    /// </summary>
    public static IQueryable<Diagnostic> Expired(this IQueryable<Diagnostic> query)
    {
        var today = DateTime.Today;
        return query.Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value.Date < today);
    }

    /// <summary>
    /// Filtre les diagnostics expirant bientôt.
    /// </summary>
    public static IQueryable<Diagnostic> ExpiringSoon(this IQueryable<Diagnostic> query, int daysThreshold = 30)
    {
        var today = DateTime.Today;
        var thresholdDate = today.AddDays(daysThreshold);
        return query.Where(d => d.ExpiryDate.HasValue &&
                               d.ExpiryDate.Value.Date > today &&
                               d.ExpiryDate.Value.Date <= thresholdDate);
    }

    /// <summary>
    /// Filtre les diagnostics avec anomalies.
    /// </summary>
    public static IQueryable<Diagnostic> WithAnomalies(this IQueryable<Diagnostic> query)
    {
        return query.Where(d => d.HasAnomalies);
    }

    /// <summary>
    /// Filtre les diagnostics non conformes.
    /// </summary>
    public static IQueryable<Diagnostic> NonCompliant(this IQueryable<Diagnostic> query)
    {
        return query.Where(d => !d.IsCompliant);
    }

    /// <summary>
    /// Filtre les diagnostics par mission.
    /// </summary>
    public static IQueryable<Diagnostic> ForMission(this IQueryable<Diagnostic> query, Guid missionId)
    {
        return query.Where(d => d.MissionId == missionId);
    }

    /// <summary>
    /// Filtre les diagnostics par diagnostiqueur.
    /// </summary>
    public static IQueryable<Diagnostic> ByDiagnostician(this IQueryable<Diagnostic> query, Guid diagnosticianId)
    {
        return query.Where(d => d.DiagnosticianId == diagnosticianId);
    }

    /// <summary>
    /// Inclut les relations de navigation standard.
    /// </summary>
    public static IQueryable<Diagnostic> IncludeStandard(this IQueryable<Diagnostic> query)
    {
        return query
            .Include(d => d.Mission)
                .ThenInclude(m => m.Client)
            .Include(d => d.Mission)
                .ThenInclude(m => m.Property)
            .Include(d => d.Diagnostician);
    }

    /// <summary>
    /// Tri par date de diagnostic (décroissant par défaut).
    /// </summary>
    public static IQueryable<Diagnostic> OrderByDiagnosticDate(this IQueryable<Diagnostic> query, bool ascending = false)
    {
        return ascending
            ? query.OrderBy(d => d.DiagnosticDate)
            : query.OrderByDescending(d => d.DiagnosticDate);
    }

    /// <summary>
    /// Tri par date d'expiration (croissant par défaut).
    /// </summary>
    public static IQueryable<Diagnostic> OrderByExpiryDate(this IQueryable<Diagnostic> query, bool descending = false)
    {
        return descending
            ? query.OrderByDescending(d => d.ExpiryDate)
            : query.OrderBy(d => d.ExpiryDate);
    }
}

/// <summary>
/// Extensions LINQ pour faciliter les requêtes sur les clients.
/// </summary>
public static class ClientQueryExtensions
{
    /// <summary>
    /// Filtre les clients par entreprise.
    /// </summary>
    public static IQueryable<Client> ForCompany(this IQueryable<Client> query, Guid companyId)
    {
        return query.Where(c => c.CompanyId == companyId);
    }

    /// <summary>
    /// Filtre les clients par type.
    /// </summary>
    public static IQueryable<Client> OfType(this IQueryable<Client> query, ClientType type)
    {
        return query.Where(c => c.ClientType == type);
    }

    /// <summary>
    /// Filtre les clients VIP.
    /// </summary>
    public static IQueryable<Client> VipOnly(this IQueryable<Client> query)
    {
        return query.Where(c => c.IsVip);
    }

    /// <summary>
    /// Filtre les clients acceptant le marketing.
    /// </summary>
    public static IQueryable<Client> AcceptsMarketing(this IQueryable<Client> query)
    {
        return query.Where(c => c.AcceptsMarketing);
    }

    /// <summary>
    /// Recherche par nom (nom ou prénom).
    /// </summary>
    public static IQueryable<Client> SearchByName(this IQueryable<Client> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.ToLower();
        return query.Where(c => c.LastName.ToLower().Contains(searchTerm) ||
                               (c.FirstName != null && c.FirstName.ToLower().Contains(searchTerm)));
    }

    /// <summary>
    /// Recherche par email.
    /// </summary>
    public static IQueryable<Client> SearchByEmail(this IQueryable<Client> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.ToLower();
        return query.Where(c => c.Email.ToLower().Contains(searchTerm));
    }

    /// <summary>
    /// Inclut les missions du client.
    /// </summary>
    public static IQueryable<Client> IncludeMissions(this IQueryable<Client> query)
    {
        return query.Include(c => c.Missions);
    }

    /// <summary>
    /// Tri par nom (croissant par défaut).
    /// </summary>
    public static IQueryable<Client> OrderByName(this IQueryable<Client> query, bool descending = false)
    {
        return descending
            ? query.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName)
            : query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);
    }
}

/// <summary>
/// Extensions LINQ pour faciliter les requêtes sur les biens immobiliers.
/// </summary>
public static class PropertyQueryExtensions
{
    /// <summary>
    /// Filtre les biens par entreprise.
    /// </summary>
    public static IQueryable<Property> ForCompany(this IQueryable<Property> query, Guid companyId)
    {
        return query.Where(p => p.CompanyId == companyId);
    }

    /// <summary>
    /// Filtre les biens par type.
    /// </summary>
    public static IQueryable<Property> OfType(this IQueryable<Property> query, PropertyType type)
    {
        return query.Where(p => p.PropertyType == type);
    }

    /// <summary>
    /// Filtre les biens par usage.
    /// </summary>
    public static IQueryable<Property> WithUsage(this IQueryable<Property> query, PropertyUsage usage)
    {
        return query.Where(p => p.PropertyUsage == usage);
    }

    /// <summary>
    /// Filtre les biens par type de transaction.
    /// </summary>
    public static IQueryable<Property> ForTransaction(this IQueryable<Property> query, TransactionType transaction)
    {
        return query.Where(p => p.TransactionType == transaction);
    }

    /// <summary>
    /// Recherche par référence.
    /// </summary>
    public static IQueryable<Property> SearchByReference(this IQueryable<Property> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.ToLower();
        return query.Where(p => p.Reference != null && p.Reference.ToLower().Contains(searchTerm));
    }

    /// <summary>
    /// Inclut les missions du bien.
    /// </summary>
    public static IQueryable<Property> IncludeMissions(this IQueryable<Property> query)
    {
        return query.Include(p => p.Missions);
    }

    /// <summary>
    /// Tri par date de création (décroissant par défaut).
    /// </summary>
    public static IQueryable<Property> OrderByCreatedDate(this IQueryable<Property> query, bool ascending = false)
    {
        return ascending
            ? query.OrderBy(p => p.CreatedAt)
            : query.OrderByDescending(p => p.CreatedAt);
    }
}
