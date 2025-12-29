namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Définit les différents types de fichiers gérés par le système.
/// </summary>
public enum FileType
{
    /// <summary>
    /// Document PDF (Portable Document Format).
    /// </summary>
    PDF = 0,

    /// <summary>
    /// Fichier image (JPEG, PNG, etc.).
    /// </summary>
    Image = 1,

    /// <summary>
    /// Document bureautique (Word, Excel, etc.).
    /// </summary>
    Document = 2,

    /// <summary>
    /// Plan technique (DWG, DXF, etc.).
    /// </summary>
    Plan = 3
}
