using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class ServiceDescriptorGenerator(Type type, params Type[] types )
{
    private readonly Type[] _filterTypes = [GetType(type), ..types.Select(GetType)];
    private static Type GetType(Type type)
    {
        if (type.IsConstructedGenericType)
        {
            return type.GetGenericTypeDefinition();
        }
        if (type.IsGenericTypeDefinition)
        {
            return type;
        }

        return type;
    }
    
    public IEnumerable<ServiceDescriptor> CreateServiceDescriptors()
    {
        return new AllOpcUaServicesFixture()
            .Services
            .Where(MatchesFilterType);
    }

    private bool MatchesFilterType(ServiceDescriptor descriptor)
    {
        var serviceType = descriptor.ServiceType;
        var implType = descriptor.ImplementationType;
        return MatchesFilterType(serviceType) || MatchesFilterType(implType);
    }

    private bool MatchesFilterType(Type? type)
    {
        return type != null &&_filterTypes.Any(filterType =>
            type == filterType 
            || type.IsSubclassOf(filterType) 
            || type.IsAssignableFrom(filterType)  
            || (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == filterType));
    }
}