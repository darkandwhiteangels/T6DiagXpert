using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using The6DiagXpert.Core.Models.Plans;

namespace The6DiagXpert.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interface du repository pour la persistance des plans.
    /// </summary>
    public interface IPlanRepository : IGenericRepository<Plan>
    {
        Task<IEnumerable<Plan>> GetByMissionIdAsync(Guid missionId);
        Task<Plan?> GetByIdWithChildrenAsync(Guid planId);
        Task<bool> ExistsForMissionAsync(Guid missionId, int floorIndex, string name);
    }
}
