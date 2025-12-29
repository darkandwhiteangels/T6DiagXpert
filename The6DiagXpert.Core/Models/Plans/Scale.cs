using System;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Échelle de calibration : un segment en pixels correspond à une distance réelle (mètres).
/// Permet conversions px <-> m.
/// </summary>
public sealed class Scale
{
    /// <summary>
    /// Longueur mesurée sur l'image (pixels).
    /// </summary>
    public double PixelLength { get; set; }

    /// <summary>
    /// Longueur réelle correspondante (mètres).
    /// </summary>
    public double RealLengthMeters { get; set; }

    /// <summary>
    /// Optionnel : point A de la calibration (pixels).
    /// </summary>
    public Position? PixelPointA { get; set; }

    /// <summary>
    /// Optionnel : point B de la calibration (pixels).
    /// </summary>
    public Position? PixelPointB { get; set; }

    public Scale() { }

    public Scale(double pixelLength, double realLengthMeters)
    {
        PixelLength = pixelLength;
        RealLengthMeters = realLengthMeters;
    }

    public bool IsValid => PixelLength > 0 && RealLengthMeters > 0;

    public double PixelsPerMeter => IsValid ? (PixelLength / RealLengthMeters) : 0d;

    public double MetersPerPixel => IsValid ? (RealLengthMeters / PixelLength) : 0d;

    public double ToMeters(double pixels)
    {
        if (!IsValid) return 0d;
        return pixels * MetersPerPixel;
    }

    public double ToPixels(double meters)
    {
        if (!IsValid) return 0d;
        return meters * PixelsPerMeter;
    }

    public override string ToString()
        => IsValid ? $"{PixelLength:0.###} px = {RealLengthMeters:0.###} m" : "Invalid scale";
}
