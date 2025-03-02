using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

/// <summary>
///     Uses the <paramref name="services" /> to create a <see cref="ServiceProvider" /> and uses it to wire up services
///     necessary for configuring OpcUa entities. Disposing the <see cref="ServiceRegistry" /> will remove services used
///     for internal wiring from <paramref name="services" />.
/// </summary>
/// <param name="services">
///     The collection providing the register with services necessary for the creation of a number of
///     services related to OpcUa state observation
/// </param>
internal sealed class ServiceRegistry(IServiceCollection services)
{
}