using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interface du repository pour les clients.
    /// </summary>
    public interface IClientRepository : IGenericRepository<Client>
    {
        /// <summary>
        /// Récupère un client par son email pour une entreprise.
        /// </summary>
        Task<Client?> GetByEmailAsync(Guid companyId, string email);

        /// <summary>
        /// Récupère un client par son numéro de téléphone.
        /// </summary>
        Task<Client?> GetByPhoneAsync(Guid companyId, string phone);

        /// <summary>
        /// Récupère un client par son numéro SIRET (pour professionnels).
        /// </summary>
        Task<Client?> GetBySiretAsync(Guid companyId, string siret);

        /// <summary>
        /// Récupère les clients par type pour une entreprise.
        /// </summary>
        Task<IEnumerable<Client>> GetByTypeAsync(Guid companyId, ClientType type);

        /// <summary>
        /// Récupère les clients d'une entreprise.
        /// </summary>
        Task<IEnumerable<Client>> GetByCompanyIdAsync(Guid companyId);

        /// <summary>
        /// Récupère un client avec toutes ses propriétés.
        /// </summary>
        Task<Client?> GetWithPropertiesAsync(Guid id);

        /// <summary>
        /// Récupère un client avec toutes ses missions.
        /// </summary>
        Task<Client?> GetWithMissionsAsync(Guid id);

        /// <summary>
        /// Récupère un client avec toutes ses relations (Properties + Missions).
        /// </summary>
        Task<Client?> GetWithAllRelationsAsync(Guid id);

        /// <summary>
        /// Recherche de clients par nom, email, téléphone ou entreprise.
        /// </summary>
        Task<IEnumerable<Client>> SearchAsync(Guid companyId, string searchTerm);

        /// <summary>
        /// Récupère les clients actifs (ayant des missions récentes).
        /// </summary>
        Task<IEnumerable<Client>> GetActiveClientsAsync(Guid companyId, int monthsRange = 12);

        /// <summary>
        /// Récupère les clients inactifs (sans missions récentes).
        /// </summary>
        Task<IEnumerable<Client>> GetInactiveClientsAsync(Guid companyId, int monthsRange = 12);

        /// <summary>
        /// Récupère les clients VIP.
        /// </summary>
        Task<IEnumerable<Client>> GetVipClientsAsync(Guid companyId);

        /// <summary>
        /// Vérifie si un email existe déjà pour une entreprise.
        /// </summary>
        Task<bool> EmailExistsAsync(Guid companyId, string email, Guid? excludeClientId = null);

        /// <summary>
        /// Vérifie si un numéro de téléphone existe déjà.
        /// </summary>
        Task<bool> PhoneExistsAsync(Guid companyId, string phone, Guid? excludeClientId = null);

        /// <summary>
        /// Vérifie si un SIRET existe déjà.
        /// </summary>
        Task<bool> SiretExistsAsync(Guid companyId, string siret, Guid? excludeClientId = null);

        /// <summary>
        /// Récupère les statistiques d'un client.
        /// </summary>
        Task<ClientStatistics> GetClientStatisticsAsync(Guid clientId);

        /// <summary>
        /// Récupère les clients avec le nombre de propriétés et missions.
        /// </summary>
        Task<IEnumerable<ClientSummary>> GetClientsSummaryAsync(Guid companyId);
    }

    /// <summary>
    /// Statistiques client.
    /// </summary>
    public class ClientStatistics
    {
        public int TotalProperties { get; set; }
        public int TotalMissions { get; set; }
        public int CompletedMissions { get; set; }
        public int PendingMissions { get; set; }
        public DateTime? LastMissionDate { get; set; }
        public decimal TotalSpent { get; set; }
    }

    /// <summary>
    /// Résumé client pour listings.
    /// </summary>
    public class ClientSummary
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public ClientType ClientType { get; set; }
        public bool IsVip { get; set; }
        public int PropertiesCount { get; set; }
        public int MissionsCount { get; set; }
        public DateTime? LastMissionDate { get; set; }
    }
}