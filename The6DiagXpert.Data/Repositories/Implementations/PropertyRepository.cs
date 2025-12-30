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
    /// Implémentation du repository pour les propriétés.
    /// </summary>
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        private new readonly AppDbContext _context;

        public PropertyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbSet
                .Where(p => p.ClientId == clientId)
                .Include(p => p.Missions)
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _dbSet
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Client)
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByTypeAsync(Guid companyId, PropertyType type)
        {
            return await _dbSet
                .Where(p => p.CompanyId == companyId && p.PropertyType == type)
                .Include(p => p.Client)
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByUsageAsync(Guid companyId, PropertyUsage usage)
        {
            return await _dbSet
                .Where(p => p.CompanyId == companyId && p.PropertyUsage == usage)
                .Include(p => p.Client)
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<Property?> GetWithMissionsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Missions)
                    .ThenInclude(m => m.AssignedDiagnostician)
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property?> GetWithAllRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Client)
                .Include(p => p.Company)
                .Include(p => p.Missions)
                    .ThenInclude(m => m.AssignedDiagnostician)
                // ❌ plus de ThenInclude(m => m.Diagnostics) car Mission n’a pas la nav
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> SearchAsync(Guid companyId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetByCompanyIdAsync(companyId);

            searchTerm = searchTerm.Trim();
            var like = $"%{searchTerm}%";

            return await _dbSet
                .Include(p => p.Client)
                .Where(p => p.CompanyId == companyId &&
                    (
                        (p.Reference != null && EF.Functions.Like(p.Reference, like)) ||
                        EF.Functions.Like(p.AddressJson, like)
                    ))
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByLocationAsync(Guid companyId, string city, string? postalCode = null)
        {
            if (string.IsNullOrWhiteSpace(city))
                return await GetByCompanyIdAsync(companyId);

            city = city.Trim();
            var likeCity = $"%{city}%";

            var query = _dbSet
                .Include(p => p.Client)
                .Where(p => p.CompanyId == companyId &&
                            EF.Functions.Like(p.AddressJson, likeCity));

            if (!string.IsNullOrWhiteSpace(postalCode))
            {
                postalCode = postalCode.Trim();
                var likePostal = $"%{postalCode}%";
                query = query.Where(p => EF.Functions.Like(p.AddressJson, likePostal));
            }

            return await query
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        /// <summary>
        /// Propriétés ayant au moins un diagnostic expirant bientôt (via Diagnostics + Missions, sans navigation Mission.Diagnostics).
        /// </summary>
        public async Task<IEnumerable<Property>> GetPropertiesNeedingRenewalAsync(Guid companyId, int monthsBeforeExpiry = 3)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(monthsBeforeExpiry);

            // 1) Missions complétées de la company
            var completedMissions = _context.Missions
                .Where(m => m.CompanyId == companyId && m.Status == MissionStatus.Completed);

            // 2) Diagnostics expirant bientôt
            var expiringDiagnostics = _context.Diagnostics
                .Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value <= cutoffDate);

            // 3) PropertyIds concernés
            var propertyIds = await expiringDiagnostics
                .Join(completedMissions,
                      d => d.MissionId,
                      m => m.Id,
                      (d, m) => m.PropertyId)
                .Distinct()
                .ToListAsync();

            // 4) Charger les propriétés + relations utiles
            return await _dbSet
                .Include(p => p.Client)
                .Include(p => p.Missions)
                .Where(p => p.CompanyId == companyId && propertyIds.Contains(p.Id))
                .OrderBy(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<bool> ReferenceExistsAsync(Guid companyId, string reference, Guid? excludePropertyId = null)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return false;

            reference = reference.Trim();

            var query = _dbSet.Where(p =>
                p.CompanyId == companyId &&
                p.Reference != null &&
                p.Reference.Equals(reference, StringComparison.OrdinalIgnoreCase));

            if (excludePropertyId.HasValue)
                query = query.Where(p => p.Id != excludePropertyId.Value);

            return await query.AnyAsync();
        }

        public async Task<Dictionary<PropertyType, int>> CountByTypeAsync(Guid companyId)
        {
            return await _dbSet
                .Where(p => p.CompanyId == companyId)
                .GroupBy(p => p.PropertyType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }

        public async Task<Dictionary<PropertyUsage, int>> CountByUsageAsync(Guid companyId)
        {
            return await _dbSet
                .Where(p => p.CompanyId == companyId)
                .GroupBy(p => p.PropertyUsage)
                .Select(g => new { Usage = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Usage, x => x.Count);
        }

        public async Task<PropertyStatistics> GetPropertyStatisticsAsync(Guid propertyId)
        {
            var property = await _dbSet
                .Include(p => p.Missions)
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property == null)
                return new PropertyStatistics();

            var now = DateTime.UtcNow;

            var totalMissions = property.Missions.Count;

            var lastMissionDate = property.Missions.Any()
                ? property.Missions.Max(m => m.MissionDate)
                : (DateTime?)null;

            var nextScheduledMission = property.Missions
                .Where(m => m.Status == MissionStatus.Scheduled && m.ScheduledDate.HasValue)
                .OrderBy(m => m.ScheduledDate)
                .FirstOrDefault()
                ?.ScheduledDate;

            // Diagnostics sans navigation Mission.Diagnostics
            var allDiagnostics = await _context.Diagnostics
                .Join(_context.Missions.Where(m => m.PropertyId == propertyId && m.Status == MissionStatus.Completed),
                      d => d.MissionId,
                      m => m.Id,
                      (d, m) => d)
                .ToListAsync();

            var activeDiagnostics = allDiagnostics.Count(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value > now);
            var expiredDiagnostics = allDiagnostics.Count(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value <= now);
            var expiringSoonDiagnostics = allDiagnostics.Count(d =>
                d.ExpiryDate.HasValue &&
                d.ExpiryDate.Value > now &&
                d.ExpiryDate.Value <= now.AddMonths(3));

            return new PropertyStatistics
            {
                TotalMissions = totalMissions,
                LastMissionDate = lastMissionDate,
                NextScheduledMission = nextScheduledMission,
                ActiveDiagnostics = activeDiagnostics,
                ExpiredDiagnostics = expiredDiagnostics,
                ExpiringSoonDiagnostics = expiringSoonDiagnostics
            };
        }
    }
}
