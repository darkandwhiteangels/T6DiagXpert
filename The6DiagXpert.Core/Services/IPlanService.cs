using System;
using System.Collections.Generic;
using The6DiagXpert.Core.Models.Plans;

namespace The6DiagXpert.Core.Services;

/// <summary>
/// Contrat métier du module Plan (calculs, validations, helpers).
/// L'implémentation arrivera quand on attaquera les services/domain + data.
/// </summary>
public interface IPlanService
{
    // Scale helpers
    bool IsScaleValid(Scale? scale);

    double PixelsToMeters(Scale scale, double pixels);
    double MetersToPixels(Scale scale, double meters);

    // Geometry helpers (pixels)
    double DistancePixels(Position a, Position b);

    // Geometry helpers (mètres) - si scale valide
    double DistanceMeters(Position a, Position b, Scale scale);

    // Rooms
    bool IsPolygonValid(IReadOnlyList<Position> polygon, out string? error);
    double ComputePolygonAreaPixels(IReadOnlyList<Position> polygon);
    double ComputePolygonAreaSquareMeters(IReadOnlyList<Position> polygon, Scale scale);

    // Objects
    bool CanPlaceObjectOnCanvas(Plan plan, Position position, out string? error);
    bool CanAssignObjectToRoom(Plan plan, Guid roomId, out string? error);
}
