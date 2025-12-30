using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interface du repository pour les propriétés/biens immobiliers.
    /// </summary>
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        /// <summary>
        /// Récupère les propriétés d'un client.
        /// </summary>
        Task<IEnumerable<Property>> GetByClientIdAsync(Guid clientId);

        /// <summary>
        /// Récupère les propriétés d'une entreprise.
        /// </summary>
        Task<IEnumerable<Property>> GetByCompanyIdAsync(Guid companyId);

        /// <summary>
        /// Récupère les propriétés par type.
        /// </summary>
        Task<IEnumerable<Property>> GetByTypeAsync(Guid companyId, PropertyType type);

        /// <summary>
        /// Récupère les propriétés par usage.
        /// </summary>
        Task<IEnumerable<Property>> GetByUsageAsync(Guid companyId, PropertyUsage usage);

        /// <summary>
        /// Récupère une propriété avec toutes ses missions.
        /// </summary>
        Task<Property?> GetWithMissionsAsync(Guid id);

        /// <summary>
        /// Récupère une propriété avec toutes ses relations (Client + Missions).
        /// </summary>
        Task<Property?> GetWithAllRelationsAsync(Guid id);

        /// <summary>
        /// Recherche de propriétés par adresse, référence ou ville.
        /// </summary>
        Task<IEnumerable<Property>> SearchAsync(Guid companyId, string searchTerm);

        /// <summary>
        /// Récupère les propriétés dans une zone géographique.
        /// </summary>
        Task<IEnumerable<Property>> GetByLocationAsync(Guid companyId, string city, string? postalCode = null);

        /// <summary>
        /// Récupère les propriétés nécessitant un diagnostic de renouvellement.
        /// </summary>
        Task<IEnumerable<Property>> GetPropertiesNeedingRenewalAsync(Guid companyId, int monthsBeforeExpiry = 3);

        /// <summary>
        /// Vérifie si une référence cadastrale existe déjà.
        /// </summary>
        Task<bool> ReferenceExistsAsync(Guid companyId, string reference, Guid? excludePropertyId = null);

        /// <summary>
        /// Compte les propriétés par type pour une entreprise.
        /// </summary>
        Task<Dictionary<PropertyType, int>> CountByTypeAsync(Guid companyId);

        /// <summary>
        /// Compte les propriétés par usage pour une entreprise.
        /// </summary>
        Task<Dictionary<PropertyUsage, int>> CountByUsageAsync(Guid companyId);

        /// <summary>
        /// Récupère les statistiques d'une propriété.
        /// </summary>
        Task<PropertyStatistics> GetPropertyStatisticsAsync(Guid propertyId);
    }

    /// <summary>
    /// Statistiques de propriété.
    /// </summary>
    public class PropertyStatistics
    {
        public int TotalMissions { get; set; }
        public DateTime? LastMissionDate { get; set; }
        public DateTime? NextScheduledMission { get; set; }
        public int ActiveDiagnostics { get; set; }
        public int ExpiredDiagnostics { get; set; }
        public int ExpiringSoonDiagnostics { get; set; }
    }
}