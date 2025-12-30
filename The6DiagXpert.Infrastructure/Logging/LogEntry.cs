using System;

namespace The6DiagXpert.Infrastructure.Logging;

/// <summary>
/// Entrée de log standardisée.
/// </summary>
public sealed class LogEntry
{
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public LogLevel Level { get; set; } = LogLevel.Information;

    public string Category { get; set; } = "App";

    public string Message { get; set; } = string.Empty;

    public string? Operation { get; set; }

    public string? CorrelationId { get; set; }

    public string? Details { get; set; }

    public string? ExceptionType { get; set; }

    public string? StackTrace { get; set; }
}
