using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration;

/// <summary>
///     A service collection where an <see cref="OpcUaAgentServerInfo" /> has been registered.
/// </summary>
/// <param name="Collection"></param>
public record OnGoingOpcAgentServiceRegistration(IServiceCollection Collection)
{
    public IServiceCollection Collection { get; } = Collection;
}