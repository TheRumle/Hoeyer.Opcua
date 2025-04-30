using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class FilteredCollection( IEnumerable<ServiceDescriptor> services, Type wanted)
{
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
    
    public readonly IEnumerable<ServiceDescriptor> Descriptors = services.Where(s => MatchesFilterType(s, GetType(wanted)));

    private static bool MatchesFilterType(ServiceDescriptor descriptor, Type wanted)
    {
        var serviceType = descriptor.ServiceType;
        var implType = descriptor.ImplementationType;
        return MatchesFilterType(serviceType, wanted) || MatchesFilterType(implType, wanted);
    }

    private static bool MatchesFilterType(Type? type, Type wanted)
    {
        if (type == null) return false;
        if (wanted.IsGenericTypeDefinition)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == wanted;
        }

        if (type.IsConstructedGenericType)
        {
            return type.GetGenericTypeDefinition() == wanted;
        }

        if (!wanted.IsGenericTypeDefinition && !type.IsConstructedGenericType)
        {
            return wanted.IsAssignableFrom(type);
        }
        
        return type == wanted;
    }
}