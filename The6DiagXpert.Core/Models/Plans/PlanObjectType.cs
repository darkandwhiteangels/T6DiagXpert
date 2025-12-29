namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Types d'objets posables sur un plan (bibliothèque de symboles).
/// </summary>
public enum PlanObjectType
{
    Unknown = 0,

    // Electricité
    Outlet = 10,
    Switch = 11,
    LightPoint = 12,
    ElectricalPanel = 13,
    JunctionBox = 14,

    // Chauffage / Ventilation
    Radiator = 30,
    Vmc = 31,

    // Gaz / Eau
    GasMeter = 50,
    Boiler = 51,
    WaterHeater = 52,
    WaterInlet = 53,
    WaterOutlet = 54,

    // Menuiseries / Ouvertures
    Door = 70,
    Window = 71,

    // Autres (MVP)
    SmokeDetector = 90,
    CoDetector = 91,
    Thermostat = 92
}
