using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Configuration;

/// <summary>
/// A service collection where an <see cref="OpcUaEntityServerConfiguration"/> has been registered.
/// </summary>
/// <param name="Collection"></param>
public record OnGoingOpcEntityServiceRegistration(IServiceCollection Collection) 
{
    public IServiceCollection Collection { get; } = Collection;
}