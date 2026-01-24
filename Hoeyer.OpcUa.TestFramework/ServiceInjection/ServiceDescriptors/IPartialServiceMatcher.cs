using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Test.ServiceInjection.ServiceDescriptors;

public interface IPartialServiceMatcher : IEquatable<ServiceDescriptor>
{
    Type ServiceType { get; }
    ServiceLifetime Lifetime { get; }
    Type? Implementation { get; }
}