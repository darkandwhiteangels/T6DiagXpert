using System;
using System.IO;
using System.Text.Json;

namespace The6DiagXpert.Infrastructure.Logging;

/// <summary>
/// Logger fichier très simple (Phase 6).
/// - Peut écrire en texte ou en JSON lines (1 log = 1 ligne JSON)
/// - Thread-safe via lock
/// </summary>
public sealed class FileLogger
{
    private readonly object _lock = new();
    private readonly string _filePath;
    private readonly bool _writeJsonLines;
    private readonly JsonSerializerOptions _jsonOptions;

    public FileLogger(string logDirectory, string fileName = "the6diagxpert.log", bool writeJsonLines = false)
    {
        _writeJsonLines = writeJsonLines;

        if (string.IsNullOrWhiteSpace(logDirectory))
        {
            logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
        }

        Directory.CreateDirectory(logDirectory);

        _filePath = Path.Combine(logDirectory, fileName);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };
    }

    public string FilePath => _filePath;

    public void Log(LogLevel level, string message, string category = "App", string? operation = null, string? details = null)
    {
        var entry = new LogEntry
        {
            TimestampUtc = DateTime.UtcNow,
            Level = level,
            Category = category,
            Message = message ?? string.Empty,
            Operation = operation,
            Details = details
        };

        Write(entry);
    }

    public void LogException(Exception ex, string message, string category = "App", string? operation = null)
    {
        if (ex == null) throw new ArgumentNullException(nameof(ex));

        var entry = new LogEntry
        {
            TimestampUtc = DateTime.UtcNow,
            Level = LogLevel.Error,
            Category = category,
            Message = message ?? ex.Message ?? "Exception",
            Operation = operation,
            ExceptionType = ex.GetType().FullName,
            Details = ex.Message,
            StackTrace = ex.StackTrace
        };

        Write(entry);
    }

    private void Write(LogEntry entry)
    {
        lock (_lock)
        {
            var line = _writeJsonLines
                ? JsonSerializer.Serialize(entry, _jsonOptions)
                : $"[{entry.TimestampUtc:O}] [{entry.Level}] [{entry.Category}] {(string.IsNullOrWhiteSpace(entry.Operation) ? "" : $"({entry.Operation}) ")}{entry.Message}";

            if (!_writeJsonLines && !string.IsNullOrWhiteSpace(entry.Details))
                line += $" | Details: {entry.Details}";

            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }
}
