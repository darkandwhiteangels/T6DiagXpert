using System;

namespace The6DiagXpert.Core.Models.Common;

/// <summary>
/// Représente une position géographique avec coordonnées GPS.
/// </summary>
public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public double? Altitude { get; set; }
    public double? Accuracy { get; set; }
    public DateTime? CapturedAt { get; set; }
    public string? Source { get; set; }

    public GeoLocation() { }

    public GeoLocation(double latitude, double longitude, double? altitude = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        CapturedAt = DateTime.UtcNow;
    }
}
