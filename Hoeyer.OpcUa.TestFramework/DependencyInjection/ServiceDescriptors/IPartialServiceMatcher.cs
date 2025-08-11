using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public interface IPartialServiceMatcher : IEquatable<ServiceDescriptor>
{
    Type ServiceType { get; }
    ServiceLifetime Lifetime { get; }
    Type? Implementation { get; }
}

public static class ServiceDescriptorExtensions
{
    public static IEnumerable<ServiceDescriptor> Where(this IEnumerable<ServiceDescriptor> descriptors,
        IPartialServiceMatcher matcher) => descriptors.Where(matcher.Equals);
}