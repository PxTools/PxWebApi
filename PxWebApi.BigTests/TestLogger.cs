using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

public class TestLogger<T> : ILogger<T>, IDisposable
{
    private readonly List<string> _logMessages = new List<string>();

    public IReadOnlyList<string> LogMessages => _logMessages;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => this;

    public void Dispose() { }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string? message = "Level: " + logLevel.ToString() + ", ";
        if (exception != null)
        {
            message += "Exception: " + exception.ToString() + ", ";
        }

        message += formatter(state, exception);

        _logMessages.Add(message);
    }
}
