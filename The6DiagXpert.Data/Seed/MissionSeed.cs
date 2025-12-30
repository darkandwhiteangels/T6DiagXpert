using System;
using System.Collections.Generic;
using The6DiagXpert.Core.Enums;
using The6DiagXpert.Core.Models.Missions;

namespace The6DiagXpert.Data.Seed;

public static class MissionSeed
{
    /// <summary>
    /// Génère des missions de test. 
    /// ⚠️ Les GUIDs doivent correspondre à des Clients/Properties existants en seed si tu relies par FK.
    /// </summary>
    public static List<Mission> GetMissions(Guid companyId, Guid clientId, Guid propertyId)
    {
        var now = DateTime.UtcNow;

        return new List<Mission>
        {
            new Mission
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CompanyId = companyId,
                ClientId = clientId,
                PropertyId = propertyId,

                MissionNumber = "M-000001",
                Status = MissionStatus.OnHold,
                TransactionType = TransactionType.Sale,

                MissionDate = now.Date,
                ScheduledDate = now.Date.AddDays(3).AddHours(9),

                VatRate = 20m,
                AmountHT = 180m,
                VatAmount = 36m,
                AmountTTC = 216m,

                EstimatedDuration = 120,
                CreatedAt = now,
                UpdatedAt = now
            },

            new Mission
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CompanyId = companyId,
                ClientId = clientId,
                PropertyId = propertyId,

                MissionNumber = "M-000002",
                Status = MissionStatus.InProgress,
                TransactionType = TransactionType.Rental,

                MissionDate = now.Date.AddDays(-1),
                ScheduledDate = now.Date.AddDays(1).AddHours(14).AddMinutes(30),

                VatRate = 20m,
                AmountHT = 220m,
                VatAmount = 44m,
                AmountTTC = 264m,

                EstimatedDuration = 150,
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddDays(-1)
            },

            new Mission
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CompanyId = companyId,
                ClientId = clientId,
                PropertyId = propertyId,

                MissionNumber = "M-000003",
                Status = MissionStatus.InProgress,
                TransactionType = TransactionType.Sale,

                MissionDate = now.Date,
                ScheduledDate = now.Date.AddHours(8).AddMinutes(45),

                VatRate = 20m,
                AmountHT = 310m,
                VatAmount = 62m,
                AmountTTC = 372m,

                EstimatedDuration = 210,
                ActualDuration = 45,
                CreatedAt = now.AddDays(-10),
                UpdatedAt = now
            },

            new Mission
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                CompanyId = companyId,
                ClientId = clientId,
                PropertyId = propertyId,

                MissionNumber = "M-000004",
                Status = MissionStatus.Completed,
                TransactionType = TransactionType.Sale,

                MissionDate = now.Date.AddDays(-20),
                ScheduledDate = now.Date.AddDays(-18).AddHours(8).AddMinutes(45),
                CompletedDate = now.Date.AddDays(-18),

                VatRate = 20m,
                AmountHT = 260m,
                VatAmount = 52m,
                AmountTTC = 312m,

                EstimatedDuration = 180,
                ActualDuration = 175,
                CreatedAt = now.AddDays(-25),
                UpdatedAt = now.AddDays(-18)
            }
        };
    }
}
