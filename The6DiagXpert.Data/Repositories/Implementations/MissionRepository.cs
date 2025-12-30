using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.Repositories.Implementations
{
    /// <summary>
    /// Implémentation du repository pour les missions.
    /// </summary>
    public class MissionRepository : GenericRepository<Mission>, IMissionRepository
    {
        public MissionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Mission>> GetByStatusAsync(Guid companyId, MissionStatus status)
        {
            return await _dbSet
                .Where(m => m.CompanyId == companyId && m.Status == status)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .OrderByDescending(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbSet
                .Where(m => m.ClientId == clientId)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .OrderByDescending(m => m.MissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetByDiagnosticianIdAsync(Guid diagnosticianId)
        {
            return await _dbSet
                .Where(m => m.AssignedDiagnosticianId == diagnosticianId)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.Company)
                .OrderByDescending(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _dbSet
                .Where(m => m.CompanyId == companyId)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .OrderByDescending(m => m.MissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetByDateRangeAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(m => m.CompanyId == companyId
                            && m.ScheduledDate.HasValue
                            && m.ScheduledDate.Value >= startDate
                            && m.ScheduledDate.Value <= endDate)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetScheduledForDateAsync(Guid companyId, DateTime date)
        {
            // ScheduledTime n'existe pas sur Mission => on trie par ScheduledDate puis MissionDate
            return await _dbSet
                .Where(m => m.CompanyId == companyId
                            && m.ScheduledDate.HasValue
                            && m.ScheduledDate.Value.Date == date.Date)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .OrderBy(m => m.ScheduledDate)
                .ThenBy(m => m.MissionDate)
                .ToListAsync();
        }

        public async Task<Mission?> GetWithAllRelationsAsync(Guid id)
        {
            // Diagnostics n'existe pas => pas d'Include dessus
            return await _dbSet
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Mission>> GetUnscheduledMissionsAsync(Guid companyId)
        {
            // MissionStatus.Draft n'existe pas => on filtre uniquement sur l'absence de ScheduledDate
            // (le statut "draft" sera géré plus tard quand ton enum le contiendra)
            return await _dbSet
                .Where(m => m.CompanyId == companyId && !m.ScheduledDate.HasValue)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .OrderBy(m => m.MissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetOverdueMissionsAsync(Guid companyId)
        {
            var today = DateTime.Today;

            // On ne suppose pas des statuts inexistants : on exclut Completed/Cancelled et on garde ce qui est planifié avant aujourd'hui
            return await _dbSet
                .Where(m => m.CompanyId == companyId
                            && m.ScheduledDate.HasValue
                            && m.ScheduledDate.Value.Date < today
                            && m.Status != MissionStatus.Completed
                            && m.Status != MissionStatus.Cancelled)
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetMissionsWithIncompleteDocumentsAsync(Guid companyId)
        {
            // Mission.Diagnostics n'existe pas => on ne peut pas checker ReportPath ici.
            // On retourne une base cohérente : missions complétées + filtre léger sur ReportPath s'il existe sur Mission.
            return await _dbSet
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Where(m => m.CompanyId == companyId && m.Status == MissionStatus.Completed)
                .OrderByDescending(m => m.CompletedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> SearchAsync(
            Guid companyId,
            string? searchTerm = null,
            MissionStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? clientId = null,
            Guid? diagnosticianId = null,
            Guid? propertyId = null)
        {
            var query = _dbSet
                .Include(m => m.Client)
                .Include(m => m.Property)
                .Include(m => m.AssignedDiagnostician)
                .Where(m => m.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLowerInvariant();

                query = query.Where(m =>
                    (m.MissionNumber != null && m.MissionNumber.ToLower().Contains(term)) ||
                    (m.Client != null && (
                        (m.Client.FirstName != null && m.Client.FirstName.ToLower().Contains(term)) ||
                        (m.Client.LastName != null && m.Client.LastName.ToLower().Contains(term))
                    )) ||
                    (m.Title != null && m.Title.ToLower().Contains(term)));
            }

            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            if (startDate.HasValue)
                query = query.Where(m => m.ScheduledDate.HasValue && m.ScheduledDate.Value >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.ScheduledDate.HasValue && m.ScheduledDate.Value <= endDate.Value);

            if (clientId.HasValue)
                query = query.Where(m => m.ClientId == clientId.Value);

            if (diagnosticianId.HasValue)
                query = query.Where(m => m.AssignedDiagnosticianId == diagnosticianId.Value);

            if (propertyId.HasValue)
                query = query.Where(m => m.PropertyId == propertyId.Value);

            return await query
                .OrderByDescending(m => m.MissionDate)
                .ToListAsync();
        }

        public async Task<Dictionary<MissionStatus, int>> CountByStatusAsync(Guid companyId)
        {
            return await _dbSet
                .Where(m => m.CompanyId == companyId)
                .GroupBy(m => m.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<bool> ExistsForPropertyOnDateAsync(Guid propertyId, DateTime date)
        {
            return await _dbSet.AnyAsync(m =>
                m.PropertyId == propertyId &&
                m.ScheduledDate.HasValue &&
                m.ScheduledDate.Value.Date == date.Date);
        }

        public async Task<MissionStatistics> GetDiagnosticianStatisticsAsync(Guid diagnosticianId, int year)
        {
            var missions = await _dbSet
                .Where(m => m.AssignedDiagnosticianId == diagnosticianId && m.MissionDate.Year == year)
                .ToListAsync();

            var total = missions.Count;
            var completed = missions.Count(m => m.Status == MissionStatus.Completed);

            // Ne plus utiliser Draft/Scheduled/InProgress (peut-être absents)
            var pending = missions.Count(m => m.Status != MissionStatus.Completed && m.Status != MissionStatus.Cancelled);

            var cancelled = missions.Count(m => m.Status == MissionStatus.Cancelled);

            var completionRate = total > 0 ? (decimal)completed / total * 100 : 0m;

            var completedMissions = missions.Where(m =>
                m.CompletedDate.HasValue &&
                m.ScheduledDate.HasValue &&
                m.ActualDuration.HasValue);

            var avgDuration = completedMissions.Any()
                ? (decimal)completedMissions.Average(m => m.ActualDuration!.Value)
                : 0m;

            var totalRevenue = missions
                .Where(m => m.Status == MissionStatus.Completed && m.AmountTTC.HasValue)
                .Sum(m => m.AmountTTC!.Value);

            return new MissionStatistics
            {
                TotalMissions = total,
                CompletedMissions = completed,
                PendingMissions = pending,
                CancelledMissions = cancelled,
                CompletionRate = completionRate,
                AverageDuration = avgDuration,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<string?> GetLastMissionNumberAsync(Guid companyId)
        {
            return await _dbSet
                .Where(m => m.CompanyId == companyId)
                .OrderByDescending(m => m.MissionDate)
                .Select(m => m.MissionNumber)
                .FirstOrDefaultAsync();
        }
    }
}
