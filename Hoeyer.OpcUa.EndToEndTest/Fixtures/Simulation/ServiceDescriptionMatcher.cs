using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

/**
 * Given a type T and an
 * <see cref="IServiceCollection" />
 * collection, examines the collection for all registered services that could be used for that type.
 */
public sealed class ServiceDescriptionMatcher(
    Type type,
    IEnumerable<ServiceDescriptor> collection,
    EntityTypesCollection? entityTypesCollection)
{
    public IEnumerable<ServiceDescriptor> GetMatchingDescriptors() =>
        FilterDescriptors().SelectMany(EntityInstantiatedVariantsOfServiceDescriptors);

    private IEnumerable<ServiceDescriptor> EntityInstantiatedVariantsOfServiceDescriptors(
        DescriptorUsage descriptionWrapper) =>
        descriptionWrapper.WrapsAnyOpenGeneric
            ? GetTypePerEntity(descriptionWrapper)
            : [descriptionWrapper.Descriptor];

    private IEnumerable<ServiceDescriptor> GetTypePerEntity(DescriptorUsage descriptorWrapper)
    {
        var oldDescriptor = descriptorWrapper.Descriptor;
        foreach (var entityType in entityTypesCollection.ModelledEntities)
        {
            var impl = oldDescriptor.ImplementationType!.MakeGenericType(entityType);
            var service = oldDescriptor.ServiceType.MakeGenericType(entityType);
            yield return new ServiceDescriptor(service, impl, oldDescriptor.Lifetime);
        }
    }

    private IEnumerable<DescriptorUsage> FilterDescriptors()
    {
        var usage = ExamineAttributeUsage(type);
        return usage switch
        {
            AttributeUsage.ConcreteClass => GetDescriptorsForConcreteClass(),
            AttributeUsage.Interface => GetDescriptorsForConcreteInterface(),
            AttributeUsage.ClosedGenericClass => GetDescriptorsForClosedGenericInterface(),
            AttributeUsage.ClosedGenericInterface => GetDescriptorsForClosedGenericClass(),
            var _ => throw new InvalidOperationException("Attribute usage was an unexpected value")
        };
    }

    private static AttributeUsage ExamineAttributeUsage(Type t)
    {
        var usage = t switch
        {
            { IsInterface: false, IsGenericType: false } => AttributeUsage.ConcreteClass,
            { IsInterface: true, IsGenericType: false } => AttributeUsage.Interface,
            { IsGenericTypeDefinition: true, IsInterface: true } => AttributeUsage.ClosedGenericClass,
            { IsGenericTypeDefinition: true, IsClass: true } => AttributeUsage.ClosedGenericInterface,
            var _ => throw new ArgumentException(
                "The generic argument did not match any expected pattern. It was neither concrete, concrete interface, closed generic, or closed generic interface: " +
                new
                {
                    t.IsAbstract,
                    t.IsGenericType,
                    t.IsGenericTypeDefinition,
                    t.IsInterface,
                    t.IsClass
                })
        };
        return usage;
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForClosedGenericClass()
    {
        //type usage is of format MyValue<int>
        return collection.Where(e =>
                MatchesGenerically(type, e.ServiceType) || MatchesGenerically(type, e.ImplementationType))
            .Select(d => new DescriptorUsage(d));
    }

    private static bool MatchesGenerically(Type type, Type? serviceType)
    {
        var serviceTypeMatches =
            serviceType != null && (serviceType == type ||
                                    (serviceType.IsGenericTypeDefinition && type.IsClosedGenericOf(serviceType)));
        return serviceTypeMatches;
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForClosedGenericInterface()
    {
        return collection.Where(e =>
        {
            var serviceType = e.ServiceType;
            return serviceType == type || (serviceType.IsGenericTypeDefinition && type.IsClosedGenericOf(serviceType));
        }).Select(d => new DescriptorUsage(d));
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForConcreteInterface()
    {
        //Descriptors usage is IInterface
        //need to find all services that implement that interface
        return collection.Where(e =>
                e.ServiceType == type || e.ServiceType.Implements(type) || e.ServiceType.IsAssignableTo(type))
            .Select(d => new DescriptorUsage(d));
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForConcreteClass()
    {
        return collection.Where(e => e.ServiceType == type || type == e.ImplementationType)
            .Select(d => new DescriptorUsage(d));
    }


    private enum AttributeUsage
    {
        ConcreteClass,
        Interface,
        ClosedGenericClass,
        ClosedGenericInterface
    }

    private readonly record struct DescriptorUsage(ServiceDescriptor Descriptor)
    {
        public bool WrapsOpenGenericService { get; } = Descriptor.ServiceType.IsGenericTypeDefinition;

        public bool WrapsOpenGenericImpl { get; } = Descriptor.ImplementationType is not null &&
                                                    Descriptor.ImplementationType.IsGenericTypeDefinition;

        public bool WrapsAnyOpenGeneric => WrapsOpenGenericService || WrapsOpenGenericImpl;
    }
}