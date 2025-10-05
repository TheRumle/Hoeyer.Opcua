using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration;

/// <summary>
///     A service collection where an <see cref="OpcUaTargetServerInfo" /> has been registered.
/// </summary>
/// <param name="Collection"></param>
public record OnGoingOpcEntityServiceRegistration(IServiceCollection Collection)
{
    public IServiceCollection Collection { get; } = Collection;
}