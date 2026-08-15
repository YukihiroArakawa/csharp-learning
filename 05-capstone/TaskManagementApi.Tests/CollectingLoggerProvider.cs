using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace TaskManagementApi.Tests;

public sealed record CollectedLog(
    string Category,
    LogLevel Level,
    string? Template,
    IReadOnlyDictionary<string, object?> Properties);

public sealed class CollectingLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<CollectedLog> logs = new();

    public IReadOnlyCollection<CollectedLog> Logs => logs.ToArray();

    public ILogger CreateLogger(string categoryName)
    {
        return new CollectingLogger(categoryName, logs);
    }

    public void Dispose()
    {
    }

    private sealed class CollectingLogger(
        string categoryName,
        ConcurrentQueue<CollectedLog> logs) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (state is not IEnumerable<KeyValuePair<string, object?>> values)
            {
                return;
            }

            var properties = values.ToDictionary(
                pair => pair.Key,
                pair => pair.Value);
            properties.TryGetValue("{OriginalFormat}", out var template);
            properties.Remove("{OriginalFormat}");

            logs.Enqueue(new CollectedLog(
                categoryName,
                logLevel,
                template as string,
                properties));
        }
    }
}
