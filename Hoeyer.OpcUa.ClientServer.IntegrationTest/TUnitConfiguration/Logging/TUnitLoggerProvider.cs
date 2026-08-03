using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.IntegrationTest.TUnitConfiguration.Logging;

public sealed class TUnitLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

    public ILogger CreateLogger(string categoryName)
        => new TUnitLogger(categoryName, _scopeProvider);

    public void Dispose()
    {
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }
}