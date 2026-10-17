using System.ComponentModel;
using Hoeyer.Common.Architecture;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Hoeyer.OpcUa.Core.Configuration.Application;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Core.Configuration.Options;
using Hoeyer.OpcUa.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration;

public static class ServiceCollectionExtensions
{
    public static OnGoingOpcEntityServiceRegistration AddOpcUaFromEnvironmentVariables(
        this IHostApplicationBuilder applicationBuilder,
        ApplicationConfigurationSetup configureOpcUaDefaults = null!
    )
    {
        applicationBuilder.Configuration.AddEnvironmentVariables();
        return applicationBuilder.Services.AddOpcUaFromOptions(configureOpcUaDefaults);
    }

    public static OnGoingOpcEntityServiceRegistrationWithModels WithEntityModelsFrom(
        this OnGoingOpcEntityServiceRegistration registration,
        params IEnumerable<Type> markerTypes)
    {
        var assemblies = markerTypes.Select(e => new AssemblyMarker(e)).ToArray();
        registration.Collection.WithEntityModelsFrom(assemblies);
        return new OnGoingOpcEntityServiceRegistrationWithModels(
            registration.Collection,
            registration.Collection.BuildServiceProvider()
                .GetRequiredService<EntityTypesCollection>());
    }

    public static OnGoingOpcEntityServiceRegistration AddOpcUa(this IServiceCollection services,
        Func<IApplicationTargetConfigurationBuilder, IApplicationConfigurationRequirements> configurationBuilder,
        ApplicationConfigurationSetup? configureOpcUaApplicationConfiguration = null
    )
    {
        var entityServerConfiguration = configurationBuilder.Invoke(ApplicationRequirementBuilder.Create());
        services.AddSingleton<IApplicationRequirementsFactory>(new DefaultFactory(() => entityServerConfiguration));
        services.AddApplicationConfiguration(configureOpcUaApplicationConfiguration);

        return new OnGoingOpcEntityServiceRegistration(services);
    }

    private static void AddApplicationConfiguration(this IServiceCollection services,
        ApplicationConfigurationSetup? configureOpcUaApplicationConfiguration)
    {
        services.AddSingleton(configureOpcUaApplicationConfiguration ?? (_ => { }));
        services.AddSingleton<ITelemetryContext>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerProvider>();
            return DefaultTelemetry.Create(configure =>
            {
                configure.AddProvider(loggerFactory).SetMinimumLevel(LogLevel.Debug);
            });
        });
        services.AddSingleton<IApplicationConfigurationRequirements>(p => p.GetRequiredService<IApplicationRequirementsFactory>().Get());
        services.AddSingleton<IApplicationConfigurationTemplateFactory, ApplicationConfigurationTemplateFactory>();
        services.AddSingleton<IApplicationSecurityConfigurationFactory, CertificateBasedSecurityConfigurationFactory>();
    }

    public static OnGoingOpcEntityServiceRegistration AddOpcUaFromOptions(this IServiceCollection services,
        ApplicationConfigurationSetup? configureOpcUaApplicationConfiguration = null)
    {
        services.AddOptions<OpcUaOptions>()
            .BindConfiguration("OpcUa")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CertificateConfigurationOptions>()
            .BindConfiguration("OpcUa:CertificateConfiguration")
            .ValidateDataAnnotations()
            .ValidateOnStart();


        services.AddSingleton<IApplicationRequirementsFactory, EnvironmentVariableRequirementsFactory>();
        services.AddApplicationConfiguration(configureOpcUaApplicationConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }

    public static OnGoingOpcEntityServiceRegistrationWithModels WithEntityModelsFrom(
        this IServiceCollection collection,
        params AssemblyMarker[] assemblyMarkers
    )
    {
        var assemblyTypes = assemblyMarkers.SelectMany(e => e.TypesInAssembly).ToArray();
        collection.AddKeyedSingleton(ServiceKeys.MODELLING, new ServiceAssemblyMarkerCollection(assemblyMarkers));
        collection.AddSingleton<EntityTypesCollection>();
        collection.AddSingleton<TranslatorTypesCollection>();
        
        var provider = collection.BuildServiceProvider();
        var entityTypesCollection = provider.GetRequiredService<EntityTypesCollection>();
        
        AddTypeModel(collection, entityTypesCollection);
        AddModelFactory(collection, assemblyTypes);

        var translatorCollection = provider.GetRequiredService<TranslatorTypesCollection>();
        foreach (var translator in translatorCollection.Translators)
        {
            collection.AddSingleton(translator.Service, translator.Impl);
        }

        return new OnGoingOpcEntityServiceRegistrationWithModels
        (
            collection,
            entityTypesCollection
        );
    }

    private static void AddTypeModel(this IServiceCollection collection, EntityTypesCollection entities)
    {
        collection.AddSingleton(typeof(IBrowseNameCollection<>), typeof(EntityTypeModel<>));
        collection.AddSingleton(typeof(IEntityTypeModel<>), typeof(EntityTypeModel<>));
        collection.AddSingleton(typeof(IBehaviourTypeModel<>), typeof(EntityTypeModel<>));
        foreach (var entity in entities.ModelledEntities)
        {
            collection.AddSingleton(
                typeof(IBrowseNameCollection),
                provider => provider.GetRequiredService(typeof(IBrowseNameCollection<>).MakeGenericType(entity))
            );
        }
    }

    private static void AddModelFactory(IServiceCollection collection, Type[] assemblyTypes)
    {
        collection.AddServiceAndImplSingleton(typeof(IEntityNodeStructureFactory<>),
            typeof(ReflectionBasedEntityStructureFactory<>));
        collection.AddServiceAndImplSingleton(typeof(IEntityNodeMethodAssigner<>), typeof(EntityNodeMethodAssigner<>));
        collection.AddServiceAndImplSingleton(typeof(IEntityNodePropertyAssigner<>),
            typeof(EntityNodePropertyAssigner<>));
        collection.AddServiceAndImplSingleton(typeof(IEntityNodeAlarmAssigner<>), typeof(EntityNodeAlarmAssigner<>));
        collection.AddVariantBuilders([.. assemblyTypes, typeof(UuidConverter)]);
        collection.AddPropertyStateFactories(assemblyTypes);
    }

    private static void AddTranslators(IServiceCollection services)
    {
        var translatorCollection = services.BuildServiceProvider().GetRequiredService<TranslatorTypesCollection>();
        foreach (var translator in translatorCollection.Translators)
        {
            services.AddSingleton(translator.Service, translator.Impl);
        }
    }

    private static void AddPropertyStateFactories(this IServiceCollection collection, IEnumerable<Type> assemblyTypes)
    {
        var propertyStateFactories = assemblyTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsPublic: true })
            .Select(type => (ImplType: type, ServiceType: GetIsPropertyStateFactoryInterface(type)))
            .Where(tuple => tuple.ServiceType is not null)
            .ToArray();

        foreach (var (implType, serviceType) in propertyStateFactories)
        {
            collection.AddSingleton(serviceType!, implType);
        }
    }

    private static Type? GetIsPropertyStateFactoryInterface(Type type)
    {
        return type.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPropertyStateFactory<>));
    }
}