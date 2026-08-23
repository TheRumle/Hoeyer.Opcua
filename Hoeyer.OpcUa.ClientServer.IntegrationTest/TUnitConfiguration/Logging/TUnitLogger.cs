using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.IntegrationTest.TUnitConfiguration.Logging;

public sealed class TUnitLogger(
    string categoryName,
    IExternalScopeProvider scopeProvider)
    : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull =>
        scopeProvider.Push(state);

    public bool IsEnabled(LogLevel logLevel)
        => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var context = TestContext.Current;

        if (context == null)
        {
            return;
        }

        var message = formatter(state, exception);

        context.OutputWriter.WriteLine(
            $"[{logLevel} - {DateTime.Now.ToString(CultureInfo.InvariantCulture)}] {categoryName}: {message}");

        scopeProvider.ForEachScope(
            (scope, writer) => { writer.WriteLine($"  Scope: {scope}"); },
            context.OutputWriter);

        if (exception != null)
        {
            context.OutputWriter.WriteLine(exception.ToString());
        }
    }
}