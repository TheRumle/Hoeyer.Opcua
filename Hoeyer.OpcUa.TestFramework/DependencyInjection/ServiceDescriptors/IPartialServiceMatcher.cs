using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public interface IPartialServiceMatcher : IEquatable<ServiceDescriptor>
{
    Type ServiceType { get; init; }
    ServiceLifetime Lifetime { get; init; }
    Type? Implementation { get; init; }
}