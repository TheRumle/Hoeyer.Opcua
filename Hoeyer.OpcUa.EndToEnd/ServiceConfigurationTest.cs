using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.Server.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;


public sealed class ServiceConfigurationTest
{
    [Test]
    [ClassDataSource<OpcUaServiceDescriptorFixture>]
    public async Task EntityBrowser_IsRegistered( OpcUaServiceDescriptorFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityBrowser<>));

    [Test]
    [ClassDataSource<OpcUaServiceDescriptorFixture>]
    public async Task EntityTranslator_IsRegistered( OpcUaServiceDescriptorFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityTranslator<>));
    
    [Test]
    [ClassDataSource<OpcUaServiceDescriptorFixture>]
    public async Task NodeStructureFactory_IsRegistered( OpcUaServiceDescriptorFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityNodeStructureFactory<>));

    [Test]
    [ClassDataSource<OpcUaServiceDescriptorFixture>]
    public async Task EntityInitializer_IsRegistered(OpcUaServiceDescriptorFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityInitializer));
    
    [Test]
    [ClassDataSource<OpcUaServiceDescriptorFixture>]
    public async Task EntityChanged_IsRegistered(OpcUaServiceDescriptorFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityChangedMessenger<>));
    
        
    [Test]
    [AllEntityServiceDescriptorsOfType(typeof(IEntityChangedMessenger<>))]
    public async Task EntityChangedMessenger_IsOnlyRegisteredAsSingleton(IReadOnlyCollection<ServiceDescriptor> messengers)
    {
        var numberOfSingletons = messengers.Count(e => e.Lifetime == ServiceLifetime.Singleton);
        await Assert.That(numberOfSingletons).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        await Assert.That(messengers.Where(e=>e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }  
    
    private static async Task AssertNumberEntitiesMatchesNumberServices(IEnumerable<ServiceDescriptor> collection, Type wantedType)
    {
        Predicate<Type> typeFilter = wantedType.IsGenericTypeDefinition 
            ?  serviceType => serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == wantedType
            : serviceType => serviceType.IsAssignableFrom(wantedType);
        
        var services = collection.Where(e => typeFilter.Invoke(e.ServiceType)).ToList();
        await Assert.That(services).IsNotEmpty();
        await Assert.That(services.Count).IsEqualTo(OpcUaEntityTypes.Entities.Count);
    }

    private static async Task AssertNumberEntitiesMatchesNumberServices(
        IEnumerable<ServiceDescriptor> collection, params Type[] wantedTypes)
        =>  await Task.WhenAll(wantedTypes.Select(async t =>
            await AssertNumberEntitiesMatchesNumberServices(collection.ToList(), t)));
}