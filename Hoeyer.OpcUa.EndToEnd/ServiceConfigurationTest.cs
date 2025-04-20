using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.Server.Api.Management;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest;


public sealed class ServiceConfigurationTest
{
    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task EntityBrowser_IsRegistered( AllOpcUaServicesFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityBrowser<>));

    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task EntityTranslator_IsRegistered( AllOpcUaServicesFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityTranslator<>));
    
    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task NodeStructureFactory_IsRegistered( AllOpcUaServicesFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityNodeStructureFactory<>));

    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task EntityInitializer_IsRegistered(AllOpcUaServicesFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityServiceContainer));
    
    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task EntityChanged_IsRegistered(AllOpcUaServicesFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityChangedMessenger<>));
    
    [Test]
    [ClassDataSource<AllOpcUaServicesFixture>]
    public async Task EntityMonitor_IsRegistered(AllOpcUaServicesFixture fixture) 
        => await AssertNumberEntitiesMatchesNumberServices(fixture.Services, typeof(IEntityMonitor<>));
    
        
    [Test]
    [AllEntityServiceDescriptorsOfType(typeof(IEntityMonitor<>))]
    public async Task EntityMonitor_IsOnlyRegisteredAsSingleton(IReadOnlyCollection<ServiceDescriptor> messengers)
    {
        var numberOfSingletons = messengers.Count(e => e.Lifetime == ServiceLifetime.Singleton);
        await Assert.That(numberOfSingletons).IsEqualTo(OpcUaEntityTypes.Entities.Count);
        await Assert.That(messengers.Where(e=>e.Lifetime != ServiceLifetime.Singleton)).IsEmpty();
    }  
    
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
}