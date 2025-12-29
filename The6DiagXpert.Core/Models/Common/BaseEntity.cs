using System;
using System.ComponentModel.DataAnnotations;

namespace The6DiagXpert.Core.Models.Common
{
    /// <summary>
    /// Classe de base abstraite pour toutes les entités du système.
    /// Fournit les propriétés communes d'identification, d'audit, de soft delete et de synchronisation.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identifiant unique de l'entité (local DB).
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Date et heure de création de l'entité (UTC).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date et heure de la dernière modification de l'entité (UTC).
        /// </summary>
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identifiant de l'utilisateur ayant créé l'entité.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant modifié l'entité.
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Soft delete : indique si l'entité est supprimée.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Date de suppression (soft delete) (UTC).
        /// </summary>
        public DateTime? DeletedAtUtc { get; set; }

        /// <summary>
        /// Version de concurrence / synchronisation.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Identifiant côté Firebase/Firestore (doc id) si synchronisé.
        /// </summary>
        public string? FirebaseId { get; set; }

        /// <summary>
        /// Date du dernier succès de synchronisation (UTC).
        /// </summary>
        public DateTime? LastSyncAtUtc { get; set; }

        // --- Aliases compatibles DTO/mappers (sans suffixe Utc) ---

        public DateTime CreatedAt
        {
            get => CreatedAtUtc;
            set => CreatedAtUtc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTime UpdatedAt
        {
            get => UpdatedAtUtc;
            set => UpdatedAtUtc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTime? DeletedAt
        {
            get => DeletedAtUtc;
            set => DeletedAtUtc = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        public void Touch()
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAtUtc = DateTime.UtcNow;
            Touch();
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAtUtc = null;
            Touch();
        }

        public void MarkAsModified()
        {
            UpdatedAtUtc = DateTime.UtcNow;
            Version++;
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted) return;
            IsDeleted = true;
            DeletedAtUtc = DateTime.UtcNow;
            MarkAsModified();
        }
    }
}
