using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The6DiagXpert.Core.Models.Plans;
using The6DiagXpert.Data.Context;
using The6DiagXpert.Data.Repositories.Interfaces;

namespace The6DiagXpert.Data.Repositories.Implementations
{
    /// <summary>
    /// Implémentation du repository pour les plans.
    /// </summary>
    public class PlanRepository : GenericRepository<Plan>, IPlanRepository
    {
        public PlanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Plan>> GetByMissionIdAsync(Guid missionId)
        {
            return await _dbSet
                .Where(p => p.MissionId == missionId)
                .OrderBy(p => p.FloorIndex)
                .ThenBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Plan?> GetByIdWithChildrenAsync(Guid planId)
        {
            return await _dbSet
                .Where(p => p.Id == planId)
                .Include(p => p.Rooms)
                .Include(p => p.Objects)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsForMissionAsync(Guid missionId, int floorIndex, string name)
        {
            var normalizedName = (name ?? string.Empty).Trim();

            return await _dbSet
                .AnyAsync(p =>
                    p.MissionId == missionId &&
                    p.FloorIndex == floorIndex &&
                    p.Name == normalizedName
                );
        }
    }
}
