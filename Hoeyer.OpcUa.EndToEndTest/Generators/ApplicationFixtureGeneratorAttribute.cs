using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

[SuppressMessage("Maintainability", "S2743")]
public sealed class ApplicationFixtureGeneratorAttribute<T> : DataSourceGeneratorAttribute<ApplicationFixture<T>>
    where T : notnull
{
    private readonly IServiceCollection _services = new ServiceCollection().AddTestServices();
    private readonly EntityTypesCollection EntityTypes;

    public ApplicationFixtureGeneratorAttribute()
    {
        EntityTypes = _services.BuildServiceProvider().GetService<EntityTypesCollection>()!;
    }


    protected override IEnumerable<Func<ApplicationFixture<T>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        var protocolList = new List<WebProtocol> { WebProtocol.OpcTcp };
        var servicesUnderTest = FilterDescriptors()
            .SelectMany(EntityInstantiatedVariantsOfServiceDescriptors)
            .DistinctBy(e => e.ImplementationType)
            .Where(e => e.ImplementationType != null)
            .Select(e => e.ImplementationType!)
            .ToList();

        return protocolList
            .SelectMany(protocol => servicesUnderTest
                .Select(type => new Func<ApplicationFixture<T>>(() => new ApplicationFixture<T>(type, protocol)))
            );
    }

    private IEnumerable<ServiceDescriptor> EntityInstantiatedVariantsOfServiceDescriptors(
        DescriptorUsage descriptionWrapper) =>
        descriptionWrapper.WrapsAnyOpenGeneric
            ? GetTypePerEntity(descriptionWrapper)
            : [descriptionWrapper.Descriptor];

    private IEnumerable<ServiceDescriptor> GetTypePerEntity(DescriptorUsage descriptorWrapper)
    {
        var oldDescriptor = descriptorWrapper.Descriptor;
        foreach (var entityType in EntityTypes.ModelledEntities)
        {
            var impl = oldDescriptor.ImplementationType!.MakeGenericType(entityType);
            var service = oldDescriptor.ServiceType.MakeGenericType(entityType);
            yield return new ServiceDescriptor(service, impl, oldDescriptor.Lifetime);
        }
    }

    private IEnumerable<DescriptorUsage> FilterDescriptors()
    {
        var t = typeof(T);
        var usage = ExamineAttributeUsage(t);
        return usage switch
        {
            AttributeUsage.ConcreteClass => GetDescriptorsForConcreteClass(_services, t),
            AttributeUsage.Interface => GetDescriptorsForConcreteInterface(_services, t),
            AttributeUsage.ClosedGenericClass => GetDescriptorsForClosedGenericInterface(_services, t),
            AttributeUsage.ClosedGenericInterface => GetDescriptorsForClosedGenericClass(_services, t),
            var _ => throw new InvalidOperationException("Attribute usage was an unexpected value")
        };
    }

    private AttributeUsage ExamineAttributeUsage(Type t)
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

    private IEnumerable<DescriptorUsage> GetDescriptorsForClosedGenericClass(IServiceCollection collection,
        Type type)
    {
        //type usage is of format MyValue<int>
        return collection.Where(e =>
                MatchesGenerically(type, e.ServiceType) || MatchesGenerically(type, e.ImplementationType))
            .Select(d => new DescriptorUsage(d));
    }

    private bool MatchesGenerically(Type type, Type? serviceType)
    {
        var serviceTypeMatches =
            serviceType != null && (serviceType == type ||
                                    (serviceType.IsGenericTypeDefinition && type.IsClosedGenericOf(serviceType)));
        return serviceTypeMatches;
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForClosedGenericInterface(IServiceCollection collection,
        Type type)
    {
        return collection.Where(e =>
        {
            var serviceType = e.ServiceType;
            return serviceType == type || (serviceType.IsGenericTypeDefinition && type.IsClosedGenericOf(serviceType));
        }).Select(d => new DescriptorUsage(d));
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForConcreteInterface(IServiceCollection collection,
        Type type)
    {
        //Type usage is IInterface
        //need to find all services that implement that interface
        return collection.Where(e =>
                e.ServiceType == type || e.ServiceType.Implements(type) || e.ServiceType.IsAssignableTo(type))
            .Select(d => new DescriptorUsage(d));
    }

    private IEnumerable<DescriptorUsage> GetDescriptorsForConcreteClass(IServiceCollection collection,
        Type type)
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