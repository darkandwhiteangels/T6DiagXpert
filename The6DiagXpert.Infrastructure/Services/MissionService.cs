using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;
using The6DiagXpert.Data.UnitOfWork;
using The6DiagXpert.Shared.DTOs.Missions;
using The6DiagXpert.Shared.Mappers.Missions;

namespace The6DiagXpert.Infrastructure.Services;

/// <summary>
/// Service métier pour la gestion des missions.
/// </summary>
public class MissionService
{
    private readonly IUnitOfWork _unitOfWork;

    public MissionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Crée une nouvelle mission.
    /// </summary>
    public async Task<MissionDto> CreateMissionAsync(MissionDto missionDto, CancellationToken cancellationToken = default)
    {
        if (missionDto == null) throw new ArgumentNullException(nameof(missionDto));
        if (missionDto.CompanyId == Guid.Empty) throw new InvalidOperationException("CompanyId obligatoire.");
        if (missionDto.ClientId == Guid.Empty) throw new InvalidOperationException("ClientId obligatoire.");
        if (missionDto.PropertyId == Guid.Empty) throw new InvalidOperationException("PropertyId obligatoire.");

        await ValidateClientExistsAsync(missionDto.ClientId);
        await ValidatePropertyExistsAsync(missionDto.PropertyId);

        if (string.IsNullOrWhiteSpace(missionDto.MissionNumber))
            missionDto.MissionNumber = await GenerateMissionNumberAsync(missionDto.CompanyId);

        // DTO -> Entity (mapper existant)
        var mission = missionDto.ToEntity();

        await _unitOfWork.Missions.AddAsync(mission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mission.ToDto();
    }

    /// <summary>
    /// Met à jour une mission existante.
    /// </summary>
    public async Task<MissionDto> UpdateMissionAsync(MissionDto missionDto, CancellationToken cancellationToken = default)
    {
        if (missionDto == null) throw new ArgumentNullException(nameof(missionDto));

        // BaseDto.Id = string -> on parse en Guid
        if (string.IsNullOrWhiteSpace(missionDto.Id))
            throw new InvalidOperationException("Id obligatoire.");

        if (!Guid.TryParse(missionDto.Id, out var missionId) || missionId == Guid.Empty)
            throw new InvalidOperationException("Id de mission invalide.");

        var existingMission = await _unitOfWork.Missions.GetByIdAsync(missionId)
            ?? throw new InvalidOperationException($"Mission {missionDto.Id} introuvable.");

        await ValidateClientExistsAsync(missionDto.ClientId);
        await ValidatePropertyExistsAsync(missionDto.PropertyId);

        // Update via mapper existant
        existingMission.UpdateFromDto(missionDto);

        await _unitOfWork.Missions.UpdateAsync(existingMission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existingMission.ToDto();
    }

    /// <summary>
    /// Supprime une mission (soft delete).
    /// </summary>
    public async Task DeleteMissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new InvalidOperationException("Id obligatoire.");

        var mission = await _unitOfWork.Missions.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Mission {id} introuvable.");

        await _unitOfWork.Missions.SoftDeleteAsync(mission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Obtient une mission complète par son ID.
    /// </summary>
    public async Task<MissionDto?> GetMissionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return null;

        var mission = await _unitOfWork.Missions.GetWithAllRelationsAsync(id);
        return mission?.ToDto();
    }

    /// <summary>
    /// Obtient les missions d'un diagnostiqueur (userId) via repository existant.
    /// </summary>
    public async Task<IEnumerable<MissionDto>> GetDiagnosticianMissionsAsync(Guid diagnosticianId, CancellationToken cancellationToken = default)
    {
        if (diagnosticianId == Guid.Empty) return Enumerable.Empty<MissionDto>();

        var missions = await _unitOfWork.Missions.GetByDiagnosticianIdAsync(diagnosticianId);
        return missions.Select(m => m.ToDto());
    }

    /// <summary>
    /// Obtient les missions par statut pour une société.
    /// </summary>
    public async Task<IEnumerable<MissionDto>> GetMissionsByStatusAsync(Guid companyId, MissionStatus status, CancellationToken cancellationToken = default)
    {
        if (companyId == Guid.Empty) return Enumerable.Empty<MissionDto>();

        var missions = await _unitOfWork.Missions.GetByStatusAsync(companyId, status);
        return missions.Select(m => m.ToDto());
    }

    /// <summary>
    /// Obtient les missions en retard pour une société.
    /// </summary>
    public async Task<IEnumerable<MissionDto>> GetOverdueMissionsAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        if (companyId == Guid.Empty) return Enumerable.Empty<MissionDto>();

        var missions = await _unitOfWork.Missions.GetOverdueMissionsAsync(companyId);
        return missions.Select(m => m.ToDto());
    }

    /// <summary>
    /// Recherche simple (fallback) via Query() générique.
    /// </summary>
    public async Task<IEnumerable<MissionDto>> SearchMissionsAsync(Guid companyId, string searchTerm, CancellationToken cancellationToken = default)
    {
        if (companyId == Guid.Empty) return Enumerable.Empty<MissionDto>();
        if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<MissionDto>();

        searchTerm = searchTerm.Trim();

        // Pas de SearchAsync dans ton repo -> on utilise Query
        var query = _unitOfWork.Missions.Query()
            .Where(m => m.CompanyId == companyId)
            .Where(m =>
                (m.MissionNumber != null && m.MissionNumber.Contains(searchTerm)) ||
                (m.Title != null && m.Title.Contains(searchTerm)) ||
                (m.Description != null && m.Description.Contains(searchTerm)));

        // matérialisation
        var results = query.ToList();
        return results.Select(m => m.ToDto());
    }

    /// <summary>
    /// Change le statut d'une mission.
    /// </summary>
    public async Task<MissionDto> ChangeStatusAsync(Guid id, MissionStatus newStatus, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new InvalidOperationException("Id obligatoire.");

        var mission = await _unitOfWork.Missions.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Mission {id} introuvable.");

        mission.Status = newStatus;

        if (newStatus == MissionStatus.Completed)
            mission.CompletedDate = DateTime.UtcNow;

        await _unitOfWork.Missions.UpdateAsync(mission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mission.ToDto();
    }

    /// <summary>
    /// Stats par statut (repo existant).
    /// </summary>
    public async Task<Dictionary<MissionStatus, int>> GetStatisticsAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        if (companyId == Guid.Empty) return new Dictionary<MissionStatus, int>();

        return await _unitOfWork.Missions.CountByStatusAsync(companyId);
    }

    #region Privates

    private async Task<string> GenerateMissionNumberAsync(Guid companyId)
    {
        var lastNumber = await _unitOfWork.Missions.GetLastMissionNumberAsync(companyId);

        var yearMonth = DateTime.UtcNow.ToString("yyyyMM");

        if (string.IsNullOrWhiteSpace(lastNumber))
            return $"M{yearMonth}-0001";

        // attendu: "MYYYYMM-0001"
        var parts = lastNumber.Split('-');
        var lastYearMonth = parts.Length > 0 && parts[0].Length >= 7 ? parts[0].Substring(1) : "";

        if (yearMonth == lastYearMonth && parts.Length > 1 && int.TryParse(parts[1], out var seq))
            return $"M{yearMonth}-{(seq + 1):D4}";

        return $"M{yearMonth}-0001";
    }

    private async Task ValidateClientExistsAsync(Guid clientId)
    {
        var exists = await _unitOfWork.Clients.ExistsAsync(c => c.Id == clientId);
        if (!exists) throw new InvalidOperationException($"Client {clientId} introuvable.");
    }

    private async Task ValidatePropertyExistsAsync(Guid propertyId)
    {
        var exists = await _unitOfWork.Properties.ExistsAsync(p => p.Id == propertyId);
        if (!exists) throw new InvalidOperationException($"Propriété {propertyId} introuvable.");
    }

    #endregion
}
