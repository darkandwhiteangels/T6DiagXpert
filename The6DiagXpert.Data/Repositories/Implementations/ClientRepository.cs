using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.Repositories.Implementations
{
    /// <summary>
    /// Implémentation du repository pour les clients.
    /// </summary>
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(Context.AppDbContext context) : base(context)
        {
        }

        public Task<Client?> GetByEmailAsync(Guid companyId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Task.FromResult<Client?>(null);

            email = email.Trim();

            return _dbSet.FirstOrDefaultAsync(c =>
                c.CompanyId == companyId &&
                c.Email != null &&
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public Task<Client?> GetByPhoneAsync(Guid companyId, string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return Task.FromResult<Client?>(null);

            phone = phone.Trim();

            return _dbSet.FirstOrDefaultAsync(c =>
                c.CompanyId == companyId &&
                ((c.Phone != null && c.Phone == phone) || (c.MobilePhone != null && c.MobilePhone == phone)));
        }

        public Task<Client?> GetBySiretAsync(Guid companyId, string siret)
        {
            if (string.IsNullOrWhiteSpace(siret))
                return Task.FromResult<Client?>(null);

            siret = siret.Trim();

            return _dbSet.FirstOrDefaultAsync(c =>
                c.CompanyId == companyId &&
                c.SiretNumber == siret);
        }

        public async Task<IEnumerable<Client>> GetByTypeAsync(Guid companyId, ClientType type)
        {
            return await _dbSet
                .Where(c => c.CompanyId == companyId && c.ClientType == type)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _dbSet
                .Where(c => c.CompanyId == companyId)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        /// <summary>
        /// Dans ton modèle actuel, Client n'a pas de navigation "Properties".
        /// On conserve la signature (interface), mais on retourne juste le client.
        /// </summary>
        public Task<Client?> GetWithPropertiesAsync(Guid id)
        {
            return _dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Client?> GetWithMissionsAsync(Guid id)
        {
            return _dbSet
                .Include(c => c.Missions)
                    .ThenInclude(m => m.Property)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Dans ton modèle actuel, on inclut les relations réelles :
        /// Missions (+ Property), AssignedDiagnostician, Company.
        /// </summary>
        public Task<Client?> GetWithAllRelationsAsync(Guid id)
        {
            return _dbSet
                .Include(c => c.Missions)
                    .ThenInclude(m => m.Property)
                .Include(c => c.Missions)
                    .ThenInclude(m => m.AssignedDiagnostician)
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Client>> SearchAsync(Guid companyId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetByCompanyIdAsync(companyId);

            searchTerm = searchTerm.Trim();
            var like = $"%{searchTerm}%";

            // NOTE: Client.CompanyName n'existe pas dans ton modèle.
            // Pour un pro, "LastName" contient la raison sociale (voir ton Client.cs).
            return await _dbSet
                .Where(c => c.CompanyId == companyId &&
                    (
                        (c.FirstName != null && EF.Functions.Like(c.FirstName, like)) ||
                        (c.LastName != null && EF.Functions.Like(c.LastName, like)) ||
                        (c.Email != null && EF.Functions.Like(c.Email, like)) ||
                        (c.Phone != null && EF.Functions.Like(c.Phone, like)) ||
                        (c.MobilePhone != null && EF.Functions.Like(c.MobilePhone, like)) ||
                        (c.SiretNumber != null && EF.Functions.Like(c.SiretNumber, like))
                    ))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetActiveClientsAsync(Guid companyId, int monthsRange = 12)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(-monthsRange);

            return await _dbSet
                .Include(c => c.Missions)
                .Where(c => c.CompanyId == companyId &&
                            c.Missions.Any(m => m.MissionDate >= cutoffDate))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetInactiveClientsAsync(Guid companyId, int monthsRange = 12)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(-monthsRange);

            return await _dbSet
                .Include(c => c.Missions)
                .Where(c => c.CompanyId == companyId &&
                            (!c.Missions.Any() || c.Missions.All(m => m.MissionDate < cutoffDate)))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetVipClientsAsync(Guid companyId)
        {
            return await _dbSet
                .Where(c => c.CompanyId == companyId && c.IsVip)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(Guid companyId, string email, Guid? excludeClientId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim();

            var query = _dbSet.Where(c =>
                c.CompanyId == companyId &&
                c.Email != null &&
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (excludeClientId.HasValue)
                query = query.Where(c => c.Id != excludeClientId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> PhoneExistsAsync(Guid companyId, string phone, Guid? excludeClientId = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            phone = phone.Trim();

            var query = _dbSet.Where(c =>
                c.CompanyId == companyId &&
                ((c.Phone != null && c.Phone == phone) || (c.MobilePhone != null && c.MobilePhone == phone)));

            if (excludeClientId.HasValue)
                query = query.Where(c => c.Id != excludeClientId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> SiretExistsAsync(Guid companyId, string siret, Guid? excludeClientId = null)
        {
            if (string.IsNullOrWhiteSpace(siret))
                return false;

            siret = siret.Trim();

            var query = _dbSet.Where(c => c.CompanyId == companyId && c.SiretNumber == siret);

            if (excludeClientId.HasValue)
                query = query.Where(c => c.Id != excludeClientId.Value);

            return await query.AnyAsync();
        }

        public async Task<ClientStatistics> GetClientStatisticsAsync(Guid clientId)
        {
            var client = await _dbSet
                .Include(c => c.Missions)
                    .ThenInclude(m => m.Property)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
                return new ClientStatistics();

            var totalMissions = client.Missions.Count;
            var completedMissions = client.Missions.Count(m => m.Status == MissionStatus.Completed);

            // MissionStatus.Draft n'existe pas -> état initial = Created
            var pendingMissions = client.Missions.Count(m =>
                m.Status == MissionStatus.Created ||
                m.Status == MissionStatus.Assigned ||
                m.Status == MissionStatus.Scheduled ||
                m.Status == MissionStatus.InProgress ||
                m.Status == MissionStatus.FieldworkCompleted ||
                m.Status == MissionStatus.ReportInProgress ||
                m.Status == MissionStatus.PendingValidation);

            var lastMissionDate = client.Missions.Any()
                ? client.Missions.Max(m => m.MissionDate)
                : (DateTime?)null;

            // TotalSpent: on garde Completed uniquement
            var totalSpent = client.Missions
                .Where(m => m.Status == MissionStatus.Completed && m.AmountTTC.HasValue)
                .Sum(m => m.AmountTTC!.Value);

            // Pas de Client.Properties : on calcule les biens distincts via Missions.PropertyId
            var totalProperties = client.Missions
                .Where(m => m.PropertyId != Guid.Empty)
                .Select(m => m.PropertyId)
                .Distinct()
                .Count();

            return new ClientStatistics
            {
                TotalProperties = totalProperties,
                TotalMissions = totalMissions,
                CompletedMissions = completedMissions,
                PendingMissions = pendingMissions,
                LastMissionDate = lastMissionDate,
                TotalSpent = totalSpent
            };
        }

        public async Task<IEnumerable<ClientSummary>> GetClientsSummaryAsync(Guid companyId)
        {
            // Pas de navigation Properties -> on calcule les counts à partir des missions
            return await _dbSet
                .Where(c => c.CompanyId == companyId)
                .Include(c => c.Missions)
                .Select(c => new ClientSummary
                {
                    Id = c.Id,
                    FullName = (c.FirstName ?? string.Empty).Trim() == string.Empty
                        ? c.LastName
                        : (c.FirstName + " " + c.LastName),
                    Email = c.Email,
                    Phone = c.Phone ?? c.MobilePhone,
                    ClientType = c.ClientType,
                    IsVip = c.IsVip,
                    PropertiesCount = c.Missions.Select(m => m.PropertyId).Distinct().Count(),
                    MissionsCount = c.Missions.Count,
                    LastMissionDate = c.Missions.Any() ? c.Missions.Max(m => m.MissionDate) : (DateTime?)null
                })
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
    }
}
