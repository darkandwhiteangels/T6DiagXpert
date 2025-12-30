using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interface du repository pour les missions de diagnostic.
    /// </summary>
    public interface IMissionRepository : IGenericRepository<Mission>
    {
        Task<IEnumerable<Mission>> GetByStatusAsync(Guid companyId, MissionStatus status);
        Task<IEnumerable<Mission>> GetByClientIdAsync(Guid clientId);
        Task<IEnumerable<Mission>> GetByDiagnosticianIdAsync(Guid diagnosticianId);
        Task<IEnumerable<Mission>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<Mission>> GetByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Mission>> GetScheduledForDateAsync(Guid companyId, DateTime date);
        Task<Mission?> GetWithAllRelationsAsync(Guid id);
        Task<IEnumerable<Mission>> GetUnscheduledMissionsAsync(Guid companyId);
        Task<IEnumerable<Mission>> GetOverdueMissionsAsync(Guid companyId);
        Task<IEnumerable<Mission>> GetMissionsWithIncompleteDocumentsAsync(Guid companyId);

        Task<IEnumerable<Mission>> SearchAsync(
            Guid companyId,
            string? searchTerm = null,
            MissionStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? clientId = null,
            Guid? diagnosticianId = null,
            Guid? propertyId = null
        );

        Task<Dictionary<MissionStatus, int>> CountByStatusAsync(Guid companyId);
        Task<bool> ExistsForPropertyOnDateAsync(Guid propertyId, DateTime date);
        Task<MissionStatistics> GetDiagnosticianStatisticsAsync(Guid diagnosticianId, int year);
        Task<string?> GetLastMissionNumberAsync(Guid companyId);
    }

    /// <summary>
    /// Statistiques de missions.
    /// </summary>
    public class MissionStatistics
    {
        public int TotalMissions { get; set; }
        public int CompletedMissions { get; set; }
        public int PendingMissions { get; set; }
        public int CancelledMissions { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal AverageDuration { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
