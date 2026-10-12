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
        params IEnumerable<Type> assemblyMarkers)
    {
        registration.Collection.WithEntityModelsFrom(assemblyMarkers);
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

    public static IServiceCollection WithEntityModelsFrom(this IServiceCollection services,
        params IEnumerable<Type> assemblyMarkers)
    {
        var markers = assemblyMarkers.ToList();
        services.AddEntityModels(markers);
        services.AddSingleton<TranslatorTypesCollection>();
        services.AddSingleton(typeof(IBrowseNameCollection<>), typeof(EntityTypeModel<>));
        services.AddSingleton(typeof(IEntityTypeModel<>), typeof(EntityTypeModel<>));
        services.AddSingleton(typeof(IBehaviourTypeModel<>), typeof(EntityTypeModel<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodeStructureFactory<>),
            typeof(ReflectionBasedEntityStructureFactory<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodeMethodAssigner<>), typeof(EntityNodeMethodAssigner<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodePropertyAssigner<>),
            typeof(EntityNodePropertyAssigner<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodeAlarmAssigner<>), typeof(EntityNodeAlarmAssigner<>));
        AddTranslators(services);
        
        services.AddVariantConverters([..markers, typeof(GuidConverter)]);
        return services;
    }

    public static IServiceCollection AddEntityModels(this IServiceCollection services, List<Type> markers)
    {
        services.AddKeyedSingleton(ServiceKeys.MODELLING, markers.Select(m => new AssemblyMarker(m)));
        services.AddSingleton<EntityTypesCollection>();
        services.AddPropertyTranslators(markers.SelectMany(e => e.Assembly.GetTypes()));
        return services;
    }

    private static void AddTranslators(IServiceCollection services)
    {
        var translatorCollection = services.BuildServiceProvider().GetRequiredService<TranslatorTypesCollection>();
        foreach (var translator in translatorCollection.Translators)
        {
            services.AddSingleton(translator.Service, translator.Impl);
        }
    }

    private static void AddPropertyTranslators(this IServiceCollection collection, IEnumerable<Type> assemblyTypes)
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
        return type.GetInterfaces().FirstOrDefault(i =>i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPropertyStateFactory<>));
    }
}