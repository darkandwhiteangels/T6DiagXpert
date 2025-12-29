using System;
using System.Collections.Generic;
using System.Linq;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Exceptions;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Core.Validators;

public sealed class MissionValidator
{
    public void ValidateForCreate(Mission mission)
    {
        if (mission == null) throw new ArgumentNullException(nameof(mission));

        var errors = new List<string>();

        if (mission.CompanyId == Guid.Empty) errors.Add("CompanyId est obligatoire.");
        if (mission.ClientId == Guid.Empty) errors.Add("ClientId est obligatoire.");
        if (mission.PropertyId == Guid.Empty) errors.Add("PropertyId est obligatoire.");

        if (string.IsNullOrWhiteSpace(mission.MissionNumber))
            errors.Add("MissionNumber est obligatoire.");

        if (mission.MissionDate == default)
            errors.Add("MissionDate est invalide.");

        if (mission.VatRate < 0 || mission.VatRate > 100)
            errors.Add("VatRate doit être compris entre 0 et 100.");

        if (mission.EstimatedDuration.HasValue && mission.EstimatedDuration.Value < 0)
            errors.Add("EstimatedDuration ne peut pas être négative.");

        if (mission.ActualDuration.HasValue && mission.ActualDuration.Value < 0)
            errors.Add("ActualDuration ne peut pas être négative.");

        ValidateDatesConsistency(mission, errors);
        ValidateAmountsConsistency(mission, errors);

        if (errors.Any())
            throw new ValidationException(string.Join(" | ", errors));
    }

    public void ValidateForUpdate(Mission mission)
    {
        if (mission == null) throw new ArgumentNullException(nameof(mission));
        if (mission.Id == Guid.Empty) throw new ValidationException("Id est obligatoire pour une mise à jour.");

        // mêmes règles que Create, mais on tolère MissionNumber si déjà en base
        var errors = new List<string>();

        if (mission.CompanyId == Guid.Empty) errors.Add("CompanyId est obligatoire.");
        if (mission.ClientId == Guid.Empty) errors.Add("ClientId est obligatoire.");
        if (mission.PropertyId == Guid.Empty) errors.Add("PropertyId est obligatoire.");

        if (mission.VatRate < 0 || mission.VatRate > 100)
            errors.Add("VatRate doit être compris entre 0 et 100.");

        ValidateDatesConsistency(mission, errors);
        ValidateAmountsConsistency(mission, errors);

        if (errors.Any())
            throw new ValidationException(string.Join(" | ", errors));
    }

    public void ValidateStatusTransition(MissionStatus from, MissionStatus to)
    {
        // Transition simple et robuste (tu affineras quand tu auras les use cases Phase 10+)
        if (from == to) return;

        // États terminaux
        if (from is MissionStatus.Completed or MissionStatus.Cancelled)
            throw new ValidationException($"Transition interdite : {from} est un état terminal.");

        // Règles usuelles
        if (to == MissionStatus.Completed && from == MissionStatus.OnHold)
            throw new ValidationException("Impossible de passer de Draft à Completed directement.");

        // Cancel possible depuis presque tout sauf Completed
        if (to == MissionStatus.Cancelled && from == MissionStatus.Completed)
            throw new ValidationException("Impossible d’annuler une mission Completed.");
    }

    private static void ValidateDatesConsistency(Mission mission, List<string> errors)
    {
        if (mission.ScheduledDate.HasValue && mission.CompletedDate.HasValue)
        {
            if (mission.CompletedDate.Value < mission.ScheduledDate.Value)
                errors.Add("CompletedDate ne peut pas être antérieure à ScheduledDate.");
        }

        if (mission.ExpiryDate.HasValue && mission.MissionDate != default)
        {
            if (mission.ExpiryDate.Value < mission.MissionDate)
                errors.Add("ExpiryDate ne peut pas être antérieure à MissionDate.");
        }
    }

    private static void ValidateAmountsConsistency(Mission mission, List<string> errors)
    {
        // si rien n’est renseigné -> ok
        if (!mission.AmountHT.HasValue && !mission.AmountTTC.HasValue && !mission.VatAmount.HasValue)
            return;

        if (mission.AmountHT.HasValue && mission.AmountHT.Value < 0) errors.Add("AmountHT ne peut pas être négatif.");
        if (mission.AmountTTC.HasValue && mission.AmountTTC.Value < 0) errors.Add("AmountTTC ne peut pas être négatif.");
        if (mission.VatAmount.HasValue && mission.VatAmount.Value < 0) errors.Add("VatAmount ne peut pas être négatif.");

        // cohérence TTC = HT + TVA (tolérance 0,02)
        if (mission.AmountHT.HasValue && mission.VatAmount.HasValue && mission.AmountTTC.HasValue)
        {
            var expected = mission.AmountHT.Value + mission.VatAmount.Value;
            var delta = Math.Abs(expected - mission.AmountTTC.Value);

            if (delta > 0.02m)
                errors.Add("Incohérence montants : AmountTTC devrait être proche de AmountHT + VatAmount.");
        }
    }
}
