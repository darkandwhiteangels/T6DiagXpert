using The6DiagXpert.Core.Enums;
using The6DiagXpert.Data.UnitOfWork;
using The6DiagXpert.Shared.DTOs.Missions;
using The6DiagXpert.Shared.Mappers.Missions;

namespace The6DiagXpert.Infrastructure.Services;

/// <summary>
/// Service métier pour la gestion des clients.
/// Aligné sur: DTO Id = string / Entity Id = Guid / Repos sans CancellationToken.
/// </summary>
public class ClientService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ClientDto> CreateClientAsync(ClientDto clientDto, CancellationToken cancellationToken = default)
    {
        if (clientDto == null) throw new ArgumentNullException(nameof(clientDto));

        // Validations (repo signatures: EmailExistsAsync(Guid companyId, string email, Guid? excludeId))
        await ValidateUniqueEmailAsync(clientDto.CompanyId, clientDto.Email, null);

        if (clientDto.ClientType == ClientType.Professional && !string.IsNullOrWhiteSpace(clientDto.SiretNumber))
            await ValidateUniqueSiretAsync(clientDto.CompanyId, clientDto.SiretNumber, null);

        var client = clientDto.ToEntity();

        await _unitOfWork.Clients.AddAsync(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return client.ToDto();
    }

    public async Task<ClientDto> UpdateClientAsync(ClientDto clientDto, CancellationToken cancellationToken = default)
    {
        if (clientDto == null) throw new ArgumentNullException(nameof(clientDto));

        var clientId = ParseGuidOrThrow(clientDto.Id, "Id client invalide");

        var existingClient = await _unitOfWork.Clients.GetByIdAsync(clientId)
            ?? throw new InvalidOperationException($"Client {clientDto.Id} introuvable.");

        await ValidateUniqueEmailAsync(existingClient.CompanyId, clientDto.Email, clientId);

        if (clientDto.ClientType == ClientType.Professional && !string.IsNullOrWhiteSpace(clientDto.SiretNumber))
            await ValidateUniqueSiretAsync(existingClient.CompanyId, clientDto.SiretNumber, clientId);

        existingClient.UpdateFromDto(clientDto);

        await _unitOfWork.Clients.UpdateAsync(existingClient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existingClient.ToDto();
    }

    public async Task DeleteClientAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Client {id} introuvable.");

        // Missions.ExistsAsync(predicate, includeDeleted=false) -> pas de token
        var hasMissions = await _unitOfWork.Missions.ExistsAsync(m => m.ClientId == id);
        if (hasMissions)
            throw new InvalidOperationException("Impossible de supprimer un client ayant des missions.");

        await _unitOfWork.Clients.SoftDeleteAsync(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ClientDto?> GetClientByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // IClientRepository: GetWithAllRelationsAsync(Guid id)
        var client = await _unitOfWork.Clients.GetWithAllRelationsAsync(id);
        return client?.ToDto();
    }

    public async Task<ClientDto?> GetClientByEmailAsync(Guid companyId, string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var client = await _unitOfWork.Clients.GetByEmailAsync(companyId, email);
        return client?.ToDto();
    }

    public async Task<ClientDto?> GetClientBySiretAsync(Guid companyId, string siretNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(siretNumber))
            return null;

        var client = await _unitOfWork.Clients.GetBySiretAsync(companyId, siretNumber);
        return client?.ToDto();
    }

    public async Task<IEnumerable<ClientDto>> GetAllClientsAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var clients = await _unitOfWork.Clients.GetByCompanyIdAsync(companyId);
        return clients.Select(c => c.ToDto());
    }

    public async Task<IEnumerable<ClientDto>> GetClientsByTypeAsync(Guid companyId, ClientType clientType, CancellationToken cancellationToken = default)
    {
        var clients = await _unitOfWork.Clients.GetByTypeAsync(companyId, clientType);
        return clients.Select(c => c.ToDto());
    }

    public async Task<IEnumerable<ClientDto>> SearchClientsAsync(Guid companyId, string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Array.Empty<ClientDto>();

        var clients = await _unitOfWork.Clients.SearchAsync(companyId, searchTerm);
        return clients.Select(c => c.ToDto());
    }

    #region Validations

    private async Task ValidateUniqueEmailAsync(Guid companyId, string? email, Guid? excludeClientId)
    {
        if (string.IsNullOrWhiteSpace(email))
            return;

        var exists = await _unitOfWork.Clients.EmailExistsAsync(companyId, email, excludeClientId);
        if (exists)
            throw new InvalidOperationException($"Un client avec l'email '{email}' existe déjà.");
    }

    private async Task ValidateUniqueSiretAsync(Guid companyId, string siretNumber, Guid? excludeClientId)
    {
        if (string.IsNullOrWhiteSpace(siretNumber))
            return;

        var exists = await _unitOfWork.Clients.SiretExistsAsync(companyId, siretNumber, excludeClientId);
        if (exists)
            throw new InvalidOperationException($"Un client avec le SIRET '{siretNumber}' existe déjà.");
    }

    private static Guid ParseGuidOrThrow(string value, string errorMessage)
        => Guid.TryParse(value, out var g) ? g : throw new InvalidOperationException(errorMessage);

    #endregion
}
