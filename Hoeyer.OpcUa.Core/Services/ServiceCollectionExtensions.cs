using System;
using System.Collections.Frozen;
using System.Linq;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public static class ServiceCollectionExtensions
{
    private static readonly Type NonGenericTranslatorInterface = typeof(IEntityTranslator<>);

    private static (Type Services, Type Impl) ConstructServiceImplTuple(Type type)
    {
        try
        {
            return (
                Services: type.GetInterfaces().FirstOrDefault(@interface =>
                    @interface.IsGenericImplementationOf(NonGenericTranslatorInterface)),
                Impl: type);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("constructing tuple for type " + type.FullName + " threw exception");
        }
    }


    public static OnGoingOpcEntityServiceRegistration AddOpcUa(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }


    public static IServiceCollection WithEntityServices(this IServiceCollection services)
    {
        var marker = typeof(ServiceCollectionExtensions);
        var servicesAndImpls = OpcUaEntityTypes.TypesFromReferencingAssembliesUsingMarker(marker)
            .Where(type => type is { IsInterface: false, IsAbstract: false, IsGenericTypeDefinition: false })
            .Select(ConstructServiceImplTuple)
            .Where(e => e.Services is not null)
            .ToFrozenSet();

        foreach (var (service, impl) in servicesAndImpls)
        {
            services.AddServiceAndImplSingleton(service, impl);
        }

        services.AddSingleton(typeof(IEntityNodeStructureFactory<>), typeof(ReflectionBasedEntityStructureFactory<>));
        services.AddSingleton(typeof(IBrowseNameCollection<>), typeof(BrowseNameCollection<>));
        return services;
    }

    public static OnGoingOpcEntityServiceRegistration WithEntityServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        registration.Collection.WithEntityServices();
        return registration;
    }
}