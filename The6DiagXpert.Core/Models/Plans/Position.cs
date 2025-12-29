using System;

namespace The6DiagXpert.Core.Models.Plans;

/// <summary>
/// Position 2D en coordonnées "plan" (généralement pixels sur le canvas importé).
/// </summary>
public sealed class Position
{
    public double X { get; set; }
    public double Y { get; set; }

    public Position() { }

    public Position(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double DistanceTo(Position other)
    {
        var dx = other.X - X;
        var dy = other.Y - Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public override string ToString() => $"({X:0.###}, {Y:0.###})";
}
