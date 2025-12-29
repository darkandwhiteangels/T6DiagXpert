namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Types de pièces (classification simple MVP).
/// </summary>
public enum RoomType
{
    Unknown = 0,
    LivingRoom = 10,
    Bedroom = 11,
    Kitchen = 12,
    Bathroom = 13,
    Toilet = 14,
    Hallway = 15,
    Office = 16,

    Garage = 30,
    Basement = 31,
    Attic = 32,
    TechnicalRoom = 33,

    Balcony = 50,
    Terrace = 51,
    Garden = 52,

    Other = 99
}
