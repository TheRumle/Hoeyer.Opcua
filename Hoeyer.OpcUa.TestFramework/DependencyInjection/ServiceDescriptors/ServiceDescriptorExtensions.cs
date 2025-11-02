using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public static class ServiceDescriptorExtensions
{
    public static IEnumerable<ServiceDescriptor> Where(this IEnumerable<ServiceDescriptor> descriptors,
        IPartialServiceMatcher matcher) => descriptors.Where(matcher.Equals);
}